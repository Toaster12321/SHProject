using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Attacking Params")]
    public float attackDistance = 3.5f;
    public float attackDelay = 0.001f; //delay of when attack should hit
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

    public AudioSource swingSound;
    private PlayerControls playerControls;
    private Transform playerCamera;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;

    //Animation Params
    public Animator animator;

    private void Awake()
    {
        playerControls = new PlayerControls();
        AssignInput();
    }

    private void Start()
    {
        playerCamera = Camera.main.transform; //get camera's current spot
    }

    private void Update()
    {
        if (playerControls.Player.Attack.IsPressed()) //if player holds down attack, keep attacking
            Swing();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void Swing()
    {
        if (!readyToAttack || attacking) return; //if we aren't ready to attack or are already attacking -> do nothing

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(AttackRaycast), attackDelay);//calls the attack ray cast function after attack delay
        Invoke(nameof(ResetAttack), attackSpeed); //calls reset attack function after 1s(attack speed)

        swingSound.pitch = Random.Range(0.7f, 0.9f);
        swingSound.Play();

        if (attackCount == 0)
        {
            animator.SetTrigger("swinging");
            attackCount++;
        }
        else
        {
            animator.SetTrigger("following_up");
            attackCount = 0;
        }
    }

    private void ResetAttack() //reset bools
    {
        attacking = false;
        readyToAttack = true;
    }

    private void AttackRaycast()
    {
        Ray meleeRay = new Ray(playerCamera.position, playerCamera.forward); //raycast pointing from camera forwards
        if (Physics.Raycast(meleeRay, out RaycastHit hitinfo, attackDistance, attackLayer)) //if the raycast collides
        {
            EnemyStateMachine enemy = hitinfo.collider.GetComponentInParent<EnemyStateMachine>();
            Debug.Log("Hit: " + hitinfo.collider.name);
            if (enemy) //if we hit an object with the enemystatemachine script
            {
                enemy.TakeDamage(attackDamage); //apply damage
            }
        }
    }

    void AssignInput()
    {
        playerControls.Player.Attack.performed += ctx => Swing(); //call swing when attack is performed
    }
}

