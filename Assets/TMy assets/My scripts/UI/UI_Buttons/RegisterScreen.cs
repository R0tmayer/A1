using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterScreen : MonoBehaviour
{
    [SerializeField] private Button _menuButton;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _menuButton.onClick.AddListener(() => {UIManager.Instance.ShowLoginScreen();});
    }

    private void OnDestroy()
    {
        _menuButton.onClick.RemoveAllListeners();
    }
}
