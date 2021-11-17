using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceHolder : MonoBehaviour
{
    public float value = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
