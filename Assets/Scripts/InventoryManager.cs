using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [HideInInspector]
    public ItemGrid selectedItemGrid;

    InventoryItem selectedItem;
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
        tileGridPosition = selectedItemGrid.GetTileGridPosition(Mouse.current.position.ReadValue()); //reads which grid was pressed based on mouse input

        if (selectedItem == null) //if we dont have an item picked up, pick one up
        {
            PickUpItem();
        }
        else //otherwise an item is already picked up so place it at a location
        {
            PlaceItem();
        }
    }

    private void PlaceItem()
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y);
        if (complete)
            selectedItem = null; //reset item

    }

    private void PickUpItem()
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
