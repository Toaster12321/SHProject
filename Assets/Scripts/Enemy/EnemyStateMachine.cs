using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Timeline.TimelinePlaybackControls;
public class EnemyStateMachine : StateManager<EnemyStateMachine.EEnemyState>
{
    public enum EEnemyState
    {
        Patrol,
        Idle,
        Chase,
        Attack,
        Die,
    }

    public enum EnemyType
    {
        Scab,
        CarnPlant
    }

    private EnemyStateContext _context;
    [SerializeField] private float maxHP;
    private float currentHP;
    private float _invulnDuration = 0.2f;
    private float _lastTimeHit;
    public bool isDead {  get; private set; }

    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private Material _enemyRenderer;
    [SerializeField] private NavMeshAgent _agent; //navmesh agent reference
    [SerializeField] private Transform _player; //player position reference
    [SerializeField] private LayerMask _whatIsGround, _whatIsPlayer; //layers for ground and player
    [SerializeField] private Animator _animator; //animation reference
    [SerializeField] private ParticleSystem _particleEmitter;
    [SerializeField] private float _sightRange, _attackRange, _walkPointRange;
    [SerializeField] private Transform _selfTransform;
    [SerializeField] private Collider _attackHitbox;
    private bool _isRotatingEnabled = true;
    private Color _originalColor;



    private void Start()
    {
        _originalColor = _enemyRenderer.color;
        currentHP = maxHP; //make sure enemy starts with max HP
    }

    public void TakeDamage(float amount)
    { 

        if (isDead) return;

        if (Time.time < _lastTimeHit + _invulnDuration) //apply invulnerability for 0.2s to prevent multiple instances of damage
            return;

        currentHP -= amount;
        _lastTimeHit = Time.time;

        if (_enemyRenderer != null)
            StartCoroutine(FlashRed());
        if (_enemyType == EnemyType.Scab)
            TransitionToState(EEnemyState.Chase);

        if (currentHP <= 0)
        {
            isDead = true;
            TransitionToState(EEnemyState.Die);
        }
    }

    private void Awake()
    {
        _context = new EnemyStateContext(_enemyType, _enemyRenderer, _originalColor, _agent, _player, _whatIsGround, _attackHitbox, _whatIsPlayer, _animator, _particleEmitter, _sightRange,
            _attackRange, _walkPointRange, _selfTransform, _isRotatingEnabled);
        InitializeStates();
    }

    private void InitializeStates()
    {
        States.Add(EEnemyState.Idle, new EnemyStateIdle(_context, EEnemyState.Idle));
        States.Add(EEnemyState.Patrol, new EnemyStatePatrol(_context, EEnemyState.Patrol));
        States.Add(EEnemyState.Chase, new EnemyStateChase(_context, EEnemyState.Chase));
        States.Add(EEnemyState.Attack, new EnemyStateAttack(_context, EEnemyState.Attack));
        States.Add(EEnemyState.Die, new EnemyStateDie(_context, EEnemyState.Die));
        CurrentState = States[EEnemyState.Idle];
    }

    IEnumerator FlashRed() //coroutine to set red flash as indicator for damage for 0.1s
    {
        _enemyRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _enemyRenderer.color = _originalColor;
    }

    public void EnableAttackHitbox()//ANIM EVENT
    {
        _attackHitbox.enabled = true;
    }

    public void DisableAttackHitbox()//ANIM EVENT
    {
        _attackHitbox.enabled = false;
    }

    public void DisableRotation() //ANIM EVENT used for stopping rotation of carn plant when attacking
    {
        _context.IsRotatingEnabled = false;
    }

    public void EnableRotation()//ANIM EVENT
    {
        _context.IsRotatingEnabled = true;
    }

    private void DestroyEnemy() //ANIM EVENT
    {
        GameObject.Destroy(gameObject);
    }
}
