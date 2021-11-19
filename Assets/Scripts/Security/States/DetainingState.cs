using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetainingState : StateSecurity
{
    float time;
    float timer;

    int prevTime;
    List<string> variant = new List<string>() { "А1 на месте. До задержания " };
    int q ;
    public DetainingState(SecurityController character, StateMachineSecurity stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        character.isStoped = true;
        // MainPlayer.Instance.PoliceCarState = "Делаю арес";
        character.tartgetHouse._house.OnCompleteRobbir += EndRobbie;
        timer = character.timeToArest;
        //Debug.Log("задержания случится через " + (timer));

        q = Random.Range(0, variant.Count);
        character.Inst(variant[q] + Mathf.Round(timer - time) + " сек");;
    }

    public override void HandleInput()
    { }

    public override void LogicUpdate()
    {
        time += Time.deltaTime;
        
        if (prevTime != (int)time)
        {
/*            character.DeleteInst();*/
            character.ChangeInst(variant[q] + Mathf.Round(timer - time) + " сек");
        }

        prevTime = (int)time;
        if (time > timer) {
/*            Debug.Log(time);
            Debug.Log(timer);*/
            character.tartgetHouse._house.IsBeingProtected();
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
