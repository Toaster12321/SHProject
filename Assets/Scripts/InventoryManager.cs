using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid { get => selectedItemGrid; 
        set 
        { 
            selectedItemGrid = value; 
            inventoryHighlight.SetParent(value); //sets parent to selectedItemGrid whenever setting selectedItemGrid
        } }

    InventoryItem selectedItem;
    InventoryItem overlappedItem;

    RectTransform rectTransform;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;
    [SerializeField] UIManager uiManager;

    InventoryHighlight inventoryHighlight;

    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    private void Update()
    {
        if (uiManager.inventoryOpen)
        {
            ItemIconDrag();

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                if (selectedItem == null)
                    CreateRandomItem();
            }

            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                InsertRandomItem();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                RotateItem();
            }

            if (selectedItemGrid == null)
            {
                inventoryHighlight.Show(false);
                return;
            }

            HandleHighlight();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                LeftMouseButtonPress();
            }
        }
    }

    private void RotateItem()
    {
        if (selectedItem == null)
            return;

        selectedItem.Rotate();

        //reset old position so highlight can switch to new grids when rotated
        oldItemPosition = new Vector2Int(-1, -1);
        HandleHighlight();
    }

    private void InsertRandomItem()
    {
        if (selectedItemGrid == null)
            return;

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    public void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) //no space on grid
        {
            print("no space");
            // HANDLE NO SPACE CLAUSE
            return;
        }

        selectedItemGrid.PlaceItemOnGrid(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    Vector2Int oldItemPosition;
    InventoryItem itemToHighlight;
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();

        if (oldItemPosition == positionOnGrid)
            return;

        oldItemPosition = positionOnGrid;
        if (selectedItem == null) //nothing is picked up
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y); //get hovered item's location

            if (itemToHighlight != null) //if there is an item to highlight, show with same size as the item's width and height
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
                inventoryHighlight.Show(false);
        }
        else //item is picked up
        {
            inventoryHighlight.Show(selectedItemGrid.BoundraryCheck( //boundrary check bool to make sure grids dont show outside of grid space
                positionOnGrid.x, 
                positionOnGrid.y, 
                selectedItem.WIDTH, 
                selectedItem.HEIGHT
                )); //show highlight on empty grids to show space taken up

            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();//sets the current item to always show above the canvas and any items on it
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }    

    public void AddItem(ItemData itemData) //add's picked up item to inventory
    {
        if (selectedItemGrid  == null)
            return; 

        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        inventoryItem.Set(itemData);
        InsertItem(inventoryItem);
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();

        if (selectedItem == null) //if we dont have an item picked up, pick one up
        {
            PickUpItem(tileGridPosition);
        }
        else //otherwise an item is already picked up so place it at a location
        {
            PlaceItem(tileGridPosition);
        }
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 cursorPosition = Mouse.current.position.ReadValue();

        if (selectedItem != null) //offsets cursor position based on item size when placing items
        {
            cursorPosition.x -= (selectedItem.WIDTH - 2) * ItemGrid.tileSizeWidth / 4;
            cursorPosition.y += (selectedItem.HEIGHT - 2) * ItemGrid.tileSizeHeight / 4;
        }

        return selectedItemGrid.GetTileGridPosition(cursorPosition);
    }

    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool placementAllowed = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlappedItem);
        if (placementAllowed)
        {
            selectedItem = null; //reset item
            if (overlappedItem != null) //if theres an overlapped item, pick up the item and set it to show on top of the canvas/item
            {
                selectedItem = overlappedItem;
                overlappedItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
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
