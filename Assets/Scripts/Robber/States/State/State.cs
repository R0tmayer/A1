using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    protected RobberController character;
    protected StateMachine stateMachine;

    protected State(RobberController character, StateMachine stateMachine)
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