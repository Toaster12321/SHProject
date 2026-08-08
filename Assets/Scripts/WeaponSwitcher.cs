using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    private int selectedWeapon = 0;
    [SerializeField] private InputActionReference weaponSwitchInput;
    [SerializeField] private Animator[] weaponAnimators;

    private bool switching = false;
    private int pendingWeapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectWeapon(selectedWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        if (switching)
            return;

        int previousSelectedWeapon = selectedWeapon;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            selectedWeapon = 0;
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            selectedWeapon = 1;

        if (weaponSwitchInput.action.ReadValue<Vector2>().y > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon += 1;
            //if (InventoryManager.instance.CheckIfItemTypeInInventory(ItemData.ItemType.Weapon))
            //    return;
        }

        if (weaponSwitchInput.action.ReadValue<Vector2>().y < 0f)
        {
            if (selectedWeapon <= 0f)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon -= 1;
            //if (InventoryManager.instance.CheckIfItemTypeInInventory(ItemData.ItemType.Weapon))
            //    return;
        }


        if (previousSelectedWeapon != selectedWeapon)
        {
            StartWeaponSwitch(selectedWeapon);
        }


    }


    private void StartWeaponSwitch(int newWeapon)
    {
        switching = true;
        pendingWeapon = newWeapon;
        weaponAnimators[selectedWeapon].SetTrigger("holster");
    }

    public void AnimEventFinishHolster()  //ANIMATION EVENT ONLY
    {
        SelectWeapon(pendingWeapon);
        selectedWeapon = pendingWeapon;
    }

    public void AnimEventFinishDraw()  //ANIMATION EVENT ONLY
    {
        switching = false;
    }

    private void SelectWeapon(int index)
    {
        if (index >= transform.childCount)
        {
            index = transform.childCount - 1;
        }

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
