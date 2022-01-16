using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class FacebookManager : MonoBehaviour
{
    private FirebaseAuth _auth;
    private DependencyStatus _dependencyStatus;
    private FirebaseUser _user;

    public Text text;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            _dependencyStatus = task.Result;

            if (_dependencyStatus == DependencyStatus.Available)
            {
                UnityMainThread.wkr.AddJob(InitFacebook);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + _dependencyStatus);
                text.text = $"Could not resolve all Firebase dependencies: + {_dependencyStatus}";
            }
        });
    }

    private void InitFacebook()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += AuthStateChanged;


        if(!FB.IsInitialized)
        {
            FB.Init();
        }
    }
    
    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (_auth.CurrentUser != null)
        {
            _user = _auth.CurrentUser;
        }
    }

    private void AuthStatusCallback(ILoginResult result)
    {
        if (result.Error != null)
        {
            text.text = result.Error;

            Debug.Log(result.Error);
            return;
        }

        if (FB.IsLoggedIn)
        {
            text.text  = "LoginViaFirebaseFacebook";

            Debug.Log("LoginViaFirebaseFacebook");
            LoginViaFirebaseFacebook();
        }
        else
        {
            text.text  = "User cancelled login";
            Debug.Log("User cancelled login");
        }
    }

    private void LoginViaFirebaseFacebook()
    {
        LogOut();
        var accessToken = AccessToken.CurrentAccessToken;
        Credential credential = FacebookAuthProvider.GetCredential(accessToken.TokenString);

        _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                text.text  = "SignInWithCredentialAsync was canceled";

                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                text.text  = "SignInWithCredentialAsync encountered an error:" + task.Exception;
                return;
            }

            FirebaseUser newUser = task.Result;
            UnityMainThread.wkr.AddJob(UIManager.instance.ShowMainMenuScreen);

            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

            text.text  = "Success";
        });
    }

    public void LoginButtonForFB()
    {
        var permissions = new List<string>() {};
        FB.LogInWithReadPermissions(null, AuthStatusCallback);
    }

    private void LogOut()
    {
        _auth.SignOut();
    }
}