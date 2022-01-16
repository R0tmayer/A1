using UnityEngine;
using UnityEngine.UI;

public class DemoSceneController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject _mainGameCanvas;
    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private GameObject _tutorialCanvas;

    [Header("Buttons")]
    [SerializeField] private Button _backgroundButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private Button _skipTutorialButton;
    [SerializeField] private Button _quitButton;

    private GameObject _currentCanvas;
    private TutorialUI _tutorial;

    private void Awake()
    {
        SetCurrentCanvas(_mainGameCanvas);
    }

    private void OnEnable()
    {
        _continueButton.onClick.AddListener(ContinueGame);
        _settingsButton.onClick.AddListener(OpenSettingsCanvas);
        _backgroundButton.onClick.AddListener(ContinueGame);
        _tutorialButton.onClick.AddListener(ShowGameGuide);
        _skipTutorialButton.onClick.AddListener(ContinueGame);
        _quitButton.onClick.AddListener(CloseApplication);
    }

    private void Update()
    {
        CheckEscapeButtonPress();
    }

    private void OnDisable()
    {
        _continueButton.onClick.RemoveListener(ContinueGame);
        _settingsButton.onClick.RemoveListener(OpenSettingsCanvas);
        _backgroundButton.onClick.RemoveListener(ContinueGame);
        _tutorialButton.onClick.RemoveListener(ShowGameGuide);
        _skipTutorialButton.onClick.RemoveListener(ContinueGame);
        _quitButton.onClick.RemoveListener(CloseApplication);
    }

    private void OpenPauseCanvas()
    {
        Pause();
        SetCurrentCanvas(_pauseCanvas);
    }

    private void OpenSettingsCanvas()
    {
        SetCurrentCanvas(_settingsCanvas);
    }

    private void ContinueGame()
    {
        Unpause();
        SetCurrentCanvas(_mainGameCanvas);
    }

    private void ShowGameGuide()
    {
        SetCurrentCanvas(_tutorialCanvas);
    }

    private void CloseApplication()
    {
        Application.Quit();
    }

    private void Pause()
    {
        Time.timeScale = 0;
    }

    private void Unpause()
    {
        Time.timeScale = 1;
    }

    private void CheckEscapeButtonPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(_currentCanvas == _pauseCanvas)
            {
                ContinueGame();
                return;
            }
            else if (_currentCanvas == _tutorialCanvas)
            {
                ContinueGame();
                return;
            }

            OpenPauseCanvas();
        }
    }

    private void SetCurrentCanvas(GameObject canvas)
    {
        if (_currentCanvas != null)
        {
            _currentCanvas.SetActive(false);
        }

        _currentCanvas = canvas;
        _currentCanvas.SetActive(true);
    }
}
