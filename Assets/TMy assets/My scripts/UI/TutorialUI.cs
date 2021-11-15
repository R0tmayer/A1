using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private Button _nextStageButton;
    [SerializeField] private Button _skipButton;

    [SerializeField] private GameObject _firstStage;
    [SerializeField] private GameObject _secondStage;
    [SerializeField] private GameObject _thirdStage;

    [SerializeField] private Button _cameraModeButton;

    private void ClearUI()
    {
        _firstStage.SetActive(false);
        _secondStage.SetActive(false);
        _thirdStage.SetActive(false);
    }

    private void OnEnable()
    {
        _nextStageButton.onClick.AddListener(SetNextStage);
        ClearUI();
        _firstStage.SetActive(true);
        _nextStageButton.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _nextStageButton.onClick.RemoveListener(SetNextStage);
    }

    private void SetNextStage()
    {
        if (_firstStage.activeSelf)
        {
            _firstStage.SetActive(false);
            _secondStage.SetActive(true);
        }
        else if (_secondStage.activeSelf)
        {
            _secondStage.SetActive(false);
            _thirdStage.SetActive(true);
            _nextStageButton.gameObject.SetActive(false);
            _cameraModeButton.gameObject.SetActive(true);
            _skipButton.gameObject.SetActive(true);
        }
    }
}
