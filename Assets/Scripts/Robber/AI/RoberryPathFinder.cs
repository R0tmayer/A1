using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoberryPathFinder : MonoBehaviour
{
    public House movePositionHouse;
    private NavMeshAgent _navMeshAgent;
    private RobberController _robberController;
    public bool isStartMove;
    public bool isRobbies;


    [SerializeField] private float _rangeToTargetPoint = 2f;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _robberController = GetComponent<RobberController>();
    }

    public void Move() {
        _navMeshAgent.destination = movePositionHouse.transformForPathFinder.position;
        var dist1 = Vector3.Distance(movePositionHouse.transformForPathFinder.position, transform.position);
        var dist2 = movePositionHouse.security ? Vector3.Distance(movePositionHouse.security.GetComponent<Transform>().position, transform.position) : 555f;
      
        if ((dist1 < _rangeToTargetPoint && !isRobbies) || (dist2 < 4f && movePositionHouse.security.isStoped && !isRobbies && movePositionHouse.securityProtected))
        {
            _navMeshAgent.isStopped = true;
            isRobbies = true;
        }
    }

    public void SetTarget(House target) {
        isStartMove = true;
        movePositionHouse = target;
        movePositionHouse.houseRob = gameObject.GetComponent<NavMeshAgent>() ;
    }
    
    

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            var path = _navMeshAgent.path;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            } 
        }
    }
}
