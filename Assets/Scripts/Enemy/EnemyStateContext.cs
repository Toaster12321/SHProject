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


    public EnemyStateContext(SkinnedMeshRenderer enemyRenderer, Color originalColor, NavMeshAgent agent, Transform player, LayerMask whatIsGround,
        LayerMask whatIsPlayer, Animator animator, ParticleSystem particleEmittor)
    {
        _enemyRenderer = enemyRenderer;
        _originalColor = originalColor;
        _agent = agent;
        _player = player;
        _whatIsGround = whatIsGround;
        _whatIsPlayer = whatIsPlayer;
        _animator = animator;
        _particleEmittor = particleEmittor;
    }

    public SkinnedMeshRenderer EnemyRenderer => _enemyRenderer;
    public Color OriginalColor => _originalColor;
    public NavMeshAgent Agent => _agent;
    public Transform Player => _player;
    public Animator Animator => _animator;
    public ParticleSystem ParticleEmittor => _particleEmittor;
    public LayerMask WhatIsGround => _whatIsGround;
    public LayerMask whatIsPlayer => _whatIsPlayer;
}
