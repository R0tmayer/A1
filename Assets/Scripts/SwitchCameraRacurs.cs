using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwitchCameraRacurs : MonoBehaviour
{
    private Quaternion _viewRacurs;
    [SerializeField] private Quaternion _buildRacurs;

    private Camera _camera;

    private float _elapsedTime;
    [SerializeField] private bool _isSwitch;

    [SerializeField]  private float SwichSpeed = 2;

    [SerializeField] private GameObject ButtonBuildMode;
    [SerializeField] private GameObject ButtonViewMode;



    void Start()
    {
        _camera = GetComponent<Camera>();
        _viewRacurs = _camera.transform.localRotation;
    }


    public void SetBuildMode() {
        ButtonBuildMode?.SetActive(false);
        ButtonViewMode?.SetActive(true);
        TargetsManager.Instance.isBuildMode = true;
        _isSwitch = true;
        _elapsedTime = 0;
    }


    public void SetViewMode() {
        ButtonBuildMode?.SetActive(true);
        ButtonViewMode?.SetActive(false);
        TargetsManager.Instance.isBuildMode = false;
        _isSwitch = true;
        _elapsedTime = 0;
    }


    void Update()
    {
        if (TargetsManager.Instance.isBuildMode && _isSwitch)
        {

            if (_camera.transform.localRotation == _buildRacurs)
            {
                _isSwitch = false;
                return;
            }

            if (_camera.transform.localRotation != _buildRacurs)
            {
                _elapsedTime += Time.deltaTime;
                _camera.transform.localRotation = Quaternion.Lerp(_camera.transform.localRotation, _buildRacurs, _elapsedTime * SwichSpeed / 10);
                if (TargetsManager.Instance.factoriesSecurity._lastSpawnObject != null)
                {
                    var carPos = TargetsManager.Instance.factoriesSecurity._lastSpawnObject.position;
                    _camera.transform.position = Vector3.Lerp(_camera.transform.position, new Vector3(carPos.x, _camera.transform.position.y, carPos.z), _elapsedTime * SwichSpeed / 10);
                }
               
            }

        }

        if (!TargetsManager.Instance.isBuildMode && _isSwitch)
        {
            if (_camera.transform.localRotation == _viewRacurs)
            {
                _isSwitch = false;
                return;
            }

            if (_camera.transform.localRotation != _viewRacurs)
            {
                _elapsedTime += Time.deltaTime;
                _camera.transform.localRotation = Quaternion.Lerp(_camera.transform.localRotation, _viewRacurs, _elapsedTime * SwichSpeed / 10);
                var carPos = TargetsManager.Instance.factoriesSecurity._lastSpawnObject.position;
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, new Vector3(carPos.x-25, _camera.transform.position.y, carPos.z+25), _elapsedTime * SwichSpeed / 10);
            }
           
        }


    }
}
