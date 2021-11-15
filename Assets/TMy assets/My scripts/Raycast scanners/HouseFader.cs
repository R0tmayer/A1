using System.Collections;
using UnityEngine;

public class HouseFader : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed;
    [SerializeField] private float _minFadeValue;
    [SerializeField] private float _fadeInPeriod;
    
    private float _fadeTimer;
    private bool _isFadingIn;
    private IEnumerator _fadeCoroutine;
    private Material[] _materials;

    private void Awake()
    {
        _materials = GetComponent<MeshRenderer>().materials;
    }

    private void Update()
    {
        _fadeTimer += Time.deltaTime;

        if (_isFadingIn)
        {
            return;
        }
        
        if (_fadeTimer >= _fadeInPeriod)
        {
            _isFadingIn = true;
            StartFadeIn();
        }
    }

    public void ResetFadeTimer()
    {
        _fadeTimer = 0;
    }
    
    public void StartFadeOut()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _isFadingIn = false;
        ResetFadeTimer();
        
        _fadeCoroutine = FadeOut();
        StartCoroutine(_fadeCoroutine);
    }

    private void StartFadeIn()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = FadeIn();
        StartCoroutine(_fadeCoroutine);
    }

    private IEnumerator FadeOut()
    {
        Color objectColor = GetComponent<MeshRenderer>().material.color;

        while (objectColor.a > _minFadeValue)
        {
            float fadeAmount = objectColor.a - (_fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);

            SetMaterialsProperties(objectColor);
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        Color objectColor = GetComponent<MeshRenderer>().material.color;

        while (objectColor.a < 1)
        {
            float fadeAmount = objectColor.a + (_fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);

            SetMaterialsProperties(objectColor);
            yield return null;
        }
    }

    private void SetMaterialsProperties(Color color)
    {
        foreach (var material in _materials)
        {
            material.SetColor("_Color", color);
            material.SetFloat("_Mode", 3);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.EnableKeyword("_ALPHABLEND_ON");
            material.renderQueue = 3000;
        }
    }

    
    
}
