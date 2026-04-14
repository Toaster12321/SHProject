using System;
using UnityEngine;

public class EnemyStateAttack : EnemyState
{
    private float _attackCooldown = 2f;//how long before damage is applied again
    private float _lastAttackTime;

    public EnemyStateAttack( EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
        
    }

    public override void EnterState()
    {
        _lastAttackTime = 0f;

        Context.Animator.SetBool("attacking", true);
        Context.Agent.SetDestination(Context.SelfTransform.position);
    }

    public override void ExitState()
    {
        Context.Agent.isStopped = false;
        Context.Animator.SetBool("attacking", false);
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
        
        Player _player = other.GetComponent<Player>();

        if (_player == null) //do nothing if not player collider
            return;

        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            Context.Player.GetComponentInParent<Player>().TakeDamage(1); //apply damage
            _lastAttackTime = Time.time;
        }

    }

}
