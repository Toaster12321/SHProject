using UnityEngine;

public class EnemyStateAttack : EnemyState
{
    public EnemyStateAttack( EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
        
    }

    public override void EnterState()
    {
        Context.Animator.SetBool("attacking", true);
        Context.Agent.SetDestination(Context.SelfTransform.position); //make sure enemy doesn't move
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (Context.PlayerInSightRange && !Context.PlayerInAttackRange) //if player enters vision radius -> chase
            return EnemyStateMachine.EEnemyState.Chase;

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
