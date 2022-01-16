using UnityEngine;

public class CarScanner : AScanner
{
    [SerializeField] private float _timeToScan;
    private float _scanTimer;

    private void Update()
    {
        _scanTimer += Time.deltaTime;

        if (_scanTimer >= _timeToScan)
        {
            Ray ray = mainCamera.ScreenPointToRay(mainCamera.WorldToScreenPoint(transform.position));
            RaycastScan(ray);
        }
    }

}