using UnityEngine;

public class EnemyStateDie : EnemyState
{
    public EnemyStateDie(EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
       
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {

    }

    public override void OnTriggerStay(Collider other)
    {

    }
}
