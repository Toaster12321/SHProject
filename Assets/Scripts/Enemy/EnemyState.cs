using UnityEngine;

public abstract class EnemyState : BaseState<EnemyStateMachine.EEnemyState>
{
    protected EnemyStateContext Context;

    public EnemyState(EnemyStateContext context, EnemyStateMachine.EEnemyState stateKey) : base(stateKey)
    {
        Context = context;
    }
}
