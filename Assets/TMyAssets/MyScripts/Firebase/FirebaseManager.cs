using System;
using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;
using System.Threading.Tasks;
using TMPro;

public class FirebaseManager : MonoBehaviour
{
    #region Fields

    [Header("Firebase")] private DependencyStatus _dependencyStatus;
    private FirebaseAuth _auth;
    private FirebaseUser _user;
    private DatabaseReference _dataBaseReference;

    [Header("LoginScreen")]
    [SerializeField]
    private TMP_InputField _emailLoginField;

    [SerializeField] private TMP_InputField _passwordLoginField;
    [SerializeField] private TMP_Text _warningLoginText;
    [SerializeField] private TMP_Text _loginDebugMessage;

    [Header("RegisterScreen")]
    [SerializeField]
    private TMP_InputField _usernameRegisterField;

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
            UIManager.instance.ShowMainMenuScreen();
        }
        else
        {
            UIManager.instance.ShowLoginScreen();
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
                    _warningLoginText.text = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    _warningLoginText.text = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    _warningLoginText.text = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    _warningLoginText.text = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    _warningLoginText.text = "Account does not exist";
                    break;
                default:
                    _warningLoginText.text = "LoginCoroutine Failed!";
                    break;
            }
        }
        else
        {
            _user = loginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", _user.DisplayName, _user.Email);
            _warningLoginText.text = string.Empty;
            _loginDebugMessage.text = "Logged In";

            DownloadUserDataFromFirebase();

            yield return new WaitForSeconds(2);

            UIManager.instance.ShowMainMenuScreen();
            _loginDebugMessage.text = string.Empty;
            ClearLoginFields();
            ClearRegisterFields();
        }
    }

    public void SignOutButton()
    {
        _auth.SignOut();
        UIManager.instance.ShowLoginScreen();
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
                    _warningLoginText.text = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    _warningLoginText.text = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    _warningLoginText.text = "Weak password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    _warningLoginText.text = "Email already in use";
                    break;
                default:
                    _warningLoginText.text = "RegisterCoroutine failed!";
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

        if (profileTask.Exception != null)
        {
            Debug.LogWarning($"Failed to Register with {profileTask.Exception}");
            _warningRegisterText.text = "Failed UpdateUserProfileAsync";
            yield break;
        }

        yield return new WaitForSeconds(2);
        UIManager.instance.ShowLoginScreen();
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
                //TODO Изменить значения после ??
                float exp = Convert.ToSingle(childSnapshot.Child(_dbExpField).Value ?? ExperienceHolder.value);
                Debug.LogWarning("childSnapshot.Child(_dbExpField).Value is null");
                var username = childSnapshot.Child(_dbUsernameField).Value.ToString() ?? "#Error 404";

                GameObject scoreboardElement = Instantiate(_scoreElement, _scoreboardContent);
                scoreboardElement.GetComponent<Leaderboard>().NewScoreElement(username, exp);
            }

            UIManager.instance.ShowLeaderboardScreen();
        }
    }
    
    #endregion

    #region ClearFields & PasteEmailPassword

    private void ClearLoginFields()
    {
        _emailLoginField.text = string.Empty;
        _passwordLoginField.text = string.Empty;
    }

    private void ClearRegisterFields()
    {
        _usernameRegisterField.text = string.Empty;
        _emailRegisterField.text = string.Empty;
        _passwordRegisterField.text = string.Empty;
        _passwordRegisterVerifyField.text = string.Empty;
    }

    public void PasteEmailPassword()
    {
        _emailLoginField.text = "r.salnikov1998@gmail.com";
        _passwordLoginField.text = "12345a";
    }
    
    #endregion

}