using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Revolver : MonoBehaviour
{
    [SerializeField] private AudioSource gunshot;
    [SerializeField] private AudioSource cocking;
    [SerializeField] private AudioSource dryFire;
    [SerializeField] private int clipSize;
    
    public UnityEvent onGunShoot;
    public float fireCooldown;
    public Animator gunAnimator;
    public Animator cameraAnimator;

    private float currentCooldown;
    private InputAction attackAction;
    private InputAction reloadAction;
    private int currentClipAmmoCount;
    public int reserveAmmoCount;

    private FirstPersonController firstPersonController;

    private void Awake()
    {
        firstPersonController = GetComponentInParent<FirstPersonController>();
    }

    void Start()
    {
        currentClipAmmoCount = clipSize;
        currentCooldown = fireCooldown; //set cooldown
        attackAction = FirstPersonController.playerInput.actions["Attack"];
        reloadAction = FirstPersonController.playerInput.actions["Reload"];
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
        if (currentClipAmmoCount <= 0)
        {
            gunAnimator.SetTrigger("empty");
            return;
        }

        if (currentCooldown <= 0f)
        {
            currentClipAmmoCount -= 1;
            gunAnimator.SetTrigger("fire");
            cameraAnimator.SetTrigger("recoil");
            onGunShoot?.Invoke(); //event that fires on shoot
            currentCooldown = fireCooldown; //reset cooldown
            if (!gunshot.isPlaying)
                StartCoroutine(PlayGunshot());

            StartCoroutine(PlayCocking());
        }
    }

    private void Reload()
    {
        if (currentClipAmmoCount == clipSize) //full clip, can't reload
            return;

        if (reserveAmmoCount <= 0) //no reserve ammo to reload
             return;

        gunAnimator.SetTrigger("reloading");
        int remainderAmmo = clipSize - currentClipAmmoCount;

        if (reserveAmmoCount >= remainderAmmo) //if we have more than enough reserve ammo give a full clip when reloading
            currentClipAmmoCount += remainderAmmo;
        else //otherwise give the remaning reserve ammo in the clip
            currentClipAmmoCount = reserveAmmoCount;

        reserveAmmoCount = Mathf.Clamp(reserveAmmoCount -= remainderAmmo, 0, int.MaxValue); //dont go below 0 in reserve ammo
    }

    private void AssignInput()
    {
        reloadAction.performed += ctx => Reload();
        attackAction.performed += ctx => Shoot();
    }


    IEnumerator PlayCocking()
    {
        yield return new WaitForSeconds(0.2f); //delay to match when gunshot is over
        cocking.Play();
    }

    IEnumerator PlayGunshot()
    {
        yield return new WaitForSeconds(0.1f); //delay to match when gunshot is over
        gunshot.Play();
    }

    private void animEventDryFire()
    {
        dryFire.Play();
    }
}
