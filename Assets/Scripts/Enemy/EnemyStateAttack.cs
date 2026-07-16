using System;
using UnityEngine;
using System.Collections;

public class EnemyStateAttack : EnemyState
{
    private float _attackCooldown = 2f;//how long before damage is applied again
    private float _lastAttackTime;
    private float _rotationSpeed = 150f;

    private float jumpHeight = 1.2f;
    private float jumpDisplacement = 7f;
    private float jumpDuration = 0.8f;

    private bool _jumpFinished = false;
    public EnemyStateAttack( EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
        
    }

    public override void EnterState()
    {
        _lastAttackTime = 0f;
        Context.Animator.SetBool("attacking", true);
        _jumpFinished = false;

        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab)
        {
            Context.Agent.SetDestination(Context.SelfTransform.position);
        }
        
        if (Context.EnemyType == EnemyStateMachine.EnemyType.CarnPlant)
        {
            Context.IsRotatingEnabled = true;
        }

        if (Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
        {
            Context.EnemyStateMachine.StartCoroutine(JumpAttack());
        }

    }

    public override void ExitState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab || Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
            Context.Agent.isStopped = false;
        Context.Animator.SetBool("attacking", false);
    }

    public override void UpdateState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.CarnPlant && Context.IsRotatingEnabled)
        {
            var lookPos = Context.Player.position - Context.SelfTransform.position; //vector from enemy to player
            lookPos.y = 0; //ignore vertical rotation, only moving along y axis
            var lookRotation = Quaternion.LookRotation(lookPos); //turns Vector3 -> Quaternion with rotation
            Context.SelfTransform.rotation = Quaternion.RotateTowards(Context.SelfTransform.rotation, lookRotation, _rotationSpeed * Time.deltaTime); //set rotation to rotate towards the player at set rotation speed 
        }

    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {

        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab || Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
        {
            if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab)
                if (Context.PlayerInSightRange && !Context.PlayerInAttackRange) //if player enters vision radius -> chase
                    return EnemyStateMachine.EEnemyState.Chase;


            if (Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
                if (Context.PlayerInSightRange && !Context.PlayerInAttackRange && _jumpFinished) //if player enters vision radius -> chase
                    return EnemyStateMachine.EEnemyState.Chase;
                else if (!Context.PlayerInSightRange && !Context.PlayerInAttackRange && _jumpFinished)
                    return EnemyStateMachine.EEnemyState.Patrol;

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
            Debug.Log("take dmg");
            Context.Player.GetComponentInParent<Player>().TakeDamage(1); //apply damage
            _lastAttackTime = Time.time;
        }


    }

    private IEnumerator JumpAttack()
    {
        Vector3 _startPos = Context.SelfTransform.position;
        Vector3 _endPos = _startPos + Context.SelfTransform.forward * jumpDisplacement;

        float elapsedTime = 0f;
        Context.Agent.isStopped = true;
        Context.Agent.updatePosition = false;
        Context.Agent.updateRotation = false;

        yield return new WaitForSeconds(0.5f);
        
        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentTime = elapsedTime / jumpDuration;

            Vector3 _pos = Vector3.Lerp(_startPos, _endPos, currentTime); //forward movement

            _pos.y += Mathf.Sin(currentTime * MathF.PI) * jumpHeight;

            Context.SelfTransform.position = _pos;

            yield return null;
        }

        Context.SelfTransform.position = _endPos;
        Context.Agent.Warp(_endPos);

        Context.Agent.updatePosition = true;
        Context.Agent.updateRotation = true;
        Context.Agent.isStopped = false;

        _jumpFinished = true;

    }

}
