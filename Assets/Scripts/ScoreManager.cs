using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private int currentScore = -1;
    private int highScore = 0;

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

        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void IncrementScore()
    {
        currentScore++;
        UpdateScoreDisplay();

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    public void ResetScore()
    {
        currentScore = -1;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }

        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }

    public void DisplayGameOverScores()
    {
        // Additional logic if needed when game is over, like displaying final scores.
        UpdateScoreDisplay();
    }
}
