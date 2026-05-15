using GLTFast.Schema;
using UnityEngine;

public class EnemyStateDie : EnemyState
{
    private float _timer;
    private float _tickRate;
    private float _gasDuration = 3.1f;
    private float _explosionAnimationTime;
    private float _totalAnimationTime;
    private bool _explosionStarted = false;
    public EnemyStateDie(EnemyStateContext context, EnemyStateMachine.EEnemyState estate) : base(context, estate)
    {
       
    }

    public override void EnterState()
    {
        Context.Animator.SetTrigger("no_hp");
        Context.Agent.GetComponent<BoxCollider>().enabled = false;

        Context.Agent.isStopped = true; //turn off navmesh functions
        Context.Agent.ResetPath();
        Context.Agent.enabled = false;

        _timer = 0f;
        _tickRate = 0f;
        _explosionAnimationTime = 1.25f;
        _totalAnimationTime = _explosionAnimationTime + _gasDuration;
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        if (Context.EnemyType == EnemyStateMachine.EnemyType.Scab)
        {
            _timer += Time.deltaTime;

            if (_timer >= _explosionAnimationTime && !_explosionStarted) //wait for explosion animation to finish
            {
                Context.ParticleEmitter.Play(); //play gas explosion effect and turn off sprite
                Context.Agent.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                _explosionStarted = true;
            }

            if (_explosionStarted && _timer <= _totalAnimationTime) //if explosion has started and the gas duration is still active -> do damage
            {
                _tickRate += Time.deltaTime;

                if (_tickRate >= 1f) //every 1 seconds call explosion for DOT
                {
                    Explosion();
                    _tickRate = 0f;
                }
            }

            if (_timer >= _totalAnimationTime) //if the timer is over the gas duration destroy object
            {
                GameObject.Destroy(Context.Agent.gameObject);
            }
        }
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
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

    private void Explosion()
    {
        bool playerDamaged = false;
         
        Collider[] colliders = Physics.OverlapSphere(Context.ParticleEmitter.transform.position, 3f); //physics collision sphere with a radius of 5f

        foreach (Collider c in colliders) 
        {
            if (c.GetComponent<Player>() && !playerDamaged)//if a collider was of type Player and they haven't been damaged -> apply 1f damage
            {
                playerDamaged = true;
                c.GetComponent<Player>().TakeDamage(1f);
            }

        }
    }
}
