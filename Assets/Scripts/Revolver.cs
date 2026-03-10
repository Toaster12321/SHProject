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
    public Animator animator; 

    private float currentCooldown;
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    void Start()
    {
        currentCooldown = fireCooldown; //set cooldown
    }

    void Update()
    {
        currentCooldown -= Time.deltaTime; //start timer
    }

    private void Shoot( InputAction.CallbackContext ctx )
    {
        if (currentCooldown <= 0f)
        {
            Debug.Log("shoot");
            animator.SetTrigger( "fire" );
            if (!gunshot.isPlaying)
                gunshot.Play();

            onGunShoot?.Invoke(); //event that fires on shoot
            currentCooldown = fireCooldown; //reset cooldown
            StartCoroutine( PlayCocking() );
        }
    }

    private void OnEnable()
    {
        playerControls.Player.Fire.performed += Shoot;
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Fire.performed -= Shoot;
        playerControls.Disable();
    }

    IEnumerator PlayCocking()
    {
        yield return new WaitForSeconds(0.2f); //delay to match when gunshot is over
        cocking.Play();
    }
}
