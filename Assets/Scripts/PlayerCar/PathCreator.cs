using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayerCar
{
   [RequireComponent(typeof(LineRenderer))]
   
   public class PathCreator : MonoBehaviour
   {
        [SerializeField] private float _distanceBetweenPoints = 2f;
        [SerializeField] private Vector3 _pointOffset = Vector3.up;
        public int _lastPointIndex;
      
        private const string _mouseButton = "Fire1";
        private const string _roadLayer = "Road";
        private static string _playerCarLayer = "PlayerCar";
        private static string _startPlayerCar = "StartPlayerCar";
        private static string _endPointLayer = "PointTarget";
        private int _layerMask;
        private int _layerMaskToPlayer;
        private int _layerMaskToEndPoint;
        private int _layerStartPlayerCart;

        
        bool rightDistantion; 
        bool notProtected; // Если никто не едет защищать
        bool nowRob;
        
        private LineRenderer _lineRenderer;
        public List<Vector3> _points = new List<Vector3>();
        private int _closestPointIndex;

        public bool _isMove;

        private bool _pathIinterrupted;
      
        public Action<List<Vector3>> OnNewPathCreated;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _layerMask = 1 << LayerMask.NameToLayer(_roadLayer);
            _layerMaskToPlayer = 1 << LayerMask.NameToLayer(_playerCarLayer);
            _layerMaskToEndPoint = 1 << LayerMask.NameToLayer(_endPointLayer);
            _layerStartPlayerCart = 1 << LayerMask.NameToLayer(_startPlayerCar);
        }

        private void Update()
        {

            if (_isMove) return;
            
            if (TargetsManager.Instance.isDrawLine || _isMove)
            {
                GetComponent<SecurityController>().fieldPathDraw.SetActive(false);

            }
            else {
                if (_points.Count > 1)
                {
                    if (TargetsManager.Instance.isDrawLine)
                    {
                        GetComponent<SecurityController>().fieldPathDraw.SetActive(false);

                    }
                    else
                    {
                        if (!TargetsManager.Instance.isBuildMode) return;
                        GetComponent<SecurityController>().fieldPathDraw.transform.position = _points.Last();
                        GetComponent<SecurityController>().fieldPathDraw.SetActive(true);
                    }
                }



            }


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            RaycastHit hitPlayer;
            RaycastHit endTarget;

            bool rayCastRoad = Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask);

            //if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMaskToPlayer)){}
            Vector3 hitPoint = hit.point +_pointOffset;


            if (Input.GetButtonDown(_mouseButton) && GetComponent<SecurityController>().autoPilot == false)
            {

                if (Physics.Raycast(ray, out hitPlayer, Mathf.Infinity, _layerStartPlayerCart))
                {
                    if (rayCastRoad && !_pathIinterrupted)
                    {
                        _points.Clear();
                        _points.Add(hit.point + _pointOffset);
                        
                        _lastPointIndex = 0;
                        OnDrawLine();
                       // MainPlayer.Instance.PathError = "Начало рисования маршрутка";
                        return;
                    }
                }
                else
                {
                    // MainPlayer.Instance.PathError = "Не верная точка начала маршрутка";
                    OffDrawLine();
                }
            }


            if (_points.Count > 0) {
                if (Input.GetButton(_mouseButton))
                {
                    if (!rayCastRoad)
                    {
                        _pathIinterrupted = _points.Count > 0;
                        return;
                    }
                    _pathIinterrupted = false;

                    if (DistanceToClosestPoint(hitPoint) < _distanceBetweenPoints)
                    {
                        RemoveSparePoints();
                        _lastPointIndex = _points.Count - 1;
                        return;
                    }

                    if ((DistanceToLast(hitPoint) < _distanceBetweenPoints))
                    {
                       // MainPlayer.Instance.PathError = "Рисую маршрут";
                        if (_points.Count == _lastPointIndex + 1) {
                            _points.Add(hitPoint);
                            OnDrawLine();
                        }
                        else
                        {
                            OnDrawLine();
                            _points[_lastPointIndex + 1] = hitPoint;
                        }

                        DrawPath();
                        return;
                    }
                    else
                    {
                        if (Vector3.Distance(hitPoint, _points.Last()) < _distanceBetweenPoints * 1.2f)
                        {
                            _points.Add(hitPoint);
                            OnDrawLine();
                        }
                    }

                    _lastPointIndex = _points.Count - 1;
                    DrawPath();
                }
                else if (Input.GetButtonUp(_mouseButton))
                {
                    if (Physics.Raycast(ray, out endTarget, Mathf.Infinity, _layerMaskToEndPoint) )
                    {
                        if (!_isMove)
                        {
                            GetComponent<SecurityController>().tartgetHouse = endTarget.collider.gameObject.GetComponent<TargetPont>();
      
                        }
                        else
                        {
                            /*MainPlayer.Instance.ShowMessage("Сюда уже едет другая машина");*/
                        }
                        
                        var house = GetComponent<SecurityController>().tartgetHouse;

                        rightDistantion = Vector3.Distance(hitPoint, _points.Last()) < _distanceBetweenPoints * 1.2f; 
                        notProtected = !GetComponent<SecurityController>().tartgetHouse._house.securityProtected; // Если никто не едет защищать
                        nowRob = (GetComponent<SecurityController>().tartgetHouse._house.rob || GetComponent<SecurityController>().tartgetHouse._house.going_to_rob); //  Если дом грабят или едут грабить
                        
                        if (!_pathIinterrupted && rightDistantion && notProtected && nowRob)
                        {
                           
                            StartMoveAutopilot(_points);
                        } else
                        {
                            if (!nowRob)
                            {
                                // ErrorPath("Точку никто не грабит");
                                OffDrawLine();
                            }
                            else if (!notProtected)
                            {
                                // ErrorPath("Точку уже под защитой");
                                if (_isMove) return;
                                MainPlayer.Instance.ShowMessage("Сюда уже едет другая машина");
                                OffDrawLine();
                            }

                        }

                    } else
                    {
                        //MainPlayer.Instance.ShowMessage("Не верная точка завершения маршрута");
                        OffDrawLine();
                    }
                 

                }
                
            }

        }


        public void StartMoveAutopilot(List<Vector3> points)
        {
            _points = points;
            OffDrawLine();
           // ErrorPath("Маршрут построен");
            GetComponent<SecurityController>().tartgetHouse._house.SetSecurityProtected();
            OnNewPathCreated?.Invoke(points);
            _isMove = true;
            GetComponent<SecurityController>().Arrow.SetActive(false);
            Destroy(GetComponent<SecurityController>().StartField);
           
            GetComponent<SecurityController>().fieldPathDraw.SetActive(false);
            _lineRenderer.enabled = false;
        }

        private void OffDrawLine() {
            TargetsManager.Instance.isDrawLine = false;
        }

        private void OnDrawLine()
        {
            if (!TargetsManager.Instance.isBuildMode) return;
            TargetsManager.Instance.isDrawLine = true;
        }

        private void DrawPath()
      {
          if (!_isMove)
          {
              _lineRenderer.positionCount = _points.Count;
              _lineRenderer.SetPositions(_points.ToArray());
          }
 
      }

      private void RemoveSparePoints()
      {
         for (int i = _points.Count-1; i > _closestPointIndex; i--)
            _points.Remove(_points.Last());
      }

      private float DistanceToLast(Vector3 hitPoint) =>
         !_points.Any() ? Mathf.Infinity : Vector3.Distance(_points[_lastPointIndex], hitPoint);
      
      private float DistanceToClosestPoint(Vector3 hitPoint) => 
         _points.Count <= 2 ? Mathf.Infinity : ClosestPointDistance(hitPoint);

      private float ClosestPointDistance(Vector3 hitPoint)
      {
         float distance = 100f;
         for (int i = 0; i < _points.Count -2; i++)
         {
            Vector3 point = _points[i];
            if (Vector3.Distance(point, hitPoint) < distance)
            {
               distance = Vector3.Distance(point, hitPoint);
               _closestPointIndex = i;
            }
         }
         return distance;
      }
   }
}