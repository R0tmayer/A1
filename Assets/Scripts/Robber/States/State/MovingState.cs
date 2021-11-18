using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class ExtensionMethods
{


    public static float GetPathRemainingDistance(this NavMeshAgent navMeshAgent)
    {
        if (navMeshAgent.pathPending ||
            navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
            navMeshAgent.path.corners.Length == 0)
            return -1f;

        float distance = 0.0f;
        for (int i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
        }

        return distance;
    }
}

public class MovingState : State
{
    private float val;
    private float time;

    public MovingState(RobberController character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        character.roberryPathFinder.movePositionHouse.OnCompleteRobbir += EndRobbie;
        character.roberryPathFinder.movePositionHouse.StartRobbery();
        character.roberryPathFinder.movePositionHouse.going_to_rob = true;

        val = Mathf.Round(character.roberryPathFinder.movePositionHouse.upg_zabor_or_signalization ?
            character.factorPropertyperSecodn / 1.5f :
            character.factorPropertyperSecodn);

        time = Mathf.Round(character.roberryPathFinder.movePositionHouse.Property / val);

        //;

    }
    public void EndRobbie()
    {
        stateMachine.ChangeState(character.escaping);
    }

    public override void HandleInput()
    { }

    public override void LogicUpdate()
    {
        if (character.roberryPathFinder.isStartMove)
            character.roberryPathFinder.Move();

        if (character.roberryPathFinder.isRobbies)
            stateMachine.ChangeState(character.robbery);


        character.roberryPathFinder.movePositionHouse.marker.GetComponent<UITimer>().SetNewTime(""+(int)((character.navMeshAgent.GetPathRemainingDistance() / character.navMeshAgent.speed)+ time), true);

    }

    public override void PhysicsUpdate()
    { }

    public override void Exit()
    {
        character.roberryPathFinder.movePositionHouse.OnCompleteRobbir -= EndRobbie;
       // character.roberryPathFinder.movePositionHouse.marker.GetComponent<UITimer>().SetNewTime("", false);

    }
}
