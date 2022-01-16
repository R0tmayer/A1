using UnityEngine;

public class MainScanner : AScanner
{
    private void Update()
    {
        
#if UNITE_EDITOR
        MouseScan();
#endif

#if UNITY_ANDROID
        TouchScan();
#endif
        
    }

    private void MouseScan()
    {
        if (Input.GetMouseButton(0)) //&& TargetsManager.Instance.isDrawLine)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastScan(ray);
        }
    }

    private void TouchScan()
    {
        if (Input.touchCount > 0) //&& TargetsManager.Instance.isDrawLine)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                RaycastScan(ray);
            }
        }
        
    }
}