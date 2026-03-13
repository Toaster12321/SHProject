using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float maxHP;
    private float currentHP;
    private Renderer enemyRenderer;
    private Color originalColor;

    public NavMeshAgent agent; //navmesh agent reference

    public Transform player; //player position reference

    public LayerMask whatIsGround, whatIsPlayer; //layers for ground and player

    public Animator animator; //animaton reference

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public bool isIdling;

    //Attacking 
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public bool isDead;
    private void Start()
    {
        GetSetHealth = maxHP; //make sure enemy starts with max HP
    }

    public float GetSetHealth
    {
        get 
        { 
            return currentHP; 
        }
        set
        {
            currentHP = value; //value from max HP
            Debug.Log(currentHP);
            if (currentHP != maxHP && !isDead)
            {
                StartCoroutine(FlashRed());

            }
            if ( currentHP <= 0f )
            {
                animator.SetTrigger("no_hp");
                isDead = true;
                agent.isStopped = true;
                isIdling = false;
                //Destroy(gameObject);
            }
        }
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform; //assign player to the game object called player and its transform settings
        agent = GetComponent<NavMeshAgent>(); //assign agent to the navmeshagent 
        animator = GetComponent<Animator>();
        enemyRenderer = this.gameObject.GetComponentInChildren<Renderer>(); //renderer is located in child for scab
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    private void Update()
    {
        if ( !isDead )
        {
            //Check for sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);//checks for the position of the player in a radius of sight range on layer mask of player
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); //checks for the position of the player in a radius of attack range on layer mask of player

            if (!playerInSightRange && !playerInAttackRange) Patrolling(); //if we cant see the player and are not in the player attack range, patrol
            if (playerInSightRange && !playerInAttackRange) ChasePlayer(); //if we see the player but not in the attack range, chase the player
            if (playerInAttackRange && playerInSightRange) AttackPlayer(); //if we see the player and the player is in attack range, attack
        }

    }

    private void Patrolling()
    {
        if (isIdling)
            return;

        animator.SetBool("walking", true);
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);//agent starts patrolling to walk point

        Vector3 distanceToWalkPoint = transform.position - walkPoint; //distance from current spot to walk point

        if (distanceToWalkPoint.magnitude < 1f)//if we reached the walkpoint create a new one
        {
            StartCoroutine(Idle(4.5f));
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange); //random range between our two walk points for the enemy to patrol in X and Z axis
        float randomX = Random.Range(-walkPointRange, walkPointRange); 

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ); //set current position on x and z to our random values

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) //make sure our walkpoint is on the ground and not off the map
            walkPointSet = true;  //walk point is set
    }

    private void ChasePlayer()
    {
        animator.SetBool("walking", true);
        agent.SetDestination(player.position); //make agent go to the player
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position); //make sure enemy doesnt move

        transform.LookAt(player); //rotate the enemy so it faces the player when attacking
        //animator.Play("Eat");

        if (!alreadyAttacked)
        {
            alreadyAttacked = true; //set attack to true
            Invoke(nameof(ResetAttack), timeBetweenAttacks); //invokes the reset atack function after a delay of time between attacks
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    
    private IEnumerator Idle(float idleTime) //idle for a set amount of time before moving again
    {
        isIdling = true;

        animator.SetBool("walking", false);
        agent.isStopped = true;

        yield return new WaitForSeconds(idleTime);

        agent.isStopped = false;
        isIdling = false;
    }

    IEnumerator FlashRed() //coroutine to set red flash as indicator for damage for 0.1s
    {
        enemyRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        enemyRenderer.material.color = originalColor;
    }

}
