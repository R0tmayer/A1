using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetricaVal : MonoBehaviour
{
    public float value;
    public Slider slider;
    public Text val;

    
/*    public void SetValFromGame(float val) {
        value = (float)Math.Round(val, 1);
        slider.value = value;
    }
*/
    void Awake()
    {
        value = slider.value;
        val.text = slider.value.ToString();
        slider.onValueChanged.AddListener(delegate { SetValSlider(); });
    }

    public void SetValSlider() {

        value = (float)Math.Round(slider.value, 1);
        slider.value = value;
        val.text = value.ToString();
    }
}
