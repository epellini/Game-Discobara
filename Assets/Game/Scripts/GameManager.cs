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

            // Schedule the removal of StartPlatform after a few seconds
            StartCoroutine(DeactivateAfterDelay(StartPlatform.gameObject, 2f)); // Set the delay as needed

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


    [SerializeField] private float rotationSpeed = 100f;
    private Vector3 targetDirection;

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
            playerAnimator.SetTrigger("Idle");
            if (Input.GetKeyDown(KeyCode.A))
            {
                //targetDirection = Vector3.left;
                playerAnimator.SetTrigger("Moved");
                targetDirection = Vector3.forward;

                TargetDestination = Player.position + new Vector3(0, 0, playerMoveDistance);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                playerAnimator.SetTrigger("Moved");
                targetDirection = Vector3.left;
                //targetDirection = Vector3.down;
                TargetDestination = Player.position + new Vector3(-playerMoveDistance, 0, 0);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                playerAnimator.SetTrigger("Moved");
                //targetDirection = Vector3.right;
                targetDirection = Vector3.back;
                TargetDestination = Player.position + new Vector3(0, 0, -playerMoveDistance);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                playerAnimator.SetTrigger("Moved");
                targetDirection = Vector3.right;
                //targetDirection = Vector3.up;
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
                playerAnimator.SetTrigger("Moved");
                targetDirection = Vector3.back;
                TargetDestination = Player.position + new Vector3(0, 0, -playerMoveDistance);
            }
            else
            {
                // Left swipe
                playerAnimator.SetTrigger("Moved");
                targetDirection = Vector3.forward;
                TargetDestination = Player.position + new Vector3(0, 0, playerMoveDistance);
            }
        }
        else
        {
            // Vertical swipe
            if (swipeDirection.y > 0)
            {
                // Up swipe
                playerAnimator.SetTrigger("Moved");
                targetDirection = Vector3.right;
                TargetDestination = Player.position + new Vector3(playerMoveDistance, 0, 0);
            }
            else
            {
                // Down swipe
                playerAnimator.SetTrigger("Moved");
                targetDirection = Vector3.left;
                TargetDestination = Player.position + new Vector3(-playerMoveDistance, 0, 0);
            }
        }
        CurrentState = GameState.Moving;
    }

    private void FixedUpdate()
    {
        if (CurrentState != GameState.Moving) return;

        Vector3 currentDirection = Player.forward;
        currentDirection.y = 0; // Keep the Y component constant to avoid tilting

        Vector3 newDirection = Vector3.RotateTowards(currentDirection, targetDirection, rotationSpeed * Time.fixedDeltaTime, 0f);
        newDirection.y = 0; // Ensure the new direction is also on the XZ plane

        // Convert the direction vector to a quaternion
        Quaternion newRotation = Quaternion.LookRotation(newDirection);

        // Apply the new rotation
        Player.rotation = newRotation;

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


    private void CreatePlatform(Vector3 position)
    {
        float platformHeight = StartPlatform.position.y;
        position.y = platformHeight; // Ensure the height is consistent

        CurrentPlatform = Instantiate(Platform, position, Quaternion.identity);

        // Decide whether to spawn a power-up
        if (Random.value < powerUpSpawnChance)
        {
            Vector3 powerUpPosition = CurrentPlatform.position;
            float powerUpVerticalOffset = 0.5f; // Manually set the vertical offset
            powerUpPosition.y += powerUpVerticalOffset; // Position it above the platform
            Instantiate(powerUpPrefab, powerUpPosition, Quaternion.identity, CurrentPlatform);
        }

        // Handle the previous platform
        if (!isFirstPlatform && previousPlatform != null)
        {
            Destroy(previousPlatform.gameObject, 0.4f); // Delay destruction
        }
        previousPlatform = CurrentPlatform;
        isFirstPlatform = false;
    }


    [SerializeField] private GameObject powerUpPrefab; // Assign this in the inspector
    private float powerUpSpawnChance = 0.01f; //  3% chance of spawning a power-up

    private IEnumerator SpawnPlatform()
    {
        while (CurrentState != GameState.GameOver)
        {
            yield return new WaitForSeconds(isSlowMotionActive ? slowMotionSpawnDelay : normalSpawnDelay);

            bool validPositionFound = false;
            Vector3 previewPosition = Vector3.zero;
            int attempts = 0;
            const int maxAttempts = 10;

            while (!validPositionFound && attempts < maxAttempts)
            {
                int direction = Random.Range(0, 4);
                previewPosition = CalculateNextPlatformPosition(CurrentPlatform, direction);

                if (IsPositionWithinBoundary(previewPosition))
                {
                    validPositionFound = true;
                }
                attempts++;
            }

            if (!validPositionFound)
            {
                Debug.LogError("Failed to find a valid platform position within the boundary after multiple attempts.");
                yield break;
            }

            Transform preview = Instantiate(PlatformPreviewPrefab, previewPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
            Destroy(preview.gameObject);
            CreatePlatform(previewPosition);
        }
    }

    private bool IsPositionWithinBoundary(Vector3 position)
    {
        const float boundary = 2f; // Half the size of the 5x5 area
        return position.x >= -boundary && position.x <= boundary &&
               position.z >= -boundary && position.z <= boundary;
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

    [SerializeField] private ParticleSystem playerFellParticles;
    //private bool hasFallen = false;

    public Animator playerAnimator;

    public IEnumerator PlayerDeathCoroutine()
    {
        // Play animation
        playerAnimator.SetTrigger("Death");

        yield return new WaitForSeconds(1f);

        // hasFallen = true;
        // // Play the particle effect
        // if (hasFallen)
        // {
        //     playerFellParticles.Play();
        //     //Invoke("StopPlayerFellParticles", 1f); // Stop after 1 second
        // }

        // Wait for 1 second before proceeding
        //Invoke("GameOver", 1f);
       

        CurrentState = GameState.GameOver;
        gamePanel.SetActive(false);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Show the final score
        ScoreManager.Instance.ShowFinalScore();
        ScoreManager.Instance.ShowHighScore();
    }

    public void PlayerFell()
    {
        
        StartCoroutine(PlayerDeathCoroutine());
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
         playerAnimator.SetTrigger("Reset");
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
        Player.position = new Vector3(0, 0, 0);
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
        StartCoroutine(DeactivateAfterDelay(StartPlatform.gameObject, 2f)); // Set the delay as needed
        StopCoroutine(SpawnPlatform());
        StartCoroutine(SpawnPlatform());

    }

}