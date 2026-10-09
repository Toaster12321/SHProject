using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    private int selectedWeaponIndex = 0;
    private int pendingWeaponIndex = 0;
    [SerializeField] private InputActionReference weaponSwitchInput;
    private Animator weaponAnimator;
    private List<ItemData> weaponList;
    private List<Transform> weaponObjects;
    private bool switching = false;

    [Header("Revolver Variables")]
    [SerializeField] private Revolver revolver;
    [SerializeField] private MeleeWeapon meleeWeapon;
    [SerializeField] private AudioSource dryFire;

    private const int REVOVLER_ANIM_LAYER = 1;
    private const int KNIFE_ANIM_LAYER = 2;


    void Start()
    {
        weaponAnimator = GetComponent<Animator>();
        weaponList = new List<ItemData>();
        weaponObjects = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++) //fill weapon animators array with each animator in child list
        {
            Transform weaponTransform = transform.GetChild(i);

            InventoryItem inventoryItem = weaponTransform.GetComponentInChildren<InventoryItem>(true);

            if (inventoryItem == null || inventoryItem.itemData == null)
                continue;

            weaponList.Add(inventoryItem.itemData); //add the item data of each item in weapon holder to the weapon list of item data
            weaponObjects.Add(weaponTransform);
        }
    }


    void Update()
    {
        print(switching);
        if (switching) //prevent switching if already switching
            return;

        int previousSelectedWeapon = selectedWeaponIndex; //get the current weapon index
        
        GetNewWeaponIndex(); //constantly check for updates in weapon index and change selectedWeapon if found

        if (previousSelectedWeapon != selectedWeaponIndex) //if the old index doesn't equal the new index begin switching weapons
        {
            StartWeaponSwitch(previousSelectedWeapon, selectedWeaponIndex);
        }
    }


    private int GetNewWeaponIndex()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) //keys 1-9 switch between weapons if owned
        {
            if (PlayerOwnsSelectedWeapon(0))
                selectedWeaponIndex = 0;
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            if (PlayerOwnsSelectedWeapon(1))
                selectedWeaponIndex = 1;
        }


        if (weaponSwitchInput.action.ReadValue<Vector2>().y > 0f) //if scroll up get the next weapon index above the current in the child list
        {
            SelectNextOwnedWeapon(1);
        }

        if (weaponSwitchInput.action.ReadValue<Vector2>().y < 0f) //if scroll down get the previous weapon index above the current in the child list
        {
            SelectNextOwnedWeapon(-1);
        }

        return selectedWeaponIndex;
    }


    private void StartWeaponSwitch(int fromWeaponIndex, int toWeaponIndex) //switches from old index(fromWeapon) to new index(toWeapon)
    {
        switching = true;

        selectedWeaponIndex = fromWeaponIndex; //get the index of the current held weapon
        pendingWeaponIndex = toWeaponIndex; //hold the next index value in pending weapon

        weaponAnimator.SetBool("holster",true); //play the holster animation the held weapon
    }

    private void SelectWeapon(int index)
    {
        if (index < 0 || index >= weaponObjects.Count)
            return;

        for (int i = 0; i < weaponObjects.Count; i++)
            weaponObjects[i].gameObject.SetActive(i == index);

        string tag = weaponObjects[index].tag;

        if (tag == "Revolver")
        {
            weaponAnimator.SetLayerWeight(REVOVLER_ANIM_LAYER, 1);
            weaponAnimator.SetLayerWeight(KNIFE_ANIM_LAYER, 0);
        }
        else if (tag == "Knife")
        {
            weaponAnimator.SetLayerWeight(REVOVLER_ANIM_LAYER, 0);
            weaponAnimator.SetLayerWeight(KNIFE_ANIM_LAYER, 1);
        }

    }


    private void SelectNextOwnedWeapon(int scrollDirection)
    {
        if (weaponList.Count == 0) 
            return;

        for (int i = 1; i <= weaponList.Count; i++)//iterate through childcount + 1 for wrap around cases
        {
            int candidate = (selectedWeaponIndex + scrollDirection * i + weaponList.Count) % weaponList.Count; //get the scroll direction * current iteration, add it to the current index, then get the remainder based off current children count
            if(PlayerOwnsSelectedWeapon(candidate)) //update selected weapon index if player owns weapon at index(candidate)
            {
                selectedWeaponIndex = candidate; 
                return;
            }
        }
    }


    private bool PlayerOwnsWeaponData(ItemData weaponData)//return whether or not a weapon item is owned
    {
        var playerWeapons = InventoryManager.instance.selectedItemGrid.GetItemTypeInInventory(ItemData.ItemType.Weapon); //get list of weapons currently in inventory
        return playerWeapons.Contains(weaponData); 
    }

    private bool PlayerOwnsSelectedWeapon(int index) //return whether the player owns a weapon at this index 
    {
        if (index < 0 || index >= weaponList.Count)
            return false;

        return PlayerOwnsWeaponData(weaponList[index]); 
    }


    public void EquipWeapon(ItemData pickedUpWeapon)
    {
        int index = weaponList.IndexOf(pickedUpWeapon);

        if (index < 0 || !PlayerOwnsSelectedWeapon(index))
            return;

        if (selectedWeaponIndex == index)
        {
            SelectWeapon(index);
            return;
        }

        if (switching)
            return;

        StartWeaponSwitch(selectedWeaponIndex, index);
    }

    //----- ANIM EVENTS -----
    private void animEventDryFire() //ANIMATION EVENT ONLY for revolver
    {
        dryFire.Play();
    }

    private void AnimEventGiveAmmo() //ANIMATION EVENT ONLY
    {
        revolver.AnimEventGiveAmmo();
    }

    private void EnableMeleeWeaponCollider() //enable/disable collider for animation events
    {
        meleeWeapon.EnableWeaponCollider();
    }

    private void DisableMeleeWeaponCollider() //enable/disable collider for animation events
    {
        meleeWeapon.DisableWeaponCollider();
    }
    private void AnimEventFinishSwing()
    {
        weaponAnimator.SetBool("swinging", false);
    }
    private void AnimEventFinishFollowUp()
    {
        weaponAnimator.SetBool("following_up", false);
    }

    public void AnimEventFinishHolster()  //ANIMATION EVENT ONLY
    {
        weaponAnimator.SetBool("holster", false); //reset bool

        SelectWeapon(pendingWeaponIndex); //after the holster animation is finished select the new weapon at index
        selectedWeaponIndex = pendingWeaponIndex; //update selected weapon index
    }


    public void AnimEventFinishDraw()  //ANIMATION EVENT ONLY
    {
        print("draw finsihed");
        weaponAnimator.SetTrigger("idling"); //allow switching again after draw animation is finished
        switching = false;
    }
}
