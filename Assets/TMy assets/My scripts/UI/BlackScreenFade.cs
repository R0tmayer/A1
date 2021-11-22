using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlackScreenFade : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed;
    [SerializeField] private Image _image;

    private void Start()
    {
        FadeIn();
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

    private void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

}
