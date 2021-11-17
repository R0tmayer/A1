using System;
using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Unity;
using TMPro;

public class FirebaseManager : MonoBehaviour
{
    [Header("Firebase")]

    private DependencyStatus _dependencyStatus;
    private FirebaseAuth _auth;
    private FirebaseUser _user;
    private DatabaseReference _dataBaseReference;

    [Header("LoginScreen")]

    [SerializeField] private TMP_InputField _emailLoginField;
    [SerializeField] private TMP_InputField _passwordLoginField;
    [SerializeField] private TMP_Text _warningLoginText;
    [SerializeField] private TMP_Text _confirmLoginText;

    [Header("RegisterScreen")]

    [SerializeField] private TMP_InputField _usernameRegisterField;
    [SerializeField] private TMP_InputField _emailRegisterField;
    [SerializeField] private TMP_InputField _passwordRegisterField;
    [SerializeField] private TMP_InputField _passwordRegisterVerifyField;
    [SerializeField] private TMP_Text _warningRegisterText;

    [Header("UserDataScreen")]

    [SerializeField] private TMP_InputField _usernameField;
    [SerializeField] private TMP_InputField _expField;
    [SerializeField] private GameObject _scoreElement;
    [SerializeField] private Transform _scoreboardContent;

    private ExperienceHolder _expHolder;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _expHolder = FindObjectOfType<ExperienceHolder>();

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

            try
            {
                StartCoroutine(LoadUserData());
                UIManager.instance.ShowMainMenuScreen();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }
        else
        {
            UIManager.instance.ShowLoginScreen();
        }
    }

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

    public void LoginButton()
    {
        StartCoroutine(Login(_emailLoginField.text, _passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(_emailRegisterField.text, _passwordRegisterField.text, _usernameRegisterField.text));
    }

    public void SignOutButton()
    {
        _auth.SignOut();
        UIManager.instance.ShowLoginScreen();
        ClearRegisterFields();
        ClearLoginFields();
    }

    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(_usernameField.text));

        var username = _usernameField.text;
        var exp = _expHolder.value;

        StartCoroutine(UpdateUserData(/*username,*/ exp));
    }

    private IEnumerator UpdateUsernameAuth(string username)
    {
        var profile = new UserProfile { DisplayName = username };

        Task ProfileTask = _user.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(() => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {ProfileTask.Exception}");
        }
    }

    private IEnumerator UpdateUserData(/*string username,*/ float exp)
    {
        //Task usernameTask = _dataBaseReference.Child("users").Child(_user.UserId).Child("username").SetValueAsync(username);
        //yield return new WaitUntil(() => usernameTask.IsCompleted);

        //if (usernameTask.Exception != null)
        //{
        //    Debug.LogWarning($"Failed to register usernameTask with {usernameTask.Exception}");
        //}

        Task expTask = _dataBaseReference.Child("users").Child(_user.UserId).Child("exp").SetValueAsync(exp);
        yield return new WaitUntil(() => expTask.IsCompleted);

        if (expTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register expTask with {expTask.Exception}");
        }
    }

    private IEnumerator LoadUserData()
    {
        var DBTask = _dataBaseReference.Child("users").Child(_user.UserId).GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);
        DataSnapshot snapshot = DBTask.Result;

        //Database 
        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            _usernameField.text = _user.DisplayName;
            _expField.text = "0";
        }
        else
        {
            _usernameField.text = (string)snapshot.Child("username").Value;
            _expHolder.value =  Convert.ToSingle(snapshot.Child("exp").Value);
            _expField.text = _expHolder.value.ToString();
        }

        //Player prefs
        SaveGameData data = SaveManager.LoadData("save.gamesave");
        var expFromDatabase = Convert.ToSingle(snapshot.Child("exp").Value);

        if (data == null)
        {
            yield break;
        }
        else if (expFromDatabase < data.exp)
        {
            _expHolder.value = data.exp;
            _expField.text = _expHolder.value.ToString();
        }
    }

    public void LeaderboardButton()
    {
        StartCoroutine(LoadLeaderboardData());
    }

    private IEnumerator Login(string email, string password)
    {
        var loginTask = _auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {loginTask.Exception}");
            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseException.ErrorCode;

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
                    _warningLoginText.text = "Login Failed!";
                    break;
            }
        }
        else
        {
            _user = loginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", _user.DisplayName, _user.Email);
            _warningLoginText.text = string.Empty;
            _confirmLoginText.text = "Logged In";
            StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(2);

            UIManager.instance.ShowMainMenuScreen();
            _confirmLoginText.text = "";
            ClearLoginFields();
            ClearRegisterFields();
        }
    }

    private IEnumerator Register(string email, string password, string username)
    {
        if (username == string.Empty)
        {
            _warningRegisterText.text = "Missing Username";
        }
        else if (_passwordRegisterField.text != _passwordRegisterVerifyField.text)
        {
            _warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            var registerTask = _auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {registerTask.Exception}");
                FirebaseException firebaseEx = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

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
                        _warningLoginText.text = "Register failed!";
                        break;
                }
            }
            else
            {
                _user = registerTask.Result;

                if (_user == null)
                {
                    yield break;
                }

                var profile = new UserProfile { DisplayName = username };

                Task profileTask = _user.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(() => profileTask.IsCompleted);

                if (profileTask.Exception != null)
                {
                    Debug.LogWarning($"Failed to register task with {profileTask.Exception}");
                    _warningRegisterText.text = "Username Set Failed!";
                }
                else
                {
                    UIManager.instance.ShowLoginScreen();
                    _warningRegisterText.text = "";
                    ClearRegisterFields();
                    ClearLoginFields();
                }
            }
        }
    }

    private IEnumerator LoadLeaderboardData()
    {
        SaveDataButton();

        var DBTask = _dataBaseReference.Child("users").OrderByChild("kills").GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            foreach (Transform child in _scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse())
            {
                var exp = Convert.ToSingle(childSnapshot.Child("exp").Value.ToString());
                var username = childSnapshot.Child("username").Value.ToString();

                GameObject scoreboardElement = Instantiate(_scoreElement, _scoreboardContent);
                scoreboardElement.GetComponent<Leaderboard>().NewScoreElement(username, exp);
            }

            UIManager.instance.ShowLeaderboardScreen();
        }
    }

    public void PasteEmailPassword()
    {
        _emailLoginField.text = "r.salnikov1998@gmail.com";
        _passwordLoginField.text = "12345a";
    }
}