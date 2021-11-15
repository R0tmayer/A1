using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturningState : StateSecurity
{
    public ReturningState(SecurityController character, StateMachineSecurity stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter()
    {
        character.tartgetHouse._house.securityProtected = false;
      //  MainPlayer.Instance.PoliceCarState = "Возвращаюсь на базу";

        character.Escape();
        character.tartgetHouse._house.security = null;

    }

    public override void HandleInput()
    { }

    public override void LogicUpdate()
    { }

    public override void PhysicsUpdate()
    { }

    public override void Exit()
    {

    }
}
