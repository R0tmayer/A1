using UnityEngine;

public abstract class AScanner : MonoBehaviour
{
    protected HouseFader currentHitHouseFader;
    protected Camera mainCamera;

    protected void Awake()
    {
        mainCamera = Camera.main;
    }

    protected void RaycastScan(Ray ray)
    {
        float rayLength = 100f;
        int layerMask = 1 << 10; //House layer

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength, layerMask))
        {
            if (hit.transform.TryGetComponent(out HouseFader houseFader))
            {
                if (currentHitHouseFader == houseFader)
                {
                    currentHitHouseFader.ResetFadeTimer();
                    return; // Нет надобности повторно запускать FadeOut если она уже запущена
                }

                currentHitHouseFader = houseFader;
                currentHitHouseFader.StartFadeOut();
            }
        }
    }

}
