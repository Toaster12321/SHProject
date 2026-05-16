using System;
using UnityEngine;

public class EnemyStateAttack : EnemyState
{
    private float _attackCooldown = 2f;//how long before damage is applied again
    private float _lastAttackTime;
    private float _rotationSpeed = 360f;

    public EnemyStateAttack( EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
        
    }

    public override void EnterState()
    {
        _lastAttackTime = 0f;
        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab)
        {
            Context.Agent.SetDestination(Context.SelfTransform.position);
        }
        Context.Animator.SetBool("attacking", true);
    }

    public override void ExitState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab)
            Context.Agent.isStopped = false;
        Context.Animator.SetBool("attacking", false);
    }

    public override void UpdateState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.CarnPlant && Context.IsRotatingEnabled)
        {
            Debug.Log(Context.IsRotatingEnabled);
            var lookPos = Context.Player.position - Context.SelfTransform.position; //vector from enemy to player
            lookPos.y = 0; //ignore vertical rotation, only moving along y axis
            var lookRotation = Quaternion.LookRotation(lookPos); //turns Vector3 -> Quaternion with rotation
            Context.SelfTransform.rotation = Quaternion.RotateTowards(Context.SelfTransform.rotation, lookRotation, _rotationSpeed * Time.deltaTime); //set rotation to rotate towards the player at set rotation speed 
        }
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {

        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab)
        {
            if (Context.PlayerInSightRange && !Context.PlayerInAttackRange) //if player enters vision radius -> chase
                return EnemyStateMachine.EEnemyState.Chase;
        }
        else if (Context.EnemyType == EnemyStateMachine.EnemyType.CarnPlant)
        {
            if (!Context.PlayerInAttackRange)
                return EnemyStateMachine.EEnemyState.Idle;
        }

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
