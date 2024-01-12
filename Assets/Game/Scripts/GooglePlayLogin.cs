using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using TMPro;
using SmokeTest;

public class GooglePlayLogin : MonoBehaviour
{
    public static GooglePlayLogin Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI statusText;
    private bool mStandby = false;
    private string mStandbyMessage = string.Empty;
    private string _mStatus = "Ready";

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
    }

    // Property for handling the status message
    private string Status
    {
        get { return _mStatus; }
        set
        {
            _mStatus = value;
            UpdateStatusText(_mStatus); // Update the UI when status changes
        }
    }

    public void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(OnSignInResult);
    }

    private void OnSignInResult(SignInStatus signInStatus)
    {
        EndStandBy();
        if (signInStatus == SignInStatus.Success)
        {
            Status = "Authenticated. Hello, " + Social.localUser.userName + " (" + Social.localUser.id + ")";
        }
        else
        {
            Status = "*** Failed to authenticate with " + signInStatus;
        }
        //ShowEffect(signInStatus == SignInStatus.Success);
    }

    internal void SetStandBy(string message)
    {
        mStandby = true;
        mStandbyMessage = message;
    }

    internal void EndStandBy()
    {
        mStandby = false;
    }

    internal void DoAuthenticate()
    {
        SetStandBy("Authenticating...");
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.ManuallyAuthenticate(OnSignInResult);
    }

    private void UpdateStatusText(string statusMessage)
    {
        if (statusText != null)
            statusText.text = statusMessage;
    }

    // LEADERBOARD:
    // Post score to Google Leaderboard
    public void PostScoreToLeaderboard(long score)
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, GPGSIds.leaderboard_Leaders, success =>
            {
                if (success)
                {
                    Debug.LogWarning("Score posted successfully");
                    // Update UI or show a message to the player about the successful score submission
                }
                else
                {
                    Debug.LogWarning("Failed to post score");
                    // Update UI or show a message to the player about the failure
                }
            });
        }
        else
        {
            Debug.Log("Not logged in. Score not posted.");
            // Inform the player they need to log in
        }
    }

//     public void LoadLeaderboardScores()
// {
//     if (Social.localUser.authenticated)
//     {
//         PlayGamesPlatform.Instance.LoadScores(
//             GPGSIds.leaderboard_Leaders,
//             LeaderboardStart.TopScores,
//             10, // Number of scores to load
//             LeaderboardCollection.Public,
//             LeaderboardTimeSpan.AllTime,
//             (data) =>
//             {
//                 if (data.Valid)
//                 {
//                     Debug.Log("Loaded " + data.Scores.Length + " scores");
//                     foreach (IScore score in data.Scores)
//                     {
//                         // Process each score as needed
//                         Debug.Log(score.userID + ": " + score.formattedValue);
//                     }
//                 }
//                 else
//                 {
//                     Debug.Log("Failed to load scores");
//                 }
//             }
//         );
//     }
//     else
//     {
//         Debug.Log("Not logged in. Cannot load scores.");
//     }
// }


    // Load Google Leaderboard UI
    public void ShowLeaderboard()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
    }
}