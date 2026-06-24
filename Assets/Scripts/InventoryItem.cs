using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public int HEIGHT //switch height/width based on object's rotation
    {
        get
        {
            if (rotated == false)
            {
                return itemData.height;
            }
            return itemData.width; //returns the width of the objects item if we are rotated
        }
    }

    public int WIDTH
    {
        get
        {
            if (rotated == false)
            {
                return itemData.width;
            }
            return itemData.height; //returns the height of the objects item if we are rotated
        }
    }

    public int onGridPositionX;
    public int onGridPositionY;

    public bool rotated = false;

   
    internal void Set(ItemData itemData) //sets the passed ItemData into the on-screen icon with its width and height
    {
        this.itemData = itemData;

        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = itemData.width * ItemGrid.tileSizeWidth;
        size.y = itemData.height * ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    internal void Rotate()
    {
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, rotated == true ? 90f : 0f); // if its already rotated rotate from 0 -> 90degrees, otherwise rotate from 90 -> 0 degrees on the z axis

    }

}
