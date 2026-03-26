using UnityEngine;

public class EnemyStateChase : EnemyState
{
    private float _aggroTimer;
    private float _aggroDuration = 3f;
    public EnemyStateChase(EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {

    }

    public override void EnterState()
    {
        _aggroTimer = 0f;

        Context.Agent.isStopped = false;
        if (Context.Animator.GetBool("attacking"))
            Context.Animator.SetBool("attacking", false);

        Context.Animator.SetBool("walking", true);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        _aggroTimer += Time.deltaTime;

        Context.Agent.SetDestination(Context.Player.position);
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (Context.PlayerInAttackRange) //if player enters attack radius -> attack
            return EnemyStateMachine.EEnemyState.Attack;

        if (_aggroTimer >= _aggroDuration) //once timer is over start idling
            return EnemyStateMachine.EEnemyState.Idle;

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
