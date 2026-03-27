using UnityEngine;

public class EnemyStateAttack : EnemyState
{
    private float _attackCooldown;
    private float _lastAttackTime;
    private bool _alreadyAttacked;
    public EnemyStateAttack( EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
        
    }

    public override void EnterState()
    {
        _alreadyAttacked = false;
        _attackCooldown = 2f; //how long before damage is applied again
        Context.Animator.SetBool("attacking", true);
        Context.Agent.SetDestination(Context.SelfTransform.position); //make sure enemy doesn't move
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        if (Time.time >= _lastAttackTime + _attackCooldown && _alreadyAttacked == false) //if the time passed if 2 seconds past the last attack time and we havent attacked already
        {
            Context.Player.GetComponent<Player>().TakeDamage(1f);
            _lastAttackTime = Time.time;
            _alreadyAttacked = true;
        }
        else //otherwise cooldown is still active
            _alreadyAttacked = false;

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
