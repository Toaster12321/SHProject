using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Attacking Params")]
    public float attackDelay = 0.001f; //delay of when attack should hit
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

    public AudioSource swingSound;
    private InputAction attackAction;
    [SerializeField] private Collider weaponCollider;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;

    //Animation Params
    public Animator knifeAnimator;
    public Animator cameraAnimator;

    private FirstPersonController firstPersonController;

    private void Awake()
    {
        firstPersonController = GetComponentInParent<FirstPersonController>();
        attackAction = FirstPersonController.playerInput.actions["Attack"];
    }

    private void Update()
    {
        if (attackAction.IsPressed()) //if player holds down attack, keep attacking
            Swing();

        if (firstPersonController.isDashing)
            knifeAnimator.SetBool("dashing", true);
        else
            knifeAnimator.SetBool("dashing", false);
    }
    private void OnActionPressed(InputAction.CallbackContext ctx) => Swing();

    public void Swing()
    {
        if (!readyToAttack || attacking) return; //if we aren't ready to attack or are already attacking -> do nothing

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed); //calls reset attack function after 1s(attack speed)

        if (attackCount == 0)
        {
            if (knifeAnimator.GetBool("dashing"))
                knifeAnimator.SetBool("dashing", false);
            knifeAnimator.SetTrigger("swinging");
            cameraAnimator.SetTrigger("knife_recoil");
            attackCount++;
        }
        else
        {
            if (knifeAnimator.GetBool("dashing"))
                knifeAnimator.SetBool("dashing", false);
            knifeAnimator.SetTrigger("following_up");
            cameraAnimator.SetTrigger("knife_followup");
            attackCount = 0;
        }
    }

    private void ResetAttack() //reset bools
    {
        attacking = false;
        readyToAttack = true;
    }


    private void OnTriggerEnter(Collider other) //when the collider connects with the enemy -> inflict damage
    {
        EnemyStateMachine enemy = other.GetComponentInParent<EnemyStateMachine>();
        if (enemy)
        {
            print("hit");
            enemy.TakeDamage(attackDamage);

        }
    }

    
    public void EnableWeaponCollider() //enable/disable collider for animation events
    {
        weaponCollider.enabled = true;
        swingSound.pitch = Random.Range(0.7f, 0.9f);
        swingSound.Play();
    }

    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }

    private void OnEnable()
    {
        attackAction.performed += OnActionPressed;
    }

    private void OnDisable()
    {
        attackAction.performed -= OnActionPressed;
    }
}

