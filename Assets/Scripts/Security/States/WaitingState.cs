using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingState : StateSecurity
{
    public WaitingState(SecurityController character, StateMachineSecurity stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter()
    {
        character.isStoped = true;
       // MainPlayer.Instance.PoliceCarState = "Ожидаем грабителя";
        character.tartgetHouse._house.OnCompleteRobbir += EndRobbie;

        if (character.tartgetHouse != null)
        {
            if (!character.tartgetHouse._house.rob && !character.tartgetHouse._house.going_to_rob)
            {
                stateMachine.ChangeState(character.returning);
            }
        }

        List<string> variant = new List<string>() { "А1 на месте" };

        int q = Random.Range(0, variant.Count);
        character.Inst(variant[q]);

    }

    public override void HandleInput()
    { }

    public override void LogicUpdate()
    {
        if (character.tartgetHouse != null && character.tartgetHouse._house.rob)
        {
            stateMachine.ChangeState(character.detaining);
        }


    }

    public override void PhysicsUpdate()
    { }
    public void EndRobbie()
    {
        stateMachine.ChangeState(character.returning);
    }
    public override void Exit()
    {
        character.tartgetHouse._house.OnCompleteRobbir -= EndRobbie;
        character.DeleteInst();
    }

    
}
