using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Unity.Services.Authentication;
using Unity.Services.Core;
//using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class GoogleIntegration : MonoBehaviour
{
    public string GooglePlayToken;
    public string GooglePlayError;

    public async Task Authenticate()
    {
        PlayGamesPlatform.Activate();
        await UnityServices.InitializeAsync();

        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Successfully authenticated with Google Play Games");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Debug.Log($"Auth code is {code}");
                    GooglePlayToken = code;
                });
            }
            else
            {
                GooglePlayError = "Failed to retrive Google Play Games auth code";
                Debug.LogError("Login Unsuccessful");
            }
        });
        await AuthenticateWithUnity();
    }

    private async Task AuthenticateWithUnity()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGoogleAsync(GooglePlayToken);
            Debug.Log("Successfully authenticated with Unity Services");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            throw;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            throw;
        }
    }
}
