using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Auth UI")] 
    
    [SerializeField] private GameObject _loginUI;
    [SerializeField] private GameObject _registerUI;

    [Header("MainMenu UI")] 
    
    [SerializeField] private GameObject _mapChoiceUI;

    [SerializeField] private GameObject _mainMenuUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _inviteFriendUI;
    [SerializeField] private GameObject _userDataUI;
    [SerializeField] private GameObject _leaderboardUI;
    [SerializeField] private GameObject _achievementsUI;

    private AchievenmentListIngame _achievementListInGame;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        _achievementListInGame = FindObjectOfType<AchievenmentListIngame>();
    }

    private void ClearScreen()
    {
        _loginUI.SetActive(false);
        _registerUI.SetActive(false);
        
        _mainMenuUI.SetActive(false);

        _mapChoiceUI.SetActive(false);
        _settingsUI.SetActive(false);
        _inviteFriendUI.SetActive(false);
        _userDataUI.SetActive(false);
        _leaderboardUI.SetActive(false);
        _achievementsUI.SetActive(false);
        
        _achievementListInGame.CloseWindow();
    }

    public void PlayAsGuest()
    {
        //TODO: Load PlayerPrefs...
        ShowMainMenuScreen();
    }

    public void ShowLoginScreen()
    {
        ClearScreen();
        _loginUI.SetActive(true);
    }

    public void ShowRegisterScreen()
    {
        ClearScreen();
        _registerUI.SetActive(true);
    }
    
    public void ShowMainMenuScreen()
    {
        ClearScreen();
        _mainMenuUI.SetActive(true);
    }

    public void ShowMapChoiceScreen()
    {
        ClearScreen();
        _mapChoiceUI.SetActive(true);
    }

    public void ShowSettingsScreen()
    {
        ClearScreen();
        _settingsUI.SetActive(true);
    }

    public void ShowInviteFriendScreen()
    {
        ClearScreen();
        _inviteFriendUI.SetActive(true);
    }

    public void ShowUserDataScreen()
    {
        ClearScreen();
        _userDataUI.SetActive(true);
    }

    public void ShowLeaderboardScreen()
    {
        ClearScreen();
        _leaderboardUI.SetActive(true);
    }
    
    public void ShowAchievementsScreen()
    {
        ClearScreen();
        _achievementsUI.SetActive(true);
        _achievementListInGame.OpenWindow();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
    public void LoadFirstMap()
    {
        SceneManager.LoadScene(StaticSceneNames.GAME_SCENE);
    }
    
    public void LoadSecondMap()
    {
        SceneManager.LoadScene(StaticSceneNames.GAME_SCENE);
    }
    
    public void LoadThirdMap()
    {
        SceneManager.LoadScene(StaticSceneNames.GAME_SCENE);
    }
}