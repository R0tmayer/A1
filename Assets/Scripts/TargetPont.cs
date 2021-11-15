using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPont : MonoBehaviour
{
    [SerializeField] private GameObject _vfx;
    [SerializeField] public House _house;

    [ContextMenu("VfxOn")]
    public void VfxOn() {
        _vfx.SetActive(true);
    }

    [ContextMenu("VfxOff")]
    public void VfxOff() {
        _vfx.SetActive(false);
    }

    public void SetParentHouse(House house) {
        _house = house;
    }
}
