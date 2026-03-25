using UnityEngine;

public class EnemyStatePatrol : EnemyState
{
    private Vector3 _walkPoint;
    private bool _walkPointSet = false; 
    public EnemyStatePatrol(EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
        
    }

    public override void EnterState()
    {
        Context.Animator.SetBool("walking", true); //show walking and resume movement
        Context.Agent.isStopped = false;

        _walkPointSet = false; //always enter state with a fresh path
    }

    public override void ExitState()
    {
        Context.Agent.ResetPath(); //stop old pathing
    }

    public override void UpdateState()
    {
        if (!_walkPointSet) //search for a walk point if one is not set
            SearchWalkPoint();
         
        if (_walkPointSet) //once set send agent to that spot
            Context.Agent.SetDestination(_walkPoint);

    }   

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (Context.PlayerInSightRange) //if player enters vision radius -> chase
            return EnemyStateMachine.EEnemyState.Chase;

        if (Context.PlayerInAttackRange) //if player enters attack radius -> attack
            return EnemyStateMachine.EEnemyState.Attack;

        if (_walkPointSet)
        {
            Vector3 distanceToWalkPoint = Context.Agent.transform.position - _walkPoint; //distance from current spot to walk point

            if (distanceToWalkPoint.magnitude < 1f)//if we reached the walkpoint -> idle
            {
                return EnemyStateMachine.EEnemyState.Idle;
            }
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

    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-Context.WalkPointRange, Context.WalkPointRange); //random range between our two walk points for the enemy to patrol in X and Z axis
        float randomX = Random.Range(-Context.WalkPointRange, Context.WalkPointRange);

        _walkPoint = new Vector3(Context.Agent.transform.position.x + randomX, Context.Agent.transform.position.y, Context.Agent.transform.position.z + randomZ); //set current position on x and z to our random values

        if (Physics.Raycast(_walkPoint, -Context.Agent.transform.up, 2f, Context.WhatIsGround)) //make sure our walkpoint is on the ground and not off the map
            _walkPointSet = true;  //walk point is set
    }
}
