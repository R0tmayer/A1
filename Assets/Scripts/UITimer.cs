using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{

    [SerializeField] private Text valTime;
    [SerializeField] private GameObject bg;
    public Window_QuestPointer child;

    public void SetNewTime(string t, bool isRob) {
        valTime.text = t.ToString();
        valTime.color = isRob ? Color.red : Color.white;
    }

    public void SetBG(bool st) {
        bg.SetActive(st);
    }
    
}
