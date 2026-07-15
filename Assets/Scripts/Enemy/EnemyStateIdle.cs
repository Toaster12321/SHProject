using UnityEngine;

public class EnemyStateIdle : EnemyState
{
    private float _idleTimer;
    private float _idleDuration = 4.5f;
    public EnemyStateIdle(EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
        
    }

    public override void EnterState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab)
        {
            _idleTimer = 0f;

            Context.Animator.SetBool("walking", false); //set walking to false in case of entering idle from attack
            Context.Agent.isStopped = true; //stop moving
        }
        if (Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
        {
            _idleTimer = 0f;

            Context.Animator.SetBool("walking", false); //set walking to false in case of entering idle from attack or chase
            Context.Animator.SetBool("chasing", false); 
            Context.Agent.isStopped = true; //stop moving
        }
    }

    public override void ExitState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab || Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
            Context.Agent.isStopped = false; //resume movement
    }

    public override void UpdateState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab || Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
            _idleTimer += Time.deltaTime; //start timer
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab || Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
        {
            if (Context.PlayerInSightRange) //if player enters vision radius -> chase
                return EnemyStateMachine.EEnemyState.Chase;

            if (Context.PlayerInAttackRange) //if player enters attack radius -> attack
                return EnemyStateMachine.EEnemyState.Attack;

            if (_idleTimer >= _idleDuration) //once timer is over start patrolling
                return EnemyStateMachine.EEnemyState.Patrol;
        }
        else if (Context.EnemyType == EnemyStateMachine.EnemyType.CarnPlant)
        {
            if (Context.PlayerInAttackRange) //if player enters attack radius -> attack
                return EnemyStateMachine.EEnemyState.Attack;
        }

        return StateKey; //returns self
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
