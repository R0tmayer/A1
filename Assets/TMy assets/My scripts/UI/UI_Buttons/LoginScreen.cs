using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField _emailField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _registerButton;
    [SerializeField] private Button _pasteButton;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _registerButton.onClick.AddListener(() => {UIManager.Instance.ShowRegisterScreen();});
        _pasteButton.onClick.AddListener(() => {UIManager.Instance.PasteData(_emailField, _passwordField);});
    }

    private void OnDestroy()
    {
        _loginButton.onClick.RemoveAllListeners();
        _registerButton.onClick.RemoveAllListeners();
        _pasteButton.onClick.RemoveAllListeners();
    }
}
