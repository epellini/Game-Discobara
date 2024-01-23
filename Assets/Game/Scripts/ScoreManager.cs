using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public GameManager gameManager; // Reference to GameManager.cs
    public TextMeshProUGUI currentScoreText; // Text that will updated on each move during gameplay - GAMEPLAY SCREEN
    public TextMeshProUGUI finalScoreText; // Text that will show at end of game on GameOver Screen - GAME OVER SCREEN
    public TextMeshProUGUI highScoreText; // Text that will also show at end of game on GameOver Screen - GAME OVER SCREEN
    public TextMeshProUGUI newHighScoreText; // Text that will only show if the player beats the high score - GAME OVER SCREEN 
    public PlayerController playerController; // Reference to PlayerController.cs
    private int currentScore = 0; // Start at -1 so that the first platform the player lands on will be 0
    private int finalScore = 0; // Start at 0
    private int highScore = 0; // Start at 0
    private bool newHighScoreAchieved = false;
    public GameObject FloatingTextPrefab; // Floating text prefab for points added animation
    public GameObject particleEffectPrefab;
    public GameObject gameOverCard; // Assign this in the inspector
    public GameObject newHighScoreCard;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        currentScoreText.text = currentScore.ToString();
    }

    public void IncrementScore()
    {
        int scoreIncrement = PowerUps.Instance.isExtraPointsActive ? 2 : 1;
        currentScore += scoreIncrement; // Increase score by 2 if extra points are active, otherwise by 1
        ShowCurrentScore();

        // Save High Score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            newHighScoreAchieved = true;

            GooglePlayLogin.Instance?.PostScoreToLeaderboard(highScore);
        }
    }

    // Methods for displaying score on UI
    public void ShowCurrentScore() // Called from PlayerController.cs
    {
        currentScoreText.text = currentScore.ToString();
        // Call points added animation here
        if (FloatingTextPrefab)
        {
            ShowFloatingText();
        }
    }

    public void ShowFinalScore() // Called from GameManager.cs on GameOver Screen
    {
        finalScoreText.text = currentScore.ToString();
        ShowEndGameCard();
    }

    private void ShowEndGameCard()
    {
        if (newHighScoreAchieved)
        {
            newHighScoreCard.SetActive(true);
            gameOverCard.SetActive(false);
        }
        else
        {
            gameOverCard.SetActive(true);
            newHighScoreCard.SetActive(false);
        }
    }

    public void ShowHighScore() // Called from GameManager.cs on GameOver Screen
    {
        if (newHighScoreAchieved)
        {
            ShowNewHighScore();
        }
        else
        {
            highScoreText.text = "High Score: " + highScore;
            newHighScoreText.gameObject.SetActive(false);
        }
    }

    public void ShowNewHighScore() // Called from GameManager.cs on GameOver Screen
    {
        if (newHighScoreAchieved)
        {
            TriggerHighScoreParticleEffect();
            highScoreText.gameObject.SetActive(false);
            newHighScoreText.gameObject.SetActive(true);
            newHighScoreText.text = highScore.ToString();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        ShowCurrentScore();
        newHighScoreAchieved = false;
        gameOverCard.SetActive(false);
        newHighScoreCard.SetActive(false);
    }

    void ShowFloatingText()
    {
        // Do not show floating text for the first point
        if (currentScore == 0)
        {
            return;
        }
        if (FloatingTextPrefab && playerController)
        {
            Vector3 playerPosition = playerController.transform.position;
            Debug.Log("Player position: " + playerPosition);

            // Define the desired rotation
            Quaternion desiredRotation = Quaternion.Euler(0, 90, 0);

            Vector3 textPosition = playerPosition + Vector3.up * 0.05f;
            GameObject go = Instantiate(FloatingTextPrefab, textPosition, desiredRotation);

            Debug.Log("Text instantiated at position: " + textPosition);

            TextMeshPro textMesh = go.GetComponent<TextMeshPro>();
            if (textMesh)
            {
                textMesh.text = "+" + (PowerUps.Instance.isExtraPointsActive ? 2 : 1);
            }
            else
            {
                Debug.LogError("TextMeshPro component not found on FloatingTextPrefab.");
            }
        }
        else
        {
            Debug.LogError("FloatingTextPrefab is not assigned in the ScoreManager.");
        }
    }


    private void TriggerHighScoreParticleEffect()
    {
        if (particleEffectPrefab)
        {
            // Choose a suitable position for the particle effect, e.g., above the high score text
            Vector3 effectPosition = newHighScoreText.transform.position + Vector3.up * 0.5f; // Adjust as needed

            // Instantiate the particle effect
            GameObject effectInstance = Instantiate(particleEffectPrefab, effectPosition, Quaternion.identity);

            // Optional: Destroy the effect after a certain duration, if it doesn't self-terminate
            Destroy(effectInstance, 3f); // Adjust duration as needed
        }
        else
        {
            Debug.LogError("Particle effect prefab is not assigned in the ScoreManager.");
        }
    }

}