using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateSecurity 
{
    protected SecurityController character;
    protected StateMachineSecurity stateMachine;

    protected StateSecurity(SecurityController character, StateMachineSecurity stateMachine)
    {
        this.character = character;
        this.stateMachine = stateMachine;
    }


    public virtual void Enter()
    { }

    public virtual void HandleInput()
    { }

    public virtual void LogicUpdate()
    { }

    public virtual void PhysicsUpdate()
    { }

    public virtual void Exit()
    { }

}