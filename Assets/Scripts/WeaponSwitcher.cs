using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    private int selectedWeapon = 0;
    [SerializeField] private InputActionReference weaponSwitchInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            selectedWeapon = 0;
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            selectedWeapon = 1;
        }

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
            SelectWeapon();
        }


    }

    private void SelectWeapon()
    {
        if (selectedWeapon >= transform.childCount)
        {
            selectedWeapon = transform.childCount - 1;
        }

        int i = 0;
        foreach (Transform _weapon in transform)
        {
            if (i == selectedWeapon)
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
