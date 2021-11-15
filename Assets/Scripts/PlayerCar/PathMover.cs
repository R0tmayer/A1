using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace PlayerCar
{
    [RequireComponent(typeof(NavMeshAgent))]

    public class PathMover : MonoBehaviour
    {
        public float _speed = 1f;
        [SerializeField] private PathCreator _pathCreator;

        private static int _simplifierPath = 2;

        private NavMeshAgent _navMeshAgent;
        public Queue<Vector3> _pathPoints = new Queue<Vector3>();
        private bool _isPathCreated;

        public Action PathReached;

        private void OnEnable() =>
           _pathCreator.OnNewPathCreated += SetPoints;
        private void OnDisable() =>
           _pathCreator.OnNewPathCreated -= SetPoints;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            
        }

        private void Start()
        {
            _navMeshAgent.speed = _speed;
        }
        private void Update() =>
           MovingThoughPath();

        private void MovingThoughPath()
        {
            if (ShouldSetDestination())

                _navMeshAgent.SetDestination(_pathPoints.Dequeue());
            if (!IsPathNeedToDestroy())
                return;

            PathReached?.Invoke();
            enabled = false;
        }

        private bool IsPathNeedToDestroy() =>
           _isPathCreated == true && _pathPoints.Count == 0 && _navMeshAgent.hasPath == false && _navMeshAgent.remainingDistance < 0.2f;

        private bool ShouldSetDestination()
        {
            if (_pathPoints.Count == 0)

                return false;
            return _navMeshAgent.hasPath == false || _navMeshAgent.remainingDistance < 0.5f;
        }


        public void StopMove() {
            _navMeshAgent.isStopped = true;
        }


      private void SetPoints(List<Vector3> points)
      {
         _isPathCreated = true;
         _pathPoints = new Queue<Vector3>(CreateTempPath(points));
         _pathCreator.OnNewPathCreated -= SetPoints;
      }

      private static IEnumerable<Vector3> CreateTempPath(IReadOnlyList<Vector3> points)
      {
         List<Vector3> tmp = new List<Vector3>();
         for (int i = 0; i < points.Count; i += _simplifierPath)
         {
            tmp.Add(points[i]);
         }

         if (!tmp.Contains(points.Last()))
            tmp.Add(points.Last());
         return tmp;
      }
   }
}