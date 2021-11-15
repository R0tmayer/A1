using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    [SerializeField] private float minSize = 8;
    [SerializeField] private float maxSize = 18;
    [SerializeField] private float sens = 10;
    private Camera m_OrthographicCamera;


    int i;


    private void Start()
    {
        m_OrthographicCamera = GetComponent<Camera>();
    }


    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag) / sens ;
            Zoom(deltaMagnitudeDiff);
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel") * sens / 10);

        

    }

    private void Zoom(float deltaMagnitudeDiff) {

        m_OrthographicCamera.orthographicSize = Mathf.Clamp(m_OrthographicCamera.orthographicSize + deltaMagnitudeDiff, minSize, maxSize);
    }


}


