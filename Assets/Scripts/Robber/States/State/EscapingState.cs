using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapingState : State
{
    public EscapingState(RobberController character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }
    public override void Enter()
    {
        character.roberryPathFinder.movePositionHouse.EndRobbery();
        character.Escape();
    }

    public override void HandleInput()
    { }

    public override void LogicUpdate()
    { }

    public override void PhysicsUpdate()
    { }

    public override void Exit()
    { }
}
