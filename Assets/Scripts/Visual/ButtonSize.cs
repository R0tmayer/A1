using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSize : MonoBehaviour
{
   private Vector3 _normalSize;
   private Vector3 _zoomSize;
   [SerializeField] private float _size;
   private bool isZoom;
   
   private void Start()
   {
      _normalSize = gameObject.GetComponent<RectTransform>().transform.localScale;
      _zoomSize = gameObject.GetComponent<RectTransform>().transform.localScale * _size;
/*        Debug.Log(_normalSize);
        Debug.Log(_zoomSize);*/
   }

   private void Update()
   {
      if (isZoom)
      {
         transform.localScale = Vector3.Lerp(transform.localScale, _zoomSize, Time.unscaledDeltaTime);
      }

      
   }

   public void Zoom()
   {
      isZoom = true;
   }

   public void NormalZoom()
   {
      isZoom = false;
      transform.localScale = _normalSize;
      
   }
}
