using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    const float tileSizeWidth = 64; //grid boxes have size of 64x64px
    const float tileSizeHeight = 64;

    InventoryItem[,] inventoryItemSlot;

    RectTransform rectTransform;

    [SerializeField] int gridSizeWidth; //change to resize grid (i.e. 5 x 10) = (320 x 640px)
    [SerializeField] int gridSizeHeight;

    [SerializeField] GameObject inventoryItemPrefab;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);

        InventoryItem inventoryItem = Instantiate(inventoryItemPrefab).GetComponent<InventoryItem>();
        PlaceItem(inventoryItem, 1, 1);
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem pickedUpItem = inventoryItemSlot[x, y]; //get the location where the item was picked up
        inventoryItemSlot[x, y] = null; //reset variable
        return pickedUpItem; //return the location where picked up
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

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY) //places item at an x an y pos on the grid
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);
        inventoryItemSlot[posX, posY] = inventoryItem;

        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth / 2 + tileSizeWidth / 4; //working with 32px asset but scaled 2x so visually 64px requires /2 and /4 for mouse to grid conversion
        position.y = -(posY * tileSizeHeight / 2 + tileSizeHeight / 4);

        rectTransform.localPosition = position;
    }
}
