using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    private int selectedWeapon = 0;
    private int pendingWeapon = 0;
    [SerializeField] private InputActionReference weaponSwitchInput;
    private Animator[] weaponAnimators;

    private bool switching = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print(transform.childCount);
        weaponAnimators = new Animator[transform.childCount];

        for (int i = 0; i < transform.childCount; i++) //fill weapon animators array with each animator in child list
        {
            weaponAnimators[i] = transform.GetChild(i).GetComponentInChildren<Animator>(true);
        }
            
        
        SelectWeapon(selectedWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        print(weaponAnimators);
        if (switching)
            return;

        int previousSelectedWeapon = selectedWeapon;
        GetWeaponIndex();
        


        if (previousSelectedWeapon != selectedWeapon)
        {
            StartWeaponSwitch(previousSelectedWeapon, selectedWeapon);
        }


    }

    private int GetWeaponIndex()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            selectedWeapon = 0;
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            selectedWeapon = 1;

        if (weaponSwitchInput.action.ReadValue<Vector2>().y > 0f) //if scroll up get the next weapon above the current in the child list
        {
            selectedWeapon = (selectedWeapon + 1) % transform.childCount; //modulus for wrap around cases
            //if (InventoryManager.instance.CheckIfItemTypeInInventory(ItemData.ItemType.Weapon))
            //    return;
        }

        if (weaponSwitchInput.action.ReadValue<Vector2>().y < 0f) //if scroll down get the previous weapon above the current in the child list
        {
            selectedWeapon = (selectedWeapon - 1 + transform.childCount) % transform.childCount;
            //if (InventoryManager.instance.CheckIfItemTypeInInventory(ItemData.ItemType.Weapon))
            //    return;
        }

        return selectedWeapon;
    }


    private void StartWeaponSwitch(int fromWeapon, int toWeapon)
    {
        switching = true;
        selectedWeapon = fromWeapon;
        pendingWeapon = toWeapon;

        weaponAnimators[selectedWeapon].SetBool("holster",true);
    }

    public void AnimEventFinishHolster()  //ANIMATION EVENT ONLY
    {
        weaponAnimators[selectedWeapon].SetBool("holster", false);
        SelectWeapon(pendingWeapon);
        selectedWeapon = pendingWeapon;
    }

    public void AnimEventFinishDraw()  //ANIMATION EVENT ONLY
    {
        weaponAnimators[selectedWeapon].SetTrigger("idling");
        switching = false;
    }

    private void SelectWeapon(int index)
    {
        int i = 0;
        foreach (Transform _weapon in transform)
        {
            if (i == index)
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
}
