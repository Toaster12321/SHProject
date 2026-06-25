using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Revolver : MonoBehaviour
{
    [SerializeField] private AudioSource gunshot;
    [SerializeField] private AudioSource cocking;
    public UnityEvent onGunShoot;
    public float fireCooldown;
    public Animator gunAnimator;
    public Animator cameraAnimator;

    private float currentCooldown;
    private InputAction attackAction;

    private FirstPersonController firstPersonController;

    private void Awake()
    {
        firstPersonController = GetComponentInParent<FirstPersonController>();
    }

    void Start()
    {
        currentCooldown = fireCooldown; //set cooldown
        attackAction = FirstPersonController.playerInput.actions["Attack"];
        AssignInput();
    }

    void Update()
    {
        currentCooldown -= Time.deltaTime; //start timer

        if (firstPersonController.isDashing)
            gunAnimator.SetBool("dashing", true);
        else
            gunAnimator.SetBool("dashing", false);
    }

    private void Shoot()
    {
        if (currentCooldown <= 0f)
        {
            gunAnimator.SetTrigger("fire");
            cameraAnimator.SetTrigger("recoil");
            if (!gunshot.isPlaying)
                gunshot.Play();

            onGunShoot?.Invoke(); //event that fires on shoot
            currentCooldown = fireCooldown; //reset cooldown
            StartCoroutine(PlayCocking());
        }
    }

    private void AssignInput()
    {
        attackAction.performed += ctx => Shoot();
    }


    IEnumerator PlayCocking()
    {
        yield return new WaitForSeconds(0.2f); //delay to match when gunshot is over
        cocking.Play();
    }
}
