using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TextMeshProUGUI currentScoreText; // Text that will updated on each move during gameplay - GAMEPLAY SCREEN
    public TextMeshProUGUI finalScoreText; // Text that will show at end of game on GameOver Screen - GAME OVER SCREEN
    public TextMeshProUGUI highScoreText; // Text that will also show at end of game on GameOver Screen - GAME OVER SCREEN
    public TextMeshProUGUI newHighScoreText; // Text that will only show if the player beats the high score - GAME OVER SCREEN 
    private int currentScore = -1; // Start at -1 so that the first platform the player lands on will be 0
    private int finalScore = 0; // Start at 0
    private int highScore = 0; // Start at 0
    private bool newHighScoreAchieved = false;

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
        currentScoreText.text = "Score: " + currentScore;
    }

    public void IncrementScore()
    {
        int scoreIncrement = GameManager.Instance.isExtraPointsActive ? 2 : 1;
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
        currentScoreText.text = "Score: " + currentScore;
    }

    public void ShowFinalScore() // Called from GameManager.cs on GameOver Screen
    {
        finalScoreText.text = "Score: " + currentScore;
    }

    public void ShowHighScore() // Called from GameManager.cs on GameOver Screen
    {
        if (!newHighScoreAchieved)
        {
            highScoreText.text = "High Score: " + highScore;
        }

        if (newHighScoreAchieved)
        {
            ShowNewHighScore();
        }
    }

    public void ShowNewHighScore() // Called from GameManager.cs on GameOver Screen
    {
        if (newHighScoreAchieved)
        {
            highScoreText.gameObject.SetActive(false);
            newHighScoreText.gameObject.SetActive(true);
            newHighScoreText.text = "You've reached a new High Score: " + highScore;
        }
    }

    public void ResetScore()
    {
        currentScore = -1;
    }
}
