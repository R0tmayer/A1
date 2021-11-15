using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace PlayerCar
{
   [RequireComponent(typeof(PathCreator))]
   
   public class PlayerCarSelector : MonoBehaviour
   {
      [SerializeField] private PathCreator _pathCreator;
      [SerializeField] private PathMover _pathMover;
      
      private static string _playerCarLayer = "PlayerCar";
      private const string _mouseButton = "Fire1";

      private int _layerMask;
      [SerializeField] private bool _isCarSelected = true;

      private void Start()
      {
         StartCoroutine(TimerEnablePathCreator());
      }

      private void Update()
      {
         _layerMask = 1 << LayerMask.NameToLayer(_playerCarLayer);
         if (Input.GetButtonDown(_mouseButton))
         {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
               return;
            if (hit.collider.transform.parent.gameObject != transform.gameObject) 
               return;
            _isCarSelected = true;
         }
         
         else if (Input.GetButtonUp(_mouseButton) && _isCarSelected)
         {
            //StartCoroutine(TimerEnablePathCreator());
            _isCarSelected = false;
         }
      }

      private IEnumerator TimerEnablePathCreator()
      {
         while (true)
         {
            yield return new WaitForSeconds(0.1f);
            EnablePathCreator(); 
            yield break;
         }
      }
      private void EnablePathCreator()
      {
         _pathCreator.enabled = true;
         _pathMover.enabled = true;
         enabled = false;
      }
   }
}