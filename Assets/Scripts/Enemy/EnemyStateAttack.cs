using System;
using UnityEngine;
using System.Collections;

public class EnemyStateAttack : EnemyState
{
    private float _attackCooldown = 2f;//how long before damage is applied again
    private float _lastAttackTime;
    private float _rotationSpeed = 150f;
    private float _rotationTolerance = 3f;
    private Quaternion targetRotation;

    //spider jump variables
    private float jumpHeight = 2f;
    private float jumpDisplacement = 7f;
    private float jumpDuration = 0.52f;

    private bool _jumpFinished = false;
    private bool _rotatedTowardsPlayer = false;
    private bool _hasntAttackedYet = true;

    private LayerMask obstacleLayerMask = LayerMask.GetMask("WhatIsWall", "Shrubbery"); //cant jump over walls or shrubbery
    public EnemyStateAttack( EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
    }

    public override void EnterState()
    {
        _lastAttackTime = 0f;

        _hasntAttackedYet = true; //first time entering the state, there have been no attacks
        _jumpFinished = false;
        _rotatedTowardsPlayer = false;

        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab)
        {
            Context.Agent.SetDestination(Context.SelfTransform.position);
            Context.Animator.SetBool("attacking", true);
        }
        
        if (Context.EnemyType == EnemyStateMachine.EnemyType.CarnPlant)
        {
            Context.IsRotatingEnabled = true;
            Context.Animator.SetBool("attacking", true);
        }

        if (Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
        {
            Context.EnemyStateMachine.StartCoroutine(JumpAttack());
            Context.Animator.SetBool("inAttack", true); //prevents switching back to idle via chase state
        }

    }

    public override void ExitState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab || Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider)
            Context.Agent.isStopped = false; //allow agent movement again

        Context.Animator.SetBool("attacking", false);
    }

    public override void UpdateState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.CarnPlant && Context.IsRotatingEnabled)
        {
            RotateTowardsPlayer();
        }

        if((Context.EnemyType == EnemyStateMachine.EnemyType.MushroomSpider && _jumpFinished) || (_hasntAttackedYet)) 
        {
            //if the jump is finished or its the 1st time attacking, rotate to the player facing him head on
            RotateTowardsPlayer();

            //if the angle between the player and spider is less than the rotation tolerance(3f) we have rotated towards the player
            float angleBetweenPlayer = Quaternion.Angle(Context.SelfTransform.rotation, targetRotation);
            if (angleBetweenPlayer <= _rotationTolerance)
                _rotatedTowardsPlayer = true;
            else
                _rotatedTowardsPlayer = false;
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
                if (_jumpFinished) //switch back to chase state after each jump attack
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

        if (Time.time >= _lastAttackTime + _attackCooldown) //if attack cooldown is over
        {
            Debug.Log("take dmg");
            Context.Player.GetComponentInParent<Player>().TakeDamage(1); //apply damage
            _lastAttackTime = Time.time;
        }


    }

    private void RotateTowardsPlayer()
    {
        var targetPos = Context.Player.position - Context.SelfTransform.position; //vector from enemy to player
        targetPos.y = 0; //ignore vertical rotation, only moving along y axis
        targetRotation = Quaternion.LookRotation(targetPos); //turns Vector3 -> Quaternion with rotation
        Context.SelfTransform.rotation = Quaternion.RotateTowards(Context.SelfTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime); //set rotation to rotate towards the player at set rotation speed 
    }

    private IEnumerator JumpAttack()
    {
        yield return new WaitUntil(() => _rotatedTowardsPlayer); //wait till spider has rotated towards player
        Context.Animator.SetBool("attacking", true); //start attacking animation 
        _hasntAttackedYet = false;

        Vector3 _startPos = Context.SelfTransform.position;
        Vector3 _endPos = _startPos + Context.SelfTransform.forward * jumpDisplacement; //end pos is the current position + the z-forward direction * the total displacement we want it to jump to

        Vector3 capsuleBot = _startPos + Vector3.up * 0.2f; //capsule cast sizes
        Vector3 capsuleTop = _startPos + Vector3.up * 1.9f;

        Vector3 direction = (_endPos - _startPos).normalized; //get the current direction the spider is facing
        float totalDistance = Vector3.Distance(_startPos, _endPos);

        //CapsuleCast aroudn the spider to check if it hit an obstacle in its radius for the whole distance of the jump
        if (Physics.CapsuleCast(capsuleBot, capsuleTop, Context.Agent.radius, direction, out RaycastHit hitInfo, totalDistance, obstacleLayerMask))
        {
            _endPos = hitInfo.point - direction * 0.5f; //if an objstacle is hit change the end position to half of the area where the collision occured
        }

        float elapsedTime = 0f;
        Context.Agent.isStopped = true; //stop all agent functions
        Context.Agent.updatePosition = false;
        Context.Agent.updateRotation = false;

        float _timeTillJump = 0.5f;
        yield return new WaitForSeconds(_timeTillJump); 
        
        while (elapsedTime < jumpDuration) //during the jump duration
        {
            elapsedTime += Time.deltaTime;
            float currentTime = elapsedTime / jumpDuration; //convert time to a 0-100% percentage

            Vector3 _pos = Vector3.Lerp(_startPos, _endPos, currentTime); //move in a direction from start to end during the time window

            _pos.y += Mathf.Sin(currentTime * MathF.PI) * jumpHeight; //sin creates a wave to simulate jumping, PI = 180degrees, so from 0 to 180degrees in a wave we are moving in the y axis

            Context.SelfTransform.position = _pos; //set spider position to the lerp

            yield return null; //dont move on till jump is over
        }

        Context.SelfTransform.position = _endPos; //set agent and current position to end location
        Context.Agent.Warp(_endPos);

        Context.Agent.updatePosition = true; //resume agent functions
        Context.Agent.updateRotation = true;
        Context.Agent.isStopped = false;

        _jumpFinished = true;
        Context.Animator.SetBool("inAttack", false);
    }

}
