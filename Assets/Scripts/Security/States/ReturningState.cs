using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturningState : StateSecurity
{
    FactoriesSecurity factoriesSecurity = TargetsManager.Instance.factoriesSecurity;
    bool stoped;

    public ReturningState(SecurityController character, StateMachineSecurity stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter()
    {
        character.tartgetHouse._house.securityProtected = false;
      //  MainPlayer.Instance.PoliceCarState = "Возвращаюсь на базу";

        
       
        //character.tartgetHouse._house.security = null;
        character.pathMover.StartMove();
        character.pathMover._navMeshAgent.destination = factoriesSecurity._spawnPoint.transform.position;
        //stateMachine.ChangeState(character.moving);
    }

    public override void HandleInput()
    { }

    public override void LogicUpdate()
    { }

    public override void PhysicsUpdate()
    {
        if (Vector3.Distance(factoriesSecurity._spawnPoint.transform.position, character.transform.position) < 1f && !stoped)
        {
            stoped = true;
            character.pathMover.StopMove();
            character.Escape();

        }

    }

   // character.Escape();


    public override void Exit()
    {

    }
}
