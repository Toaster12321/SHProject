using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [HideInInspector]
    public ItemGrid selectedItemGrid;

    InventoryItem selectedItem;
    Vector2Int tileGridPosition;
    RectTransform rectTransform;

    private void Update()
    {
        if (selectedItem != null) //if we have a picked up item move it with the cursor
            rectTransform.position = Mouse.current.position.ReadValue();

        if (selectedItemGrid == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame) 
        {
            tileGridPosition = selectedItemGrid.GetTileGridPosition(Mouse.current.position.ReadValue()); //reads which grid was pressed based on mouse input

            if (selectedItem == null ) //if we dont have an item picked up, pick one up
            {
                selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
                if (selectedItem != null)
                {
                    rectTransform = selectedItem.GetComponent<RectTransform>(); //get transform of picked up item
                }
            }
            else //otherwise an item is already picked up so place it at a location
            {
                selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y);
                selectedItem = null; //reset item
            }
        }
    }
}
