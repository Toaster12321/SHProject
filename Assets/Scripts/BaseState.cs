using System;
using UnityEngine;

//abstract template for all states in state machine
public abstract class BaseState<Estate> where Estate : Enum //Estate is a generic of type enum
    // all states will have these methods
{
    public BaseState(Estate state) //constructor to get/set StateKey as type Estate
    {
        state = StateKey;
    }
    public Estate StateKey { get; private set; }
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract Estate GetNextState(); //get next state returns a type: Estate
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerStay(Collider other);
    public abstract void OnTriggerExit(Collider other);
}
