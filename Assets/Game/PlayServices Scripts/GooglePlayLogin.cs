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
    [SerializeField] private TextMeshProUGUI leaderboardText;

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

    public void EnsureAuthentication()
    {
        if (!Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate(OnSignInResult);

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
        EnsureAuthentication();
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

    // LEADERBOARD
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

    // Load Google Leaderboard UI
    public void ShowLeaderboard()
    {
        EnsureAuthentication();
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
    }

    // Load INGAME Leaderboard UI
    public void LoadLeaderboardScores()
    {EnsureAuthentication();
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.LoadScores(
                GPGSIds.leaderboard_Leaders,
                LeaderboardStart.TopScores,
                10, // Number of scores to load
                LeaderboardCollection.Public,
                LeaderboardTimeSpan.AllTime,
                (data) =>
                {
                    if (data.Valid)
                    {
                        // Prepare to fetch user details
                        List<string> userIds = new List<string>();
                        foreach (IScore score in data.Scores)
                        {
                            userIds.Add(score.userID);
                        }

                        // Fetch user details
                        Social.LoadUsers(userIds.ToArray(), (users) =>
                        {

                            string leaderboardOutput = "Leaderboard Scores:\n";
                            foreach (IScore score in data.Scores)
                            {
                                IUserProfile user = FindUser(users, score.userID);
                                string userName = (user != null) ? user.userName : "Anonymous";
                                leaderboardOutput += $"{score.rank}. {userName}: {score.formattedValue}\n";
                            }
                            UpdateLeaderboardText(leaderboardOutput);
                        });
                    }
                    else
                    {
                        UpdateLeaderboardText("Failed to load scores");
                    }
                }
            );
        }
        else
        {
            UpdateLeaderboardText("Not logged in. Cannot load scores.");
        }
    }

    // Helper method to find a user profile by ID
    private IUserProfile FindUser(IUserProfile[] users, string userId)
    {
        foreach (IUserProfile user in users)
        {
            if (user.id == userId)
            {
                return user;
            }
        }
        return null;
    }

    private void UpdateLeaderboardText(string message)
    {
        if (leaderboardText != null)
            leaderboardText.text = message;
    }


}