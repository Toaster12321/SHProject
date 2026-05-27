using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [HideInInspector]
    public ItemGrid selectedItemGrid;

    InventoryItem selectedItem;
    InventoryItem overlappedItem;
    Vector2Int tileGridPosition;

    RectTransform rectTransform;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;
    private void Update()
    {
        ItemIconDrag();

        if( Keyboard.current.qKey.wasPressedThisFrame)
        {
            CreateRandomItem();
        }

        if (selectedItemGrid == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            LeftMouseButtonPress();
        }
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);

        int selectedItemID = Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }    

    private void LeftMouseButtonPress()
    {
        Vector2 cursorPosition = Mouse.current.position.ReadValue();

        if (selectedItem != null) //offsets cursor position based on item size when placing items
        {
            cursorPosition.x -= (selectedItem.itemData.width - 2) * ItemGrid.tileSizeWidth / 4;
            cursorPosition.y += (selectedItem.itemData.height - 2) * ItemGrid.tileSizeHeight / 4;
        }

        tileGridPosition = selectedItemGrid.GetTileGridPosition(cursorPosition); //reads which grid was pressed based on mouse input

        if (selectedItem == null) //if we dont have an item picked up, pick one up
        {
            PickUpItem(tileGridPosition);
        }
        else //otherwise an item is already picked up so place it at a location
        {
            PlaceItem(tileGridPosition);
        }
    }

    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool placementAllowed = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlappedItem);
        if (placementAllowed)
        {
            selectedItem = null; //reset item
            if (overlappedItem != null)
            {
                selectedItem = overlappedItem;
                overlappedItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
            }
        }
            

    }

    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>(); //get transform of picked up item
        }
    }

    private void ItemIconDrag()
    {
        if (selectedItem != null) //if we have a picked up item move it with the cursor
            rectTransform.position = Mouse.current.position.ReadValue();
    }
}
