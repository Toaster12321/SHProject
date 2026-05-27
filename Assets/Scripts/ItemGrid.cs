using NUnit.Framework.Interfaces;
using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 64; //grid boxes have size of 64x64px
    public const float tileSizeHeight = 64;

    InventoryItem[,] inventoryItemSlot;

    RectTransform rectTransform;

    [SerializeField] int gridSizeWidth; //change to resize grid (i.e. 5 x 10) = (320 x 640px)
    [SerializeField] int gridSizeHeight;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem pickedUpItem = inventoryItemSlot[x, y]; //get the location where the item was picked up

        if (pickedUpItem == null)
            return null;
        CleanGridReferences(pickedUpItem);

        return pickedUpItem; //return the location where picked up
    }

    private void CleanGridReferences(InventoryItem pickedUpItem)
    {
        for (int ix = 0; ix < pickedUpItem.itemData.width; ix++) //go through all tiles in item data size
        {
            for (int iy = 0; iy < pickedUpItem.itemData.height; iy++)
            {
                inventoryItemSlot[pickedUpItem.onGridPositionX + ix, pickedUpItem.onGridPositionY + iy] = null; //reset variable on all tiles
            }
        }
    }

    private void Init(int width, int height) //function to resize the grid height/width boxes and item sizes
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2((width * tileSizeWidth) / 2 , (height * tileSizeHeight) / 2); //divide by 2 since using 2x scaled texture 
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
        if (BoundraryCheck(posX, posY, inventoryItem.itemData.width, inventoryItem.itemData.height) == false)
        {
            return false; //not able to place item
        }

        if (OverlapCheck(posX, posY, inventoryItem.itemData.width, inventoryItem.itemData.height, ref overlappedItem) == false)
        {
            overlappedItem = null; //reset item stored
            return false;
        }

        if (overlappedItem != null)
        {
            CleanGridReferences(overlappedItem);
        }

        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for(int x = 0; x < inventoryItem.itemData.width; x++) //go through all tiles in the grid based off the size of the item, allow all tiles to be selected
        {
            for(int y = 0; y < inventoryItem.itemData.height; y++)
            {
                inventoryItemSlot[posX + x , posY + y] = inventoryItem;
            }
        }
        
        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;

        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth / 2 + tileSizeWidth * inventoryItem.itemData.width / 4; //working with 32px asset but scaled 2x so visually 64px requires /2 and /4 for mouse to grid conversion
        position.y = -(posY * tileSizeHeight / 2 + tileSizeHeight * inventoryItem.itemData.height / 4);

        rectTransform.localPosition = position;

        return true; //able to place item
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

    bool PositionCheck(int posX, int posY)
    {
        if(posX < 0 || posY < 0) //item is outside the boundraries for placement (negative number)
            return false;

        if (posX >= gridSizeWidth || posY >= gridSizeHeight) //item is too big for the grid, cant place
            return false;

        return true;
    }

    bool BoundraryCheck(int posX, int posY, int itemWidth, int itemHeight)
    {
        if (PositionCheck(posX, posY) == false) //check if the position is eligible first (checks top left tile of item)
            return false;

        posX += itemWidth - 1; //add item tiles to positions to get bottom right tile (min size is 1 so -> -1)
        posY += itemHeight - 1;

        if (PositionCheck(posX, posY) == false)
            return false;


        return true;
    }
}
