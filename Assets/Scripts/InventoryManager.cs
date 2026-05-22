using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [HideInInspector]
    public ItemGrid selectedItemGrid;

    private void Update()
    {
        if (selectedItemGrid == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            Debug.Log(selectedItemGrid.GetTileGridPosition(Mouse.current.position.ReadValue()));
    }
}
