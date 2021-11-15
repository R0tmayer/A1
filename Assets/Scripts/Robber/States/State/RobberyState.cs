using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberyState : State
{
    public RobberyState(RobberController character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        
        
        character.roberryPathFinder.movePositionHouse.rob = true;
        character.StartCoroutineTMP(Wringer());
        character.roberryPathFinder.movePositionHouse.OnCompleteRobbir += EndRobbie;
    }


    IEnumerator Wringer() {

        while (character.roberryPathFinder.movePositionHouse.Property > 0)
        {
            character.roberryPathFinder.movePositionHouse.marker.GetComponent<UITimer>().SetNewTime(""+character.roberryPathFinder.movePositionHouse.Property / (character.roberryPathFinder.movePositionHouse.upg_zabor_or_signalization ? character.factorPropertyperSecodn / 1.5f : character.factorPropertyperSecodn), true);
            yield return new WaitForSeconds(1);
            if (character.roberryPathFinder.movePositionHouse.Property - character.factorPropertyperSecodn <= 0)
            {
                character.StopCoroutineTMP();
            }

            character.roberryPathFinder.movePositionHouse.Property = character.roberryPathFinder.movePositionHouse.upg_zabor_or_signalization ? character.factorPropertyperSecodn / 1.5f : character.factorPropertyperSecodn; 
        }
    }


    public void EndRobbie()
    {
        stateMachine.ChangeState(character.escaping);
    }

    public override void HandleInput()
    { 
    }

    public override void LogicUpdate()
    {
    }

    public override void PhysicsUpdate()
    { }

    public override void Exit()
    {
        character.roberryPathFinder.movePositionHouse.OnCompleteRobbir -= EndRobbie;
        character.roberryPathFinder.movePositionHouse.marker.GetComponent<UITimer>().SetNewTime("", false);


    }
}
