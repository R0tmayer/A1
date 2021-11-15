using System;
using UnityEngine;

namespace PlayerCar
{
   [RequireComponent(typeof(PathMover))]
   
   public class PathDestroyer : MonoBehaviour
   {
      [SerializeField] private PathMover _pathMover;
      [SerializeField] private LineRenderer _lineRenderer;

      
      private void OnEnable()
      {
         _pathMover.PathReached += DestroyPath;
      }

      private void OnDisable() =>
         _pathMover.PathReached -= DestroyPath;

      private void DestroyPath()
      {
         _lineRenderer.positionCount = 0;
         _pathMover.PathReached -= DestroyPath;
      }
   }
}