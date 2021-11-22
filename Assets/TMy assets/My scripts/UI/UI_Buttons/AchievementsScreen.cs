using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsScreen : MonoBehaviour
{
    [SerializeField] private Button _menuButton;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _menuButton.onClick.AddListener(() => { UIManager.Instance.ShowMenuScreen(); });
    }

    private void OnDestroy()
    {
        _menuButton.onClick.RemoveAllListeners();
    }
}
