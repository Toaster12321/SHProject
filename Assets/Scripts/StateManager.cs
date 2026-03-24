using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<Estate> : MonoBehaviour where Estate : Enum
{
    //protected because only StateManager and its inherited classes need to access it
    protected Dictionary<Estate, BaseState<Estate>> States = new Dictionary<Estate, BaseState<Estate>>(); //new dictionary instance with a key of Estate(enum) and a value of Basestate's states
    protected BaseState<Estate> CurrentState;

    protected bool IsTransitioningState = false;

    void Start()
    {
        CurrentState.EnterState();
    }
    void Update()
    {
        Estate nextState = CurrentState.GetNextState();

        if (!IsTransitioningState && nextState.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        }
        else
        {
            TransitionToState(nextState);
        }
    }

    public void TransitionToState(Estate StateKey) //function to exit a state, register what the current state is then enter a new one
    {
        IsTransitioningState = true;
        CurrentState.ExitState();       
        CurrentState = States[StateKey];
        CurrentState.EnterState();
        IsTransitioningState= false;
    }
    void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }
    void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }
    private void OnTriggerExit(Collider other)
    {
        CurrentState.OnTriggerExit(other);
    }


}
