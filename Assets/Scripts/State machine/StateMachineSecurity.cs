public class StateMachineSecurity
{
    public StateSecurity CurrentState { get; private set; }

    public void Initialize(StateSecurity startingState)
    {
        
        CurrentState = startingState;
        startingState.Enter();
    }

    public void ChangeState(StateSecurity newState)
    {
        CurrentState.Exit();

        CurrentState = newState;
        newState.Enter();
    }
}

