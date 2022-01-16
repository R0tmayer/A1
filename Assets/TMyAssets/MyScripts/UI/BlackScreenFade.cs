using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlackScreenFade : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        FadeOut();
    }

    private IEnumerator FadeOutCoroutine()
    {
        var tempColor = _image.color;

        while (tempColor.a > 0)
        {
            tempColor.a -= _fadeSpeed * Time.deltaTime;
            _image.color = tempColor;
            yield return null;

        }
    }

    private IEnumerator FadeInCoroutine()
    {
        var tempColor = _image.color;

        while (tempColor.a < 1)
        {
            tempColor.a += _fadeSpeed * Time.deltaTime;
            _image.color = tempColor;
            yield return null;
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

}
