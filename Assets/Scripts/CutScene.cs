using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class CutScene : MonoBehaviour
{
    [SerializeField] private GameObject _emptyImageObject;
    [SerializeField] private Sprite[] _sprites;

    private float _elapsedTime;
    [SerializeField] private float _swichSpeed = 2;
    [SerializeField] private float _fadeSpeed = 1;
    private bool _isMove;

    [SerializeField] private string _namePrefs;

    private float _wScreen;
    private int iter;
    private float coefficientWieght = 2;

    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _endButton;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _nextSlide;

    [SerializeField] private bool _skipCutScene;


    private void Start()
    {
        if (_namePrefs == null)
        {
            Debug.LogError("Незвание _namePrefs не указано");
        }

        _wScreen = Screen.width;

        if (PlayerPrefs.GetInt(_namePrefs) == 1 || _skipCutScene)
        {
            EndCutScene();
        }
        else {
            _nextButton.SetActive(true);
            for (int i = 0; i < _sprites.Length; i++)
            {
                GameObject image = Instantiate(_emptyImageObject, GetComponent<RectTransform>().position, Quaternion.identity, transform);
                image.GetComponent<Image>().sprite = _sprites[i];
                image.GetComponent<RectTransform>().localPosition += new Vector3(i * _wScreen * coefficientWieght, 0, 0);
            }
        }
    }


    private void Update()
    {
        if (_isMove)
        {
            _elapsedTime += Time.deltaTime;
            var newPos = iter * new Vector3(_wScreen* coefficientWieght, 0, 0);
            GetComponent<RectTransform>().localPosition = Vector3.Lerp(GetComponent<RectTransform>().localPosition, newPos, _elapsedTime * _swichSpeed / 10);

            if (transform.localPosition == newPos)
            {
                GetComponent<RectTransform>().localPosition = newPos;
                _isMove = false;
            }
        }
    }

    [ContextMenu("дальше")]
    public void NextSlide() {
        _audioSource.PlayOneShot(_nextSlide);
        iter--;
        if (iter*-1 + 1 == _sprites.Length)
        {
            _nextButton.SetActive(false);
            _endButton.SetActive(true);
        }
        _isMove = true; 
        _elapsedTime = 0;
    }

    public void EndSlide() {
        NextSlide();
        StartCoroutine(FadeSlide());
    }

    IEnumerator FadeSlide() {
        _endButton.SetActive(false);
        while (gameObject.GetComponent<CanvasGroup>().alpha > 0)
        {
            gameObject.GetComponent<CanvasGroup>().alpha -= 0.025f / _fadeSpeed;
            yield return null;
        }
        PlayerPrefs.SetInt(_namePrefs, 1);
        EndCutScene();
    }

    private void EndCutScene() {
        gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    public void ResetPrefs() {
        PlayerPrefs.SetInt(_namePrefs, 0);
        AchivemntController.Instance.ResetAchivement();
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
