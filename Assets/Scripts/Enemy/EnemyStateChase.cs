using UnityEngine;

public class EnemyStateChase : EnemyState
{
    private float _aggroTimer;
    private float _aggroDuration = 3f;
    private bool _returningToStart = false;
    public EnemyStateChase(EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {

    }

    public override void EnterState()
    {
        if (!Context.PatrolStartingPositionSet)
        {
            Context.PatrolStartingPosition = Context.Agent.transform.position;
            Context.PatrolStartingPositionSet = true;
        }

        _aggroTimer = 0f;
        _returningToStart = false;

        Context.Agent.isStopped = false;
        if (Context.Animator.GetBool("attacking"))
            Context.Animator.SetBool("attacking", false);

        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab)
            Context.Animator.SetBool("walking", true);

        if (Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
        {
            Context.Animator.SetBool("chasing", true);
            Context.Animator.SetBool("walking", false);
        }
            
    }

    public override void ExitState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
            Context.Animator.SetBool("chasing", false);
    }

    public override void UpdateState()
    {

        float distanceFromStart = Vector3.Distance(Context.Agent.transform.position, Context.PatrolStartingPosition);
        if (distanceFromStart > Context.MaxPatrolRadius)
        {
            Context.Agent.SetDestination(Context.PatrolStartingPosition);
            _returningToStart = true;
            return;
        }

        if(_returningToStart)
        {
            Context.Agent.SetDestination(Context.PatrolStartingPosition); //set location again to prevent chase state

            if (distanceFromStart <= Context.HomeRadius)
            {
                _returningToStart = false;
            }

            return;
        }

        _aggroTimer += Time.deltaTime;

        Context.Agent.SetDestination(Context.Player.position);
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (_returningToStart) //prevent transitions until home radius is reached
            return StateKey;

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
