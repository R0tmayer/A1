using BGGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metric : SceneSingleton<Metric>
{
    public bool isOnMetric;

    public GameObject policeCarSpeed;
    [Space]
    [Space]
    public GameObject rob1CarSpeed;
    /*    public GameObject rob2CarSpeed;
        public GameObject rob3CarSpeed;*/
    [Space]
    [Space]
    public GameObject spawnRobTIme;
    public GameObject maxRobAtTime;
    [Space]
    [Space]
    public GameObject moneyAfterProtect;
    public GameObject expAfterProtect;
    [Space]
    [Space]
    public GameObject moneyAfterRob;
    public GameObject expBeforRob;
    [Space]
    [Space]
    public GameObject priceUpdateSecurity;
    public GameObject priceUpdateSignal;
    public GameObject priceUpdateCamera;

    public GameObject timeRob;
    public GameObject timeProtect;
    [Space]
    [Space]
    public GameObject speedAfterUpdate;
    [Space]
    [Space]
    public GameObject valuePowerSecurity;
    [Space]
    [Space]
    public GameObject signalizationValue;
    public GameObject cameraValue;
    [Header("___")]
    [Space]
    [Space]
    public GameObject ButtonOpen;
    /*    [Space]
        [Space]
        public GameObject Security;
        [Space]
        [Space]
        public GameObject Rob1;
        public GameObject Rob2;
        public GameObject Rob3;*/

    ///

    [SerializeField] private GameObject window;

    public void OpenMetric () {
        Time.timeScale = 0;
        window.SetActive(true);
    }

    public void CloseMetric () {
        Time.timeScale = 1;
        window.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(WaitAndSetValue());
        CloseMetric();
    }

    IEnumerator WaitAndSetValue() {
        yield return new WaitForSeconds(1);
        ButtonOpen.SetActive(true);

    }


}
