using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private GameObject _leaderboardUI;
    [SerializeField] private GameObject _achievementsUI;

    [Header("Toggle Buttons if Guest")]
    [SerializeField] private Button _inviteButton;
    [SerializeField] private Button _leaderboardButton;

    private AchievenmentListIngame _achievementListInGame;
    private BlackScreenFade _blackScreenFade;

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
        _blackScreenFade = FindObjectOfType<BlackScreenFade>();
    }

    private void ClearScreen()
    {
        _loginUI.SetActive(false);
        _registerUI.SetActive(false);

        _mainMenuUI.SetActive(false);

        _mapChoiceUI.SetActive(false);
        _settingsUI.SetActive(false);
        _inviteFriendUI.SetActive(false);
        _leaderboardUI.SetActive(false);
        _achievementsUI.SetActive(false);

        _achievementListInGame.CloseWindow();
    }

    public void PlayAsGuest()
    {
        ShowMainMenuScreen();

        _inviteButton.interactable = false;
        _leaderboardButton.interactable = false;

        GuestHolder.state = true;
    }

    public void OpenHyperlink()
    {
        Application.OpenURL("https://www.a1.by/");
    }

    public void ShowLoginScreen()
    {
        //TODO Set correct saveName
        _inviteButton.interactable = true;
        _leaderboardButton.interactable = true;

        GuestHolder.state = false;
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

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        _blackScreenFade.FadeIn();
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
}