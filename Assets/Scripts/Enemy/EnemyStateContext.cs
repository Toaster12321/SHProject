using UnityEngine;
using UnityEngine.AI;

public class EnemyStateContext : MonoBehaviour
{
    private SkinnedMeshRenderer _enemyRenderer;
    private Color _originalColor;
    private NavMeshAgent _agent; //navmesh agent reference
    private Transform _player; //player position reference
    private LayerMask _whatIsGround, _whatIsPlayer; //layers for ground and player
    private Animator _animator; //animaton reference
    private ParticleSystem _particleEmittor;
    private float _sightRange, _attackRange, _walkPointRange;


    public EnemyStateContext(SkinnedMeshRenderer enemyRenderer, Color originalColor, NavMeshAgent agent, Transform player, LayerMask whatIsGround,
        LayerMask whatIsPlayer, Animator animator, ParticleSystem particleEmittor, float sightRange, float attackRange, float walkPointRange)
    {
        _enemyRenderer = enemyRenderer;
        _originalColor = originalColor;
        _agent = agent;
        _player = player;
        _whatIsGround = whatIsGround;
        _whatIsPlayer = whatIsPlayer;
        _animator = animator;
        _particleEmittor = particleEmittor;
        _sightRange = sightRange;
        _attackRange = attackRange;
        _walkPointRange = walkPointRange;
    }

    public SkinnedMeshRenderer EnemyRenderer => _enemyRenderer;
    public Color OriginalColor => _enemyRenderer.material.color;
    public NavMeshAgent Agent => _agent;
    public Transform Player => _player;
    public Animator Animator => _animator;
    public ParticleSystem ParticleEmittor => _particleEmittor;
    public LayerMask WhatIsGround => _whatIsGround;
    public LayerMask WhatIsPlayer => _whatIsPlayer;
    public float SightRange => _sightRange;
    public float AttackRange => _attackRange;
    public float WalkPointRange => _walkPointRange;


    public bool PlayerInSightRange => //checks for the position of the player in a radius of sight range on layer mask of player
        Physics.CheckSphere(transform.position, _sightRange, _whatIsPlayer);  

    public bool PlayerInAttackRange => //checks for the position of the player in a radius of attack range on layer mask of player
        Physics.CheckSphere(transform.position, _attackRange, _whatIsPlayer);
}
