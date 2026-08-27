using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 96; //grid boxes have size of 64x64px
    public const float tileSizeHeight = 96;

    InventoryItem[,] inventoryItemSlot;

    RectTransform rectTransform;

    [SerializeField] int gridSizeWidth; //change to resize grid (i.e. 5 x 10) = (320 x 640px)
    [SerializeField] int gridSizeHeight;
    public List<ItemData> weaponItems;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
        weaponItems = new List<ItemData>();
        Debug.Log($"Array dimensions: {inventoryItemSlot.GetLength(0)} x {inventoryItemSlot.GetLength(1)}");
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        if (!PositionCheck(x, y)) return null; //make sure we are in the bounds when picking up an item

        InventoryItem pickedUpItem = inventoryItemSlot[x, y]; //get the location where the item was picked up

        if (pickedUpItem == null)
            return null;
        CleanGridReferences(pickedUpItem); //clean reference on the tiles the item was located

        return pickedUpItem; //return the location where picked up
    }

    public void CleanGridReferences(InventoryItem itemToChange)
    {
        for (int ix = 0; ix < itemToChange.WIDTH; ix++) //go through all tiles in item data size
        {
            for (int iy = 0; iy < itemToChange.HEIGHT; iy++)
            {
                inventoryItemSlot[itemToChange.onGridPositionX + ix, itemToChange.onGridPositionY + iy] = null; //reset variable on all tiles where item was picked up
            }
        }
    }

    private void Init(int width, int height) //function to resize the grid height/width boxes and item sizes
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Debug.Log(width + " x" + height);
        Debug.Log(tileSizeWidth + "x" + tileSizeHeight);
        Vector2 size = new Vector2((width * tileSizeWidth) / 3 , (height * tileSizeHeight) / 3); //divide by 3 since using 3x scaled texture 
        Debug.Log(size);
        rectTransform.sizeDelta = size; //change rect transform of grid
    }

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x; //gets x and y location of mouse based on the rect transform of the grid
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;


        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth); //sets which grid is being hovered using ints
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        return tileGridPosition;
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlappedItem) //places item at an x an y pos on the grid
    {
        if (BoundraryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)//check to make sure item fits current boundraries
        {
            return false;
        }

        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlappedItem) == false)//check to see if there are any overlapping items
        {
            overlappedItem = null; //reset item stored
            return false;
        }

        if (overlappedItem != null) // if there was an overlapped item on the placed location, clear all old references of the previous item
        {
            CleanGridReferences(overlappedItem); 
        }

        PlaceItemOnGrid(inventoryItem, posX, posY); //place the item at the intended location

        return true; //able to place item
    }
    
    public void PlaceItemOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < inventoryItem.WIDTH; x++) //go through all tiles in the grid based off the size of the item, allow all tiles to be selected
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem; //get the slot location of where to place the item
            }
        }

        inventoryItem.onGridPositionX = posX; //convert from item slot location to grid location
        inventoryItem.onGridPositionY = posY;
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position; //set the objects location to the intended rect transform spot on the grid
    }

    public void ReturnItemToGrid(InventoryItem item, Vector2Int pos) //places item in original location, used in inventory manager
    {
        PlaceItemOnGrid(item, pos.x, pos.y);
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth / 3 + tileSizeWidth * inventoryItem.WIDTH / 6; //working with 32px asset but scaled 3x so visually 96px requires /3 and /6 for mouse to grid conversion
        position.y = -(posY * tileSizeHeight / 3 + tileSizeHeight * inventoryItem.HEIGHT / 6);
        return position;
    }

    private bool OverlapCheck(int posX, int posY, int itemWidth, int itemHeight, ref InventoryItem overlappedItem)
    {
        for (int x = 0; x < itemWidth; x++) //go through all tiles on an item's data
        {
            for (int y = 0; y < itemHeight; y++)
            {
                if (inventoryItemSlot[posX + x , posY + y] != null) //if there is not an item located on one of the grids
                {
                    if (overlappedItem == null) //if an overlapped item is not already stored set one to a location
                    {
                        overlappedItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else
                    {
                        if (overlappedItem != inventoryItemSlot[posX + x, posY + y])
                            return false;
                    }
                }
            }
        }

        return true;
    }

    private bool CheckAvailableSpaces(int posX, int posY, int itemWidth, int itemHeight) //checks all item spots and sees if the item can fit in a spot
    {
        for (int x = 0; x < itemWidth; x++) //go through all tiles on an item's data
        {
            for (int y = 0; y < itemHeight; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null) //if there is not an item located on one of the grids
                {
                   
                    return false;

                }
            }
        }

        return true;
    }

    public void DestroyItemInInventory(ItemData itemResource) //checks all item spots and destroys the passed item
    {
        if (itemResource == null)
            return;

        for (int ix = 0; ix < gridSizeWidth; ix++) //go through all tiles in the inventory grid
        {
            for (int iy = 0; iy < gridSizeHeight; iy++)
            {
                InventoryItem itemSlot = inventoryItemSlot[ix, iy];
                if (itemSlot != null && itemSlot.itemData == itemResource) //if the item is found clean references and destroy the game object sprite
                {
                    CleanGridReferences(itemSlot);
                    Destroy(itemSlot.gameObject);
                    return;
                }

            }
        }

    }

    public bool CheckIfItemInInventory(ItemData itemResource) //checks if a requested item is in the player's inventory
    {
        if (itemResource == null)
            return false;

        for (int ix = 0; ix < gridSizeWidth; ix++) //go through all tiles in the inventory grid
        {
            for (int iy = 0; iy < gridSizeHeight; iy++)
            {
                InventoryItem itemSlot = inventoryItemSlot[ix, iy];
                if (itemSlot != null && itemSlot.itemData == itemResource) //if the item is found return true
                {
                    return true;
                }
                    
            }
        }

        return false;
    }

    public List<ItemData> GetItemTypeInInventory(ItemData.ItemType itemType) //checks if a requested item type is in the player's inventory
    {
        weaponItems.Clear();

        for (int ix = 0; ix < gridSizeWidth; ix++) //go through all tiles in the inventory grid
        {
            for (int iy = 0; iy < gridSizeHeight; iy++)
            {
                InventoryItem itemSlot = inventoryItemSlot[ix, iy];
                if (itemSlot != null && itemSlot.itemData != null && itemSlot.itemData.itemType == itemType) //if the item type is found return true
                {
                    if (itemType == ItemData.ItemType.Weapon)
                        if (weaponItems.Contains(itemSlot.itemData))
                            continue;
                        else
                            weaponItems.Add(itemSlot.itemData);
                }

            }
        }
        if (itemType == ItemData.ItemType.Weapon)
            return weaponItems;

        return new List<ItemData>(); 
    }

    bool PositionCheck(int posX, int posY) //checks the current location of all tiles and makes sure they fit parameters
    {
        if(posX < 0 || posY < 0) //item is outside the boundraries for placement (negative number)
            return false;

        if (posX >= gridSizeWidth || posY >= gridSizeHeight) //item is too big for the grid, cant place
            return false;

        return true;
    }

    public bool BoundraryCheck(int posX, int posY, int itemWidth, int itemHeight) //checks to make sure an item can fit the grid
    {
        if (PositionCheck(posX, posY) == false) //check if the position is eligible first (checks top left tile of item)
            return false;

        posX += itemWidth - 1; //add item tiles to positions to get bottom right tile (min size is 1 so -> -1)
        posY += itemHeight - 1;
        if (PositionCheck(posX, posY) == false)  //check if the position is eligible first (checks bottom right tile of item)
            return false;


        return true;
    }

    public InventoryItem GetItem(int x, int y) //returns a slot location at passed in position
    {
        if (x < 0 || y < 0)
            return null;

        if (x >= gridSizeWidth || y >= gridSizeHeight)
            return null;

        return inventoryItemSlot[x, y];
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;
        for (int y = 0; y < height; y++) //go through all grids and check if a spot that matches the items dimensions can fit there
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvailableSpaces(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT) == true)
                {
                    return new Vector2Int(x, y); //return the location where the item can fit
                }
            }
        }
        return null;
    }
}
