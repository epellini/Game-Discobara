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
    [SerializeField] private GameObject tutorialPanel;
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
    private float movementSpeed = 2.5f; // Adjust this speed in the Unity Inspector
    private Vector3 velocity = Vector3.zero; // For SmoothDamp
    private const float boundary = 2.5f; // Half the size of the 5x5 area
    private const float boundaryOffset = 0.1f; // Offset to prevent player from getting too close to the boundary
    [SerializeField] private float rotationSpeed = 100f;
    private Vector3 targetDirection;
    [SerializeField] private string[] movedAnimations;
    [SerializeField] private ParticleSystem playerFellParticles;

    public Animator playerAnimator;

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
        SoundManager.Instance.ButtonPress();
        if (!gameStartedPreviously)
        {
            // First time starting the game
            Debug.Log("Game started - FIRST time");
            menuPanel.SetActive(false);
            gamePanel.SetActive(true);
            gameOverPanel.SetActive(false);
            CurrentState = GameState.ReadyForInput;
             SoundManager.Instance.RestartMusic();
            CurrentPlatform = StartPlatform;

            // Schedule the removal of StartPlatform after a few seconds
            StartCoroutine(DeactivateStartPlatformAfterDelay(StartPlatform.gameObject, 2f)); // Set the delay as needed

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

      private GameState previousState = GameState.Menu;

    void Update()
    {
        // Check for state transition to start music
        if ((previousState == GameState.Menu || previousState == GameState.GameOver) && CurrentState == GameState.ReadyForInput)
        {
            SoundManager.Instance.RestartMusic();
        }

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
                int randomIndex = Random.Range(0, movedAnimations.Length);
                string selectedTrigger = movedAnimations[randomIndex];
                playerAnimator.SetTrigger(selectedTrigger);
                targetDirection = Vector3.forward;
                TargetDestination = Player.position + new Vector3(0, 0, playerMoveDistance);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                int randomIndex = Random.Range(0, movedAnimations.Length);
                string selectedTrigger = movedAnimations[randomIndex];
                playerAnimator.SetTrigger(selectedTrigger);
                targetDirection = Vector3.left;
                //targetDirection = Vector3.down;
                TargetDestination = Player.position + new Vector3(-playerMoveDistance, 0, 0);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                int randomIndex = Random.Range(0, movedAnimations.Length);
                string selectedTrigger = movedAnimations[randomIndex];
                playerAnimator.SetTrigger(selectedTrigger);
                //targetDirection = Vector3.right;
                targetDirection = Vector3.back;
                TargetDestination = Player.position + new Vector3(0, 0, -playerMoveDistance);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                int randomIndex = Random.Range(0, movedAnimations.Length);
                string selectedTrigger = movedAnimations[randomIndex];
                playerAnimator.SetTrigger(selectedTrigger);
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
        // Update the previous state at the end of the Update method
        previousState = CurrentState;
    }

    private void HandleSwipe()
    {
        Vector2 swipeDirection = touchEnd - touchStart;
        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            // Horizontal swipe
            if (swipeDirection.x > 0)
            {
                // Right swipe
                int randomIndex = Random.Range(0, movedAnimations.Length);
                string selectedTrigger = movedAnimations[randomIndex];
                playerAnimator.SetTrigger(selectedTrigger);
                targetDirection = Vector3.back;
                TargetDestination = Player.position + new Vector3(0, 0, -playerMoveDistance);
            }
            else
            {
                // Left swipe
                int randomIndex = Random.Range(0, movedAnimations.Length);
                string selectedTrigger = movedAnimations[randomIndex];
                playerAnimator.SetTrigger(selectedTrigger);
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
                int randomIndex = Random.Range(0, movedAnimations.Length);
                string selectedTrigger = movedAnimations[randomIndex];
                playerAnimator.SetTrigger(selectedTrigger);
                targetDirection = Vector3.right;
                TargetDestination = Player.position + new Vector3(playerMoveDistance, 0, 0);
            }
            else
            {
                // Down swipe
                int randomIndex = Random.Range(0, movedAnimations.Length);
                string selectedTrigger = movedAnimations[randomIndex];
                playerAnimator.SetTrigger(selectedTrigger);
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

        // Calculate the next position before moving
        Vector3 nextPosition = Vector3.SmoothDamp(Player.position, new Vector3(TargetDestination.x, Player.position.y, TargetDestination.z), ref velocity, movementSpeed * Time.fixedDeltaTime);

        // Check if the next position is within the boundary
        if (IsPositionWithinBoundary(nextPosition))
        {
            // Smoothly move the player towards the target destination
            Player.position = nextPosition;

            // Check if the player has reached close to the target destination
            if (Vector3.Distance(Player.position, TargetDestination) < 0.1f)
            {
                Player.position = TargetDestination;
                CurrentState = GameState.ReadyForInput;
            }
        }
        else
        {
            // Player is trying to move out of the boundary, so prevent it
            // You can add some feedback or handle this situation as needed
        }
    }


    private void CreatePlatform(Vector3 position)
    {
        float platformHeight = StartPlatform.position.y;
        position.y = platformHeight; // Ensure the height is consistent

        CurrentPlatform = Instantiate(Platform, position, Quaternion.identity);

        // // Decide whether to spawn a SLOW MOTION power-up
        // if (Random.value < PowerUps.Instance.slowMotionSpawnChance && !PowerUps.Instance.isSlowMotionActive)
        // {
        //     Vector3 powerUpPosition = CurrentPlatform.position;
        //     float powerUpVerticalOffset = 0.5f; // Manually set the vertical offset
        //     powerUpPosition.y += powerUpVerticalOffset; // Position it above the platform
        //     Instantiate(PowerUps.Instance.powerUpSlowMotionPrefab, powerUpPosition, Quaternion.identity, CurrentPlatform);
        // }

        // Decide whether to spawn an extra points power-up
        if (Random.value < PowerUps.Instance.extraPointsSpawnChance && !PowerUps.Instance.isExtraPointsActive)
        {
            Vector3 powerUpPosition = CurrentPlatform.position;
            float powerUpVerticalOffset = 0.5f; // Manually set the vertical offset
            powerUpPosition.y += powerUpVerticalOffset; // Position it above the platform
            Instantiate(PowerUps.Instance.powerUpExtraPointsPrefab, powerUpPosition, Quaternion.identity, CurrentPlatform);
        }

        // Handle the previous platform
        if (!isFirstPlatform && previousPlatform != null)
        {
            Destroy(previousPlatform.gameObject, 0.5f); // Delay destruction
        }
        previousPlatform = CurrentPlatform;
        isFirstPlatform = false;
    }

    private IEnumerator SpawnPlatform()
    {
        
        while (CurrentState != GameState.GameOver)
        {
            yield return new WaitForSeconds(PowerUps.Instance.isSlowMotionActive ? PowerUps.Instance.slowMotionSpawnDelay : PowerUps.Instance.normalSpawnDelay);

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
        return position.x >= -boundary + boundaryOffset && position.x <= boundary - boundaryOffset &&
               position.z >= -boundary + boundaryOffset && position.z <= boundary - boundaryOffset;
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

    private IEnumerator DeactivateStartPlatformAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false); // Turn off the StartPlatform
    }
    public CameraBounceZoom cameraBounceZoom;

    public void PlayerLost() { StartCoroutine(PlayerLostCoroutine()); }
    private IEnumerator PlayerLostCoroutine()
    {
        // Play sound effect
        //AudioManager.Instance.PlaySoundEffect(AudioManager.SoundEffect.PlayerFell);
        // Play animation
        SoundManager.Instance.PlayerDied(); // Plays the death sound effect
        StartCoroutine(cameraBounceZoom.DeathShake(0.3f, 0.03f));
        playerAnimator.SetTrigger("Lost");

        yield return new WaitForSeconds(0.95f); // The amount of time to wait after player lost animation
                                                // 3. Platform Handling
        if (CurrentPlatform != null && CurrentPlatform != StartPlatform)
        {
            Destroy(CurrentPlatform.gameObject);
        }
        if (previousPlatform != null && previousPlatform != StartPlatform)
        {
            Destroy(previousPlatform.gameObject);
        }

        CurrentState = GameState.GameOver; // Prevent the player from moving after losing
        gamePanel.SetActive(false); // Turn off the game panel
        if (gameOverPanel != null) // Turn on the game over panel
        {
            gameOverPanel.SetActive(true);
        }
        // Show the final score
        ScoreManager.Instance.ShowFinalScore();
        ScoreManager.Instance.ShowHighScore();
    }

    public void GoToMenu() // Menu Button on Game Over Screen
    {
        SoundManager.Instance.ButtonPress();
        CurrentState = GameState.Menu;
        if (gameOverPanel != null) { gameOverPanel.SetActive(false); }  // Reset the game over panel
        if (menuPanel != null) { menuPanel.SetActive(true); } // Reset the menu panel
    }

    public void ResetGame() // Restart Button on Game Over Screen
    {
        SoundManager.Instance.ButtonPress();
        SoundManager.Instance.RestartMusic();
        //cameraBounceZoom.OnPlayerDeath();
        transitionController.PlayTransition();
        StartCoroutine(ResetGameCoroutine());
    }

    private IEnumerator ResetGameCoroutine()
    {

        //transitionController.PlayTransition();
        // Reset Camera Bounce and Zoom Effects
        // Reset the animation controller
        CameraBounceZoom cameraBounceZoom = Camera.main.GetComponent<CameraBounceZoom>();
        if (cameraBounceZoom != null)
        {
            cameraBounceZoom.ResetEffects();
        }
        //playerAnimator.SetTrigger("Reset");

        yield return new WaitForSeconds(0.8f);
        playerAnimator.Rebind();
        cameraBounceZoom.OnPlayerDeath();
        // After resetting other game-related components, you can resume the default animation state
        playerAnimator.SetTrigger("Idle");

        // Ensure the StartPlatform is active
        if (StartPlatform != null)
        {
            StartPlatform.gameObject.SetActive(true);
            StartPlatform.position = new Vector3(0, 0, 0);
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

        CurrentPlatform = StartPlatform;
        previousPlatform = null;
        isFirstPlatform = true;
        // 6. Score and High Score Reset (if applicable)
        ScoreManager.Instance.ResetScore();

        // 4. UI Elements
        if (gameOverPanel != null) { gameOverPanel.SetActive(false); }
        gamePanel.SetActive(true);

        // 5. Reset Touch Input and Swipe Handling
        touchStart = Vector2.zero;
        touchEnd = Vector2.zero;
        isSwiping = false;

        // ScoreManager.Instance.ResetHighScore(); // If you have a high score system
        StartCoroutine(DeactivateStartPlatformAfterDelay(StartPlatform.gameObject, 2f)); // Set the delay as needed
        StopCoroutine(SpawnPlatform());
        StartCoroutine(SpawnPlatform());
    }


    public void ShowTutorial()
    {
        SoundManager.Instance.ButtonPress();
        // Show the tutorial panel
        tutorialPanel.SetActive(true);
        // Hide the menu panel
        menuPanel.SetActive(false);
    }

    public void HideTutorial()
    {
        SoundManager.Instance.ButtonPress();
        // Hide the tutorial panel
        tutorialPanel.SetActive(false);
        // Show the menu panel
        menuPanel.SetActive(true);
    }
}