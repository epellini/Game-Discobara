using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState
    {
        Menu,
        ReadyForInput,
        Moving,
        GameOver
    }

    [SerializeField] private Transform Player;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject crossfadePanel;
    [SerializeField] private TransitionController transitionController;
    private Vector3 TargetDestination = Vector3.zero;
    public GameState CurrentState;
    public Transform Platform;
    public Transform StartPlatform;
    public Transform CurrentPlatform = null;
    private Transform previousPlatform = null;
    private float playerMoveDistance = 1f;
    private bool isFirstPlatform = true;
    public Transform PlatformPreviewPrefab;
    private Vector2 touchStart;
    private Vector2 touchEnd;
    private bool isSwiping = false;

    private float normalSpawnDelay = 1f; // Normal delay between platform spawns
    private float slowMotionSpawnDelay = 2f; // Delay during slow motion
    private bool isSlowMotionActive = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // Before the game starts, set the current state to Menu
        CurrentState = GameState.Menu;
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(false);
    }

    private bool gameStartedPreviously = false;
    public void StartGame()
    {
        if (!gameStartedPreviously)
        {
            // First time starting the game
            Debug.Log("Game started - FIRST time");
            menuPanel.SetActive(false);
            gamePanel.SetActive(true);
            gameOverPanel.SetActive(false);
            CurrentState = GameState.ReadyForInput;
            CurrentPlatform = StartPlatform;
            StartCoroutine(SpawnPlatform());
            gameStartedPreviously = true;
        }
        else
        {
            // Subsequent times starting the game
            ResetGame();
            menuPanel.SetActive(false);
            Debug.Log("Game started - SECOND time");
        }
    }

    void Update()
    {
        if (CurrentState == GameState.Menu)
        {
            gameOverPanel.SetActive(false);
            return;
        }

        if (CurrentState == GameState.GameOver)
        {
            return;
        }

        if (CurrentState == GameState.ReadyForInput) // If the game is ready for input, check for input
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                TargetDestination = Player.position + new Vector3(0, 0, playerMoveDistance);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                TargetDestination = Player.position + new Vector3(-playerMoveDistance, 0, 0);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                TargetDestination = Player.position + new Vector3(0, 0, -playerMoveDistance);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                TargetDestination = Player.position + new Vector3(playerMoveDistance, 0, 0);
                CurrentState = GameState.Moving;
            }

            // Swipe detection for mobile devices
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    touchStart = touch.position;
                    isSwiping = true;
                }
                else if (touch.phase == TouchPhase.Ended && isSwiping)
                {
                    touchEnd = touch.position;
                    isSwiping = false;
                    HandleSwipe();
                }
            }
        }
    }

    [SerializeField] private float movementSpeed = 10f; // Adjust this speed in the Unity Inspector
    private Vector3 velocity = Vector3.zero; // For SmoothDamp

    private void HandleSwipe()
    {
        Vector2 swipeDirection = touchEnd - touchStart;
        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            // Horizontal swipe
            if (swipeDirection.x > 0)
            {
                // Right swipe
                TargetDestination = Player.position + new Vector3(0, 0, -playerMoveDistance);
            }
            else
            {
                // Left swipe
                TargetDestination = Player.position + new Vector3(0, 0, playerMoveDistance);
            }
        }
        else
        {
            // Vertical swipe
            if (swipeDirection.y > 0)
            {
                // Up swipe
                TargetDestination = Player.position + new Vector3(playerMoveDistance, 0, 0);
            }
            else
            {
                // Down swipe
                TargetDestination = Player.position + new Vector3(-playerMoveDistance, 0, 0);
            }
        }
        CurrentState = GameState.Moving;
    }

    private void FixedUpdate()
    {
        if (CurrentState != GameState.Moving) return;

        // Smoothly move the player towards the target destination
        Player.position = Vector3.SmoothDamp(Player.position, new Vector3(TargetDestination.x, Player.position.y, TargetDestination.z), ref velocity, movementSpeed * Time.fixedDeltaTime);

        // Check if the player has reached close to the target destination
        if (Vector3.Distance(Player.position, TargetDestination) < 0.1f)
        {
            Player.position = TargetDestination;
            CurrentState = GameState.ReadyForInput;
        }
    }

    public void ActivateSlowMotion()
    {
        isSlowMotionActive = true;
        StartCoroutine(SlowMotionDuration());
    }

    private IEnumerator SlowMotionDuration()
    {
        yield return new WaitForSeconds(5f); // Duration of the slow motion
        isSlowMotionActive = false;
    }


    private void CreatePlatform(Transform _Platform, int _InValue)
    {
        float platformHeight = StartPlatform.position.y;

        if (_InValue == 0)
        {
            CurrentPlatform = Instantiate(Platform, new Vector3(_Platform.position.x, platformHeight, _Platform.position.z + 1), Quaternion.identity);
        }
        else if (_InValue == 1)
        {
            CurrentPlatform = Instantiate(Platform, new Vector3(_Platform.position.x, platformHeight, _Platform.position.z - 1), Quaternion.identity);
        }
        else if (_InValue == 2)
        {
            CurrentPlatform = Instantiate(Platform, new Vector3(_Platform.position.x + 1, platformHeight, _Platform.position.z), Quaternion.identity);
        }
        else
        {
            CurrentPlatform = Instantiate(Platform, new Vector3(_Platform.position.x - 1, platformHeight, _Platform.position.z), Quaternion.identity);
        }

        // Decide whether to spawn a power-up
        if (Random.value < powerUpSpawnChance)
        {
            // Calculate power-up position
            Vector3 powerUpPosition = CurrentPlatform.position;
            float powerUpVerticalOffset = 0.5f; // Manually set the vertical offset
            powerUpPosition.y += powerUpVerticalOffset; // Position it above the platform

            Instantiate(powerUpPrefab, powerUpPosition, Quaternion.identity, CurrentPlatform);
        }

        // If the previous platform is the start platform and this is the first spawn,
        // don't destroy it immediately. Otherwise, schedule the destruction.
        if (!isFirstPlatform && previousPlatform != null)
        {
            Destroy(previousPlatform.gameObject, 0.5f); // Delay destruction
        }
        previousPlatform = CurrentPlatform;

        // After the first platform is created, set isFirstPlatform to false and schedule destruction of the start platform
        if (isFirstPlatform)
        {
            isFirstPlatform = false;
            if (StartPlatform != null)
            {
                StartCoroutine(DeactivateAfterDelay(StartPlatform.gameObject, 0.5f));
            }
        }
    }


    [SerializeField] private GameObject powerUpPrefab; // Assign this in the inspector
    private float powerUpSpawnChance = 0.03f; //  3% chance of spawning a power-up

    private IEnumerator SpawnPlatform()
    {
        while (CurrentState != GameState.GameOver)
        {
            float delay = isSlowMotionActive ? slowMotionSpawnDelay : normalSpawnDelay;
            yield return new WaitForSeconds(delay); // Time before showing the preview
            int newValue = Random.Range(0, 4);

            // Calculate the position for the preview
            Vector3 previewPosition = CalculateNextPlatformPosition(CurrentPlatform, newValue);

            // Instantiate the preview platform
            Transform preview = Instantiate(PlatformPreviewPrefab, previewPosition, Quaternion.identity);

            yield return new WaitForSeconds(0.3f); // Time for the player to see the preview

            // Replace the preview with the actual platform
            Destroy(preview.gameObject);
            CreatePlatform(CurrentPlatform, newValue);
        }
    }


    private Vector3 CalculateNextPlatformPosition(Transform currentPlatform, int direction)
    {
        // Assuming all platforms should be at the same height as the start platform
        float platformHeight = StartPlatform.position.y;
        Vector3 nextPosition = currentPlatform.position;

        if (direction == 0)
        {
            nextPosition = new Vector3(currentPlatform.position.x, platformHeight, currentPlatform.position.z + 1);
        }
        else if (direction == 1)
        {
            nextPosition = new Vector3(currentPlatform.position.x, platformHeight, currentPlatform.position.z - 1);
        }
        else if (direction == 2)
        {
            nextPosition = new Vector3(currentPlatform.position.x + 1, platformHeight, currentPlatform.position.z);
        }
        else if (direction == 3)
        {
            nextPosition = new Vector3(currentPlatform.position.x - 1, platformHeight, currentPlatform.position.z);
        }
        return nextPosition;
    }

    private IEnumerator DeactivateAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    public void PlayerFell()
    {
        CurrentState = GameState.GameOver;
        gamePanel.SetActive(false);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        // Here I can add more game over logic or UI updates
        // Show the final score
        ScoreManager.Instance.ShowFinalScore();
        ScoreManager.Instance.ShowHighScore();
    }

    // Go back to menu
    public void GoToMenu()
    {
        CurrentState = GameState.Menu;
        // Reset the game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        // Reset the menu panel
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }
    }

    public void ResetGame()
    {
        transitionController.StartFadeOut();
        StartCoroutine(ResetGameCoroutine());
    }

    private IEnumerator ResetGameCoroutine()
    {
        // Wait for 1 second before proceeding
        //transitionController.StartFadeOut();
        yield return new WaitForSeconds(0.8f);

        // Ensure the StartPlatform is active
        if (StartPlatform != null)
        {
            StartPlatform.gameObject.SetActive(true);
            StartPlatform.position = new Vector3(0, 0, 0);
            //Player.position = StartPlatform.position; // Reset player position to the start position
        }
        else
        {
            Debug.LogError("StartPlatform is not assigned in the GameManager.");
        }
        // 1. Reset GameState
        CurrentState = GameState.ReadyForInput;

        // 2. Reset Player Position and Movement
        Player.GetComponent<PlayerController>().ResetPlayer();
        Player.position = new Vector3(0, 0.35f, 0);
        TargetDestination = Vector3.zero;

        // 3. Platform Handling
        if (CurrentPlatform != null && CurrentPlatform != StartPlatform)
        {
            Destroy(CurrentPlatform.gameObject);
        }
        if (previousPlatform != null && previousPlatform != StartPlatform)
        {
            Destroy(previousPlatform.gameObject);
        }

        //CurrentPlatform = Instantiate(StartPlatform, new Vector3(0, 0, 0), Quaternion.identity);
        CurrentPlatform = StartPlatform;
        previousPlatform = null;
        isFirstPlatform = true;

        // 4. UI Elements
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        gamePanel.SetActive(true);

        // 5. Reset Touch Input and Swipe Handling
        touchStart = Vector2.zero;
        touchEnd = Vector2.zero;
        isSwiping = false;

        // 6. Score and High Score Reset (if applicable)
        ScoreManager.Instance.ResetScore();
        // ScoreManager.Instance.ResetHighScore(); // If you have a high score system

        StopCoroutine(SpawnPlatform());
        StartCoroutine(SpawnPlatform());

    }

}