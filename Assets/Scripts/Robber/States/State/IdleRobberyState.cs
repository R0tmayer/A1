using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRobberyState : State
{
    public IdleRobberyState(RobberController character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
    }

    public override void HandleInput()
    { }

    public override void LogicUpdate()
    {
        if (character.roberryPathFinder.movePositionHouse != null) {
            stateMachine.ChangeState(character.moving);
        }
    }

    public override void PhysicsUpdate()
    { }

    public override void Exit()
    { }
}
