using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Attacking Params")]
    public float attackDelay = 0.001f; //delay of when attack should hit
    public float attackCooldown = 0.9f;
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
    private WeaponSwitcher weaponSwitcher;

    private void Awake()
    {
        weaponSwitcher = GetComponentInParent<WeaponSwitcher>();
        firstPersonController = GetComponentInParent<FirstPersonController>();
        attackAction = FirstPersonController.playerInput.actions["Attack"];
    }

    private void Start()
    {
        attackCount = 0;
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

        Invoke(nameof(ResetAttack), attackCooldown); //calls reset attack function after 1s(cooldown)

        if (attackCount == 0)
        {
            print(attackCount);
            if (knifeAnimator.GetBool("dashing"))
                knifeAnimator.SetBool("dashing", false);
            knifeAnimator.SetBool("swinging",true);
            cameraAnimator.SetTrigger("knife_recoil");
            attackCount++;
        }
        else
        {
            print(attackCount);
            if (knifeAnimator.GetBool("dashing"))
                knifeAnimator.SetBool("dashing", false);
            knifeAnimator.SetBool("following_up", true);
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

    private void ResetAttackState()
    {
        attacking = false;
        readyToAttack = true;

        knifeAnimator.SetBool("swinging", false);
        knifeAnimator.SetBool("following_up", false);
        knifeAnimator.SetBool("dashing", false);

        weaponCollider.enabled = false;
        attackCount = 0;
        CancelInvoke(nameof(ResetAttack));
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
        ResetAttackState();
        attackAction.performed += OnActionPressed;
    }

    private void OnDisable()
    {
        ResetAttackState();
        attackAction.performed -= OnActionPressed;
    }

    private void AnimEventFinishHolster()
    {
        weaponSwitcher.AnimEventFinishHolster();
    }

    private void AnimEventFinishDraw()
    {
        weaponSwitcher.AnimEventFinishDraw();
    }

    private void AnimEventFinishSwing()
    {
        knifeAnimator.SetBool("swinging", false);
    }
    private void AnimEventFinishFollowUp()
    {
        knifeAnimator.SetBool("following_up", false);
    }
}

