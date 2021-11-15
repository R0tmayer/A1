using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateSecurity
{
    public IdleState(SecurityController character, StateMachineSecurity stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        //MainPlayer.Instance.PoliceCarState = "Жду маршрут";
    }

    public override void HandleInput()
    { }

    public override void LogicUpdate()
    {
        if (character.pathCreator._isMove)
        {
            character.targetTransform = character.tartgetHouse.transform.position;
            character.tartgetHouse._house.security = character;
            stateMachine.ChangeState(character.moving);
        }
    }

    public override void PhysicsUpdate()
    { }

    public override void Exit()
    { }
}
