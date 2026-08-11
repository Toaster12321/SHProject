using System;
using System.Collections;
using UnityEditor.Build;
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
    private int pendingRemainingAmmo;

    private FirstPersonController firstPersonController;
    private WeaponSwitcher weaponSwitcher;

    private void Awake()
    {
        firstPersonController = GetComponentInParent<FirstPersonController>();
        weaponSwitcher = GetComponentInParent<WeaponSwitcher>();
        attackAction = FirstPersonController.playerInput.actions["Attack"];
        reloadAction = FirstPersonController.playerInput.actions["Reload"];
    }

    void Start()
    {
        currentClipAmmoCount = clipSize;
        currentCooldown = fireCooldown; //set cooldown
    }

    void Update()
    {
        currentCooldown -= Time.deltaTime; //start timer

        if (firstPersonController.isDashing)
            gunAnimator.SetBool("dashing", true);
        else
            gunAnimator.SetBool("dashing", false);
    }

    private void Shoot(InputAction.CallbackContext ctx)
    {
        if (currentClipAmmoCount <= 0) //prevent shooting when empty
        {
            gunAnimator.SetTrigger("empty");
            return;
        }

        if (currentClipAmmoCount <= 0 && reserveAmmoCount > 0) //auto reload if out of ammo and have more in reserve
        {
            gunAnimator.SetBool("reloading", true);
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

    private void Reload(InputAction.CallbackContext ctx)
    {
        if (currentClipAmmoCount == clipSize) //full clip, can't reload
            return;

        if (reserveAmmoCount <= 0) //no reserve ammo to reload
             return;

        gunAnimator.SetBool("reloading",true);
    }

    private void AnimEventGiveAmmo() //ANIMATION EVENT ONLY
    {
        int remainderAmmo = clipSize - currentClipAmmoCount;

        if (reserveAmmoCount >= remainderAmmo) //if we have more than enough reserve ammo give a full clip when reloading
            currentClipAmmoCount += remainderAmmo;
        else //otherwise give the remaning reserve ammo in the clip
            currentClipAmmoCount = reserveAmmoCount;

        reserveAmmoCount = Mathf.Clamp(reserveAmmoCount -= remainderAmmo, 0, int.MaxValue); //dont go below 0 in reserve ammo
        gunAnimator.SetBool("reloading", false);
    }

    private void ResetShootState()
    {
        gunAnimator.Rebind();

        gunAnimator.ResetTrigger("fire");
        gunAnimator.ResetTrigger("empty");

        gunAnimator.SetBool("reloading", false);
        gunAnimator.SetBool("dashing", false);
        gunAnimator.SetBool("holster", false);
        CancelInvoke();
        StopAllCoroutines();
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

    private void animEventDryFire() //ANIMATION EVENT ONLY 
    {
        dryFire.Play();
    }

    private void OnEnable()
    {
        ResetShootState();
        attackAction.performed += Shoot;
        reloadAction.performed += Reload;
    }

    private void OnDisable()
    {
        ResetShootState();
        attackAction.performed -= Shoot;
        reloadAction.performed -= Reload;
    }

    private void AnimEventFinishHolster()
    {
        Debug.Log("holster finished");
        weaponSwitcher.AnimEventFinishHolster();
    }

    private void AnimEventFinishDraw()
    {
        Debug.Log("draw finished");
        weaponSwitcher.AnimEventFinishDraw();
    }
}
