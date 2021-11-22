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

    [SerializeField] private Text _messageInfo;

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
                _messageInfo.text = $"Could not resolve all Firebase dependencies: + {_dependencyStatus}";
            }
        });
    }

    private void InitFacebook()
    {
        _auth = FirebaseAuth.DefaultInstance;

        if(!FB.IsInitialized)
        {
            FB.Init();
        }
    }
    
    private void AuthStatusCallback(ILoginResult result)
    {
        if (result.Error != null)
        {
            _messageInfo.text = result.Error;

            Debug.Log(result.Error);
            return;
        }

        if (FB.IsLoggedIn)
        {
            LoginViaFirebaseFacebook();
        }
        else
        {
            _messageInfo.text  = "User cancelled login";
            Debug.Log("User cancelled login");
        }
    }

    private void LoginViaFirebaseFacebook()
    {
        _messageInfo.text  = "Login Facebook...Please wait";

        var accessToken = AccessToken.CurrentAccessToken;
        Credential credential = FacebookAuthProvider.GetCredential(accessToken.TokenString);

        _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                _messageInfo.text  = "SignInWithCredentialAsync was canceled";

                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                var firebaseException = task.Exception.GetBaseException() as FirebaseException;
                var errorCode = (AuthError)firebaseException.ErrorCode;

                if (errorCode == AuthError.AccountExistsWithDifferentCredentials)
                {
                    Debug.Log("Email already in use. May be you already register this Email?");
                    UnityMainThread.wkr.AddJob(() => {_messageInfo.text  = "Email already in use. May be you already register this Email?";});
                }
                
                Debug.LogError("task is Faulted");
            }

            FirebaseUser newUser = task.Result;
            // UnityMainThread.wkr.AddJob(UIManager.Instance.ShowMainMenuScreen);

            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }

    public void LoginButtonForFB()
    {
        FB.LogInWithReadPermissions(null, AuthStatusCallback);
    }
}