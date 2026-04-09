using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Attacking Params")]
    public float attackDistance = 3.5f;
    public float attackDelay = 0.4f; //delay of when attack should hit
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

    public AudioSource swingSound;
    public AudioSource hitSound;
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
    }

    private void Start()
    {
        playerCamera = Camera.main.transform; //get camera's current spot
    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        playerControls.Player.Attack.performed += Swing;
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Attack.performed -= Swing;
        playerControls.Disable();
    }

    public void Swing(InputAction.CallbackContext ctx)
    {
        if (!readyToAttack || attacking) return; //if we aren't ready to attack or are already attacking -> do nothing

        readyToAttack = false;
        attacking = true;

        animator.SetTrigger("swinging");
        Invoke(nameof(ResetAttack), attackSpeed); //calls reset attack function after 1s(attack speed)
        Invoke(nameof(AttackRaycast), attackDelay);//calls the attack ray cast function after 0.4s(attack delay)

        swingSound.pitch = Random.Range(0.7f, 0.9f);
        swingSound.Play();
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

    //public void ChangeAnimationState(string newState)
    //{
    //    if (currentAnimationState == newState) return; //stop the same animation from playing itself again

    //    currentAnimationState = newState;
    //    animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    //}

}
