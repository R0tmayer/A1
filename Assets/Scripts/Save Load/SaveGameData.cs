using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGameData
{
    [Header("Общее")]
    public float money;
    public float exp;

    [Header("Полицейский участок")]
    public PoliceState policeStates;

    [Header("Все здания")]
    public HouseState[] housesState;


    public SaveGameData(MainPlayer mainPlayer, TargetsManager targetsManager) {
        money = mainPlayer.Money;
        exp = mainPlayer.Raiting ;


        policeStates.upg_moreCar = TargetsManager.Instance.factoriesSecurity.upg_moreCar;
        policeStates.upg_teach = TargetsManager.Instance.factoriesSecurity.upg_teach;
        policeStates.upg_powerUp = TargetsManager.Instance.factoriesSecurity.upg_powerUp;
        policeStates.upg_tablet = TargetsManager.Instance.factoriesSecurity.upg_tablet;
        policeStates.upg_radio = TargetsManager.Instance.factoriesSecurity.upg_radio;
        policeStates.upg_fleshers = TargetsManager.Instance.factoriesSecurity.upg_fleshers;

        housesState = new HouseState[TargetsManager.Instance.houses.Count];

        for (int i = 0; i < TargetsManager.Instance.houses.Count; i++)
        {
            housesState[i].upg_zabor_or_signalization = TargetsManager.Instance.houses[i].upg_zabor_or_signalization;
            housesState[i].upg_camera = TargetsManager.Instance.houses[i].upg_camera;
        }

    }

    [System.Serializable]
    public struct PoliceState
    {
        public bool
            upg_moreCar,
            upg_teach,
            upg_powerUp,
            upg_tablet,
            upg_radio,
            upg_fleshers;

        public PoliceState(bool upg_moreCar, bool upg_teach, bool upg_powerUp, bool upg_tablet, bool upg_radio, bool upg_fleshers)
        {
            this.upg_moreCar = upg_moreCar;
            this.upg_teach = upg_teach;
            this.upg_powerUp = upg_powerUp;
            this.upg_tablet = upg_tablet;
            this.upg_radio = upg_radio;
            this.upg_fleshers = upg_fleshers;
        }
    }

    [System.Serializable]
    public struct HouseState
    {
        public bool
            upg_zabor_or_signalization,
            upg_camera;

        public HouseState(bool upg_zabor_or_signalization, bool upg_camera)
        {
            this.upg_zabor_or_signalization = upg_zabor_or_signalization;
            this.upg_camera = upg_camera;
        }

    }


}


