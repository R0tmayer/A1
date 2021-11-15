using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityMovingState : StateSecurity
{
    public SecurityMovingState(SecurityController character, StateMachineSecurity stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
       // MainPlayer.Instance.PoliceCarState = "Еду за грабителем";
        character.tartgetHouse._house.OnCompleteRobbir += EndRobbie;
        character._radioLineRenderer.enabled = false;
    }

    public override void HandleInput()
    { }

    public void EndRobbie()
    {
        stateMachine.ChangeState(character.returning);
    }

    public override void LogicUpdate()
    {

        if (!character.tartgetHouse._house.rob && !character.tartgetHouse._house.going_to_rob)
        {
            stateMachine.ChangeState(character.returning);
        }

        if (character.GetDistanceToTarget()) {
            character.pathMover.StopMove();
            stateMachine.ChangeState(character.waiting);
        }

  
        
    }

    public override void PhysicsUpdate()
    { }

    public override void Exit()
    {
        character.tartgetHouse._house.OnCompleteRobbir -= EndRobbie;
    }
}
