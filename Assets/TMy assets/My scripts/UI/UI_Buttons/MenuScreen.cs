using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _inviteButton;
    [SerializeField] private Button _moreInfoButton;
    [SerializeField] private Button _leaderboardButton;
    [SerializeField] private Button _achievementsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _signOutButton;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _playButton.onClick.AddListener(() => {UIManager.Instance.ShowMapChoiceScreen();});
        _settingsButton.onClick.AddListener(() => {UIManager.Instance.ShowSettingsScreen();});
        _inviteButton.onClick.AddListener(() => {UIManager.Instance.ShowInviteScreen();});
        _moreInfoButton.onClick.AddListener(() => {UIManager.Instance.OpenMoreInfoLink();});
        _leaderboardButton.onClick.AddListener(() => {UIManager.Instance.ShowLeaderboardScreen();});
        _achievementsButton.onClick.AddListener(() => {UIManager.Instance.ShowAchievementsScreen();});
        _exitButton.onClick.AddListener(() => {UIManager.Instance.QuitApplication();});
        _signOutButton.onClick.AddListener(() => {UIManager.Instance.ShowLoginScreen();});
    }

    private void OnDestroy()
    {
        _playButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.RemoveAllListeners();
        _inviteButton.onClick.RemoveAllListeners();
        _moreInfoButton.onClick.RemoveAllListeners();
        _leaderboardButton.onClick.RemoveAllListeners();
        _achievementsButton.onClick.RemoveAllListeners();
        _exitButton.onClick.RemoveAllListeners();
        _signOutButton.onClick.RemoveAllListeners();
    }
}
