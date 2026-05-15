using UnityEngine;
using UnityEngine.AI;

public class EnemyStateContext
{
    private EnemyStateMachine.EnemyType _enemyType;
    private Material _enemyRenderer;
    private Color _originalColor;
    private NavMeshAgent _agent; //navmesh agent reference
    private Transform _player; //player position reference
    private LayerMask _whatIsGround, _whatIsPlayer; //layers for ground and player
    private Animator _animator; //animation reference
    private ParticleSystem _particleEmitter;
    private float _sightRange, _attackRange, _walkPointRange;
    private Transform _selfTransform;
    private Collider _attackHitbox;


    public EnemyStateContext(EnemyStateMachine.EnemyType enemyType, Material enemyRenderer, Color originalColor, NavMeshAgent agent, Transform player, LayerMask whatIsGround, Collider attackHitbox,
        LayerMask whatIsPlayer, Animator animator, ParticleSystem particleEmitter, float sightRange, float attackRange, float walkPointRange, Transform selfTransform)
    {
        _enemyType = enemyType;
        _enemyRenderer = enemyRenderer;
        _originalColor = originalColor;
        _agent = agent;
        _player = player;
        _whatIsGround = whatIsGround;
        _whatIsPlayer = whatIsPlayer;
        _attackHitbox = attackHitbox;
        _animator = animator;
        _particleEmitter = particleEmitter;
        _sightRange = sightRange;
        _attackRange = attackRange;
        _walkPointRange = walkPointRange;
        _selfTransform = selfTransform;
    }

    public EnemyStateMachine.EnemyType EnemyType => _enemyType;
    public Material EnemyRenderer => _enemyRenderer;
    public Color OriginalColor => _originalColor;
    public NavMeshAgent Agent => _agent;
    public Transform Player => _player;
    public Animator Animator => _animator;
    public ParticleSystem ParticleEmitter => _particleEmitter;
    public LayerMask WhatIsGround => _whatIsGround;
    public LayerMask WhatIsPlayer => _whatIsPlayer;
    public float SightRange => _sightRange;
    public float AttackRange => _attackRange;
    public float WalkPointRange => _walkPointRange;
    public Transform SelfTransform => _selfTransform;
    public Collider AttackHitbox => _attackHitbox;

    public bool PlayerInSightRange => //checks for the position of the player in a radius of sight range on layer mask of player
        Physics.CheckSphere(SelfTransform.position, _sightRange, _whatIsPlayer);  

    public bool PlayerInAttackRange => //checks for the position of the player in a radius of attack range on layer mask of player
        Physics.CheckSphere(SelfTransform.position, _attackRange, _whatIsPlayer);
}
