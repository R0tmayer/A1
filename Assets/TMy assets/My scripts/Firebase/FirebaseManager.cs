using System;
using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FirebaseManager : MonoBehaviour
{
    #region Fields

    [Header("Firebase")] 
    
    private DependencyStatus _dependencyStatus;
    private FirebaseAuth _auth;
    private FirebaseUser _user;
    private DatabaseReference _dataBaseReference;

    [Header("LoginScreen")]
    
    [SerializeField] private TMP_InputField _emailLoginField;
    [SerializeField] private TMP_InputField _passwordLoginField;
    [SerializeField] private TMP_Text _loginMessage;
    [SerializeField] private TMP_Text _registerMessage;

    [Header("RegisterScreen")]
    
    [SerializeField] private TMP_InputField _usernameRegisterField;
    [SerializeField] private TMP_InputField _emailRegisterField;
    [SerializeField] private TMP_InputField _passwordRegisterField;
    [SerializeField] private TMP_InputField _passwordRegisterVerifyField;
    [SerializeField] private TMP_Text _warningRegisterText;

    [SerializeField] private GameObject _scoreElement;
    [SerializeField] private Transform _scoreboardContent;

    private const string _dbUsersField = "users";
    private const string _dbUsernameField = "username";
    private const string _dbExpField = "exp";

    #endregion

    #region Awake & Start
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            _dependencyStatus = task.Result;

            if (_dependencyStatus == DependencyStatus.Available)
            {
                UnityMainThread.wkr.AddJob(InitializeFirebase);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + _dependencyStatus);
            }
        });
    }

    
    #endregion

    #region InitialFirebase & AuthStateChanged
    
    private void InitializeFirebase()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _dataBaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        _auth.StateChanged += AuthStateChanged;
    }

    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (_auth.CurrentUser != null)
        {
            _user = _auth.CurrentUser;
            Debug.Log(_user.Email);

            DownloadUserDataFromFirebase();
            // UIManager.Instance.ShowMainMenuScreen();
        }
    }
    
    #endregion

    #region LoginUser & SignOut

    public void LoginWithEmailPassword()
    {
        StartCoroutine(LoginCoroutine(_emailLoginField.text, _passwordLoginField.text));
    }

    private IEnumerator LoginCoroutine(string email, string password)
    {
        _loginMessage.text = "Login...Please Wait";
        
        var loginTask = _auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {loginTask.Exception}");
            var firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            var errorCode = (AuthError)firebaseException.ErrorCode;

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    _loginMessage.text = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    _loginMessage.text = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    _loginMessage.text = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    _loginMessage.text = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    _loginMessage.text = "Account does not exist";
                    break;
                default:
                    _loginMessage.text = "LoginCoroutine Failed!";
                    break;
            }
        }
        else
        {
            _user = loginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", _user.DisplayName, _user.Email);
            _loginMessage.text = "Logged In";

            DownloadUserDataFromFirebase();
            // UIManager.Instance.ShowMainMenuScreen();
        }
    }

    public void SignOutButton()
    {
        _auth.SignOut();
        UIManager.Instance.ShowLoginScreen();
        ClearRegisterFields();
        ClearLoginFields();
    }

    #endregion

    #region  RegisterNewUser

    public void RegisterNewUser()
    {
        StartCoroutine(RegisterCoroutine());
    }

    private IEnumerator RegisterCoroutine()
    {
        var email = _emailRegisterField.text;
        var password = _passwordRegisterField.text;
        var verifyPassword = _passwordRegisterVerifyField.text;
        var username = _usernameRegisterField.text;

        if (username == string.Empty)
        {
            _warningRegisterText.text = "Missing Username";
            yield break;
        }

        if (password != verifyPassword)
        {
            _warningRegisterText.text = "Password Does Not Match!";
            yield break;
        }

        var registerTask = _auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {registerTask.Exception}");
            var firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
            var errorCode = (AuthError)firebaseException.ErrorCode;

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    _registerMessage.text = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    _registerMessage.text = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    _registerMessage.text = "Weak password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    _registerMessage.text = "Email already in use";
                    break;
                default:
                    _registerMessage.text = "RegisterCoroutine failed!";
                    break;
            }

            yield break;
        }

        _user = registerTask.Result;

        if (_user == null)
        {
            Debug.LogError("User is Null");
            yield break;
        }

        var profile = new UserProfile { DisplayName = username };
        Task profileTask = _user.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(() => profileTask.IsCompleted);
        _warningRegisterText.text = "Success Registration!";

        if (profileTask.Exception != null)
        {
            Debug.LogWarning($"Failed to Register with {profileTask.Exception}");
            _warningRegisterText.text = "Failed UpdateUserProfileAsync";
            yield break;
        }

        yield return new WaitForSeconds(2);
        UIManager.Instance.ShowLoginScreen();
        _warningRegisterText.text = string.Empty;
        ClearRegisterFields();
        ClearLoginFields();
    }

    #endregion

    #region Download & Update FirebaseData

    private void DownloadUserDataFromFirebase()
    {
        StartCoroutine(DownloadData());
    }
    
    private IEnumerator DownloadData()
    {
        var dataBaseTask = _dataBaseReference.Child(_dbUsersField).Child(_user.UserId).GetValueAsync();
        yield return new WaitUntil(() => dataBaseTask.IsCompleted);
        DataSnapshot snapshot = dataBaseTask.Result;

        if (dataBaseTask.Exception != null)
        {
            Debug.LogWarning($"Failed to Download User Data with {dataBaseTask.Exception}");
        }
        else
        {
            //Compare firebase with player prefs
            ExperienceHolder.value = Convert.ToSingle(snapshot.Child(_dbExpField).Value ?? 0f);
            SaveGameData playerPrefsData = SaveManager.LoadData("save.gamesave");

            if (playerPrefsData == null)
            {
                Debug.LogError("PlayerPrefs is null");
                yield break;
            }

            if (ExperienceHolder.value < playerPrefsData.exp)
            {
                ExperienceHolder.value = playerPrefsData.exp;
            }
        }
    }

    public void UpdateFirebaseUserData()
    {
        StartCoroutine(UpdateUserData());
    }

    private IEnumerator UpdateUserData()
    {
        Task usernameTask = _dataBaseReference.Child(_dbUsersField).Child(_user.UserId).Child(_dbUsernameField).SetValueAsync(_user.DisplayName);
        yield return new WaitUntil(() => usernameTask.IsCompleted);

        if (usernameTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register usernameTask with {usernameTask.Exception}");
        }

        Task expTask = _dataBaseReference.Child(_dbUsersField).Child(_user.UserId).Child(_dbExpField).SetValueAsync(ExperienceHolder.value);
        yield return new WaitUntil(() => expTask.IsCompleted);

        if (expTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register expTask with {expTask.Exception}");
        }
    }

    #endregion

    #region Leaderboard

    public void LeaderboardButton()
    {
        StartCoroutine(LeaderboardCoroutine());
    }

    private IEnumerator LeaderboardCoroutine()
    {
        var dataBaseTask = _dataBaseReference.Child(_dbUsersField).GetValueAsync();
        yield return new WaitUntil(() => dataBaseTask.IsCompleted);

        if (dataBaseTask.Exception != null)
        {
            Debug.LogWarning($"Failed to leaderboard task with {dataBaseTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = dataBaseTask.Result;

            foreach (Transform child in _scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse())
            {
                float exp = Convert.ToSingle(childSnapshot.Child(_dbExpField).Value ?? ExperienceHolder.value);
                var username = childSnapshot.Child(_dbUsernameField).Value.ToString() ?? "#Error 404";

                GameObject scoreboardElement = Instantiate(_scoreElement, _scoreboardContent);

                if (username == _user.DisplayName)
                {
                    scoreboardElement.GetComponent<Leaderboard>().NewScoreElement(username + " (you)", exp);
                }
                else
                {
                    scoreboardElement.GetComponent<Leaderboard>().NewScoreElement(username, exp);
                }

            }

            Addressables.InstantiateAsync("").Completed += handle => Completed(handle, new object());
            
            // UIManager.Instance.ShowLeaderboardScreen();
        }
    }

    private void Completed(AsyncOperationHandle<GameObject> obj, object haha)
    {
        // obj.Result.TryGetComponent()
        throw new NotImplementedException();
    }

    #endregion

    #region ClearFields & PasteEmailPassword

    private void ClearLoginFields()
    {
        _emailLoginField.text = string.Empty;
        _passwordLoginField.text = string.Empty;
        
        _loginMessage.text = string.Empty;
    }

    private void ClearRegisterFields()
    {
        _usernameRegisterField.text = string.Empty;
        _emailRegisterField.text = string.Empty;
        _passwordRegisterField.text = string.Empty;
        _passwordRegisterVerifyField.text = string.Empty;

        _registerMessage.text = string.Empty;
    }

    public void PasteEmailPassword()
    {
        _emailLoginField.text = "amadest98mail.ru@gmail.com";
        _passwordLoginField.text = "12345a";
    }
    
    #endregion

}