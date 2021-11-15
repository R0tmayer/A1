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
    private DatabaseReference _DBreference;

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
    [SerializeField] private TMP_InputField _killsField;
    [SerializeField] private TMP_InputField _deathsField;
    [SerializeField] private GameObject _scoreElement;
    [SerializeField] private Transform _scoreboardContent;

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

    private void InitializeFirebase()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _DBreference = FirebaseDatabase.DefaultInstance.RootReference;
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
        var exp = int.Parse(_expField.text);
        var kills = int.Parse(_killsField.text);
        var deaths = int.Parse(_deathsField.text);
        const bool busy = true;

        StartCoroutine(UpdateUserData(username, exp, kills, deaths, busy));
    }

    private IEnumerator UpdateUsernameAuth(string username)
    {
        var profile = new UserProfile {DisplayName = username};

        Task ProfileTask = _user.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(() => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {ProfileTask.Exception}");
        }
    }

    private IEnumerator UpdateUserData(string username, int exp, int kills, int deaths, bool busy)
    {
        Task usernameTask = _DBreference.Child("users").Child(_user.UserId).Child("username").SetValueAsync(username);
        yield return new WaitUntil(() => usernameTask.IsCompleted);

        if (usernameTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register usernameTask with {usernameTask.Exception}");
        }

        Task expTask = _DBreference.Child("users").Child(_user.UserId).Child("exp").SetValueAsync(exp);
        yield return new WaitUntil(() => expTask.IsCompleted);

        if (expTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register expTask with {expTask.Exception}");
        }

        Task killsTask = _DBreference.Child("users").Child(_user.UserId).Child("kills").SetValueAsync(kills);
        yield return new WaitUntil(() => killsTask.IsCompleted);

        if (killsTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register killsTask with {killsTask.Exception}");
        }

        Task deathsTask = _DBreference.Child("users").Child(_user.UserId).Child("deaths").SetValueAsync(deaths);
        yield return new WaitUntil(() => deathsTask.IsCompleted);

        if (deathsTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register deathsTask with {deathsTask.Exception}");
        }

        Task busyTask = _DBreference.Child("users").Child(_user.UserId).Child("busy").SetValueAsync(busy);
        yield return new WaitUntil(() => busyTask.IsCompleted);

        if (busyTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register busyTask with {busyTask.Exception}");
        }

        Task timeStampTask = _DBreference.Child("users").Child(_user.UserId).Child("timestamp")
            .SetValueAsync(ServerValue.Timestamp);
        yield return new WaitUntil(() => timeStampTask.IsCompleted);

        if (timeStampTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register timeStampTask with {timeStampTask.Exception}");
        }
    }

    private IEnumerator LoadUserData()
    {
        var DBTask = _DBreference.Child("users").Child(_user.UserId).GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            _usernameField.text = _user.DisplayName;
            _expField.text = "0";
            _killsField.text = "0";
            _deathsField.text = "0";
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            _usernameField.text = snapshot.Child("username").Value.ToString();
            _expField.text = snapshot.Child("exp").Value.ToString();
            _killsField.text = snapshot.Child("kills").Value.ToString();
            _deathsField.text = snapshot.Child("deaths").Value.ToString();
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
            AuthError errorCode = (AuthError) firebaseException.ErrorCode;

            var message = errorCode switch
            {
                AuthError.MissingEmail => "Missing Email",
                AuthError.MissingPassword => "Missing Password",
                AuthError.WrongPassword => "Wrong Password",
                AuthError.InvalidEmail => "Invalid Email",
                AuthError.UserNotFound => "Account does not exist",
                _ => "Login Failed!"
            };
            _warningLoginText.text = message;
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
                AuthError errorCode = (AuthError) firebaseEx.ErrorCode;

                var message = errorCode switch
                {
                    AuthError.MissingEmail => "Missing Email",
                    AuthError.MissingPassword => "Missing Password",
                    AuthError.WeakPassword => "Weak Password",
                    AuthError.EmailAlreadyInUse => "Email Already In Use",
                    _ => "Register Failed!"
                };
                _warningRegisterText.text = message;
            }
            else
            {
                _user = registerTask.Result;

                if (_user == null)
                {
                    yield break;
                }

                var profile = new UserProfile {DisplayName = username};

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
        var DBTask = _DBreference.Child("users").OrderByChild("kills").GetValueAsync();

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
                var username = childSnapshot.Child("username").Value.ToString();
                var kills = int.Parse(childSnapshot.Child("kills").Value.ToString());
                var deaths = int.Parse(childSnapshot.Child("deaths").Value.ToString());
                var exp = int.Parse(childSnapshot.Child("exp").Value.ToString());

                GameObject scoreboardElement = Instantiate(_scoreElement, _scoreboardContent);
                scoreboardElement.GetComponent<Leaderboard>().NewScoreElement(username, kills, deaths, exp);
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