using UnityEngine;

public class EnemyStateChase : EnemyState
{
    public EnemyStateChase(EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
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
