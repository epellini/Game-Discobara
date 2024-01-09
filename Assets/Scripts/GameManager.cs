using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        ReadyForInput,
        Moving,
        GameOver
    }

    [SerializeField]
    private Transform Player;
    private Vector3 TargetDestination = Vector3.zero;
    private GameState CurrentState;
    public Transform Platform;
    public Transform StartPlatform;
    public Transform CurrentPlatform = null;
    private Transform previousPlatform = null;
    private float playerMoveDistance = 1f;
    private bool isFirstPlatform = true;
    public Transform PlatformPreviewPrefab;
    public GameObject gameOverPanel;


    void Start()
    {
        CurrentPlatform = StartPlatform;
        StartCoroutine(SpawnPlatform());
    }

    void Update()
    {
        // Handle Game Over state
        if (CurrentState == GameState.GameOver)
        {

            return; // Skip the rest of the update if the game is over
        }

        if (CurrentState == GameState.ReadyForInput)
        {

            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("A pressed");
                TargetDestination = Player.position + new Vector3(0, 0, playerMoveDistance);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("S pressed");
                TargetDestination = Player.position + new Vector3(-playerMoveDistance, 0, 0);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("D pressed");
                TargetDestination = Player.position + new Vector3(0, 0, -playerMoveDistance);
                CurrentState = GameState.Moving;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("W pressed");
                TargetDestination = Player.position + new Vector3(playerMoveDistance, 0, 0);
                CurrentState = GameState.Moving;
            }

            // Swipe detection for mobile devices
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Begin touch
                if (touch.phase == TouchPhase.Began)
                {
                    touchStart = touch.position;
                    isSwiping = true;
                }

                // End touch
                else if (touch.phase == TouchPhase.Ended && isSwiping)
                {
                    touchEnd = touch.position;
                    isSwiping = false;
                    HandleSwipe();
                }
            }

        }

    }

    private void FixedUpdate()
    {
        if (CurrentState != GameState.Moving) return;

        if (CurrentState == GameState.Moving)
        {
            Player.position = Vector3.MoveTowards(Player.position, new Vector3(TargetDestination.x, Player.position.y, TargetDestination.z), 0.2f);
            if (Vector2.Distance(new Vector2(Player.position.x, Player.position.z), new Vector2(TargetDestination.x, TargetDestination.z)) < 0.1f)
            {
                Player.position = TargetDestination;
                CurrentState = GameState.ReadyForInput;
            }
        }
    }



    private void CreatePlatform(Transform _Platform, int _InValue)
    {
        //Assuming all platforms should be at the same height as the start platform
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
        } // previousPlatform = _Platform;

        // If the previous platform is the start platform and this is the first spawn,
        // don't destroy it immediately. Otherwise, schedule the destruction.
        if (!isFirstPlatform && previousPlatform != null)
        {
            Destroy(previousPlatform.gameObject, 0.5f); // Delay destruction
        }

        // Update the previousPlatform to the current platform
        previousPlatform = CurrentPlatform;
        //  previousPlatform = CurrentPlatform;


        // After the first platform is created, set isFirstPlatform to false and schedule destruction of the start platform
        if (isFirstPlatform)
        {
            isFirstPlatform = false;
            if (StartPlatform != null)
            {
                StartCoroutine(DeactivateAfterDelay(StartPlatform.gameObject, 1f));
            }
        }

    }


    private IEnumerator SpawnPlatform()
    {
        while (CurrentState != GameState.GameOver)
        {
            yield return new WaitForSeconds(0.5f); // Time before showing the preview

            int newValue = Random.Range(0, 4);

            // Calculate the position for the preview
            Vector3 previewPosition = CalculateNextPlatformPosition(CurrentPlatform, newValue);
            Transform preview = Instantiate(PlatformPreviewPrefab, previewPosition, Quaternion.identity);

            yield return new WaitForSeconds(1f); // Time for the player to see the preview

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

    private Vector2 touchStart;
    private Vector2 touchEnd;
    private bool isSwiping = false;
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

    public void PlayerFell()
    {
        Debug.Log("Game Over - Player fell");
        CurrentState = GameState.GameOver;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        // Here you can add more game over logic or UI updates

    }

}