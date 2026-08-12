using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    private int selectedWeaponIndex = 0;
    private int pendingWeaponIndex = 0;
    [SerializeField] private InputActionReference weaponSwitchInput;
    private Animator[] weaponAnimators;
    private List<ItemData> weaponList;
    private bool switching = false;
    

    void Start()
    {
        weaponAnimators = new Animator[transform.childCount];
        weaponList = new List<ItemData>();
        for (int i = 0; i < transform.childCount; i++) //fill weapon animators array with each animator in child list
        {
            weaponAnimators[i] = transform.GetChild(i).GetComponentInChildren<Animator>(true);
            weaponList.Add(transform.GetChild(i).GetComponentInChildren<InventoryItem>(true).itemData); //add the item data of each item in weapon holder to the weapon list of item data
        }
        print(weaponList.Count);

        for (int i = 0; i < weaponList.Count; i++) //if the player owns a weapon in the list on start select it 
        {
            if (PlayerOwnsSelectedWeapon(i))
                SelectWeapon(selectedWeaponIndex);
        }

    }


    void Update()
    {
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

        weaponAnimators[selectedWeaponIndex].SetBool("holster",true); //play the holster animation the held weapon
    }


    public void AnimEventFinishHolster()  //ANIMATION EVENT ONLY
    {
        weaponAnimators[selectedWeaponIndex].SetBool("holster", false); //reset bool

        SelectWeapon(pendingWeaponIndex); //after the holster animation is finished select the new weapon at index
        selectedWeaponIndex = pendingWeaponIndex; //update selected weapon index
    }


    public void AnimEventFinishDraw()  //ANIMATION EVENT ONLY
    {
        weaponAnimators[selectedWeaponIndex].SetTrigger("idling"); //allow switching again after draw animation is finished
        switching = false;
    }

    private void SelectWeapon(int index)
    {
        int i = 0;
        foreach (Transform _weapon in transform) //iterate through each weapon in weapon holder
        {
            if (i == index) //if the passed index corresponds to the index in the holder activate it, otherwise turn off all others
            {
                _weapon.gameObject.SetActive(true);
            }
            else
            {
                _weapon.gameObject.SetActive(false);
            }

            i++;
        }
    }


    private void SelectNextOwnedWeapon(int scrollDirection)
    {
        for (int i = 1; i <= transform.childCount; i++)//iterate through childcount + 1 for wrap around cases
        {
            int candidate = (selectedWeaponIndex + scrollDirection * i + transform.childCount) % transform.childCount; //get the scroll direction * current iteration, add it to the current index, then get the remainder based off current children count
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
        return PlayerOwnsWeaponData(weaponList[index]); 
    }


    public void EquipWeapon()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (PlayerOwnsSelectedWeapon(i)) //if the player owns a weapon at the passed index 
            {
                selectedWeaponIndex = i; //update selected weapon index and equp weapon at index
                SelectWeapon(i);
                return;
            }
        }
    }

}
