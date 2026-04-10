using System;
using UnityEngine;

public class EnemyStateAttack : EnemyState
{
    private float _attackCooldown = 1.7f;//how long before damage is applied again
    private float _attackStartTime;
    private float _lastAttackTime;
    private bool _alreadyAttacked;
    private float _attackRange = 1f;
    private float _attackDelay = 0.9f;
    private LayerMask _layerMask;

    public EnemyStateAttack( EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
        
    }

    public override void EnterState()
    {
        _attackStartTime = Time.time;
        _lastAttackTime = 0f;
        _layerMask = Context.WhatIsGround | Context.WhatIsPlayer;

        _alreadyAttacked = false;
        Context.Animator.SetBool("attacking", true);
        Context.Agent.SetDestination(Context.SelfTransform.position); //make sure enemy doesn't move
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        if (_alreadyAttacked == false && Time.time >= _attackStartTime + _attackDelay) //if the time passed if 2 seconds past the last attack time and we havent attacked already
        {
            AttackRaycast();
            _lastAttackTime = Time.time;
            _alreadyAttacked = true;

        }

        if (_alreadyAttacked && Time.time >= _lastAttackTime + _attackCooldown)
        {
            _attackStartTime = Time.time;
            _alreadyAttacked = false;
        }

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

    public void AttackRaycast()
    {
        Vector3 rayOrigin = Context.Agent.transform.position + Vector3.up * 1.5f; //move raycast up some

        Ray attackRay = new Ray(rayOrigin, Context.Agent.transform.forward); //raycast pointing from camera forwards
        if (Physics.Raycast(attackRay, out RaycastHit hitinfo, _attackRange, _layerMask)) //if the raycast collides
        {
            if (hitinfo.collider.GetComponentInParent<Player>()) //if we hit an object with the player script
            {
                Context.Player.GetComponentInParent<Player>().TakeDamage(1); //apply damage
            }
        }
    }
}
