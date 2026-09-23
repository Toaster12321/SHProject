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

    public void Set(ItemData itemData) //sets the passed ItemData into the on-screen icon with its width and height
    {
        this.itemData = itemData;
        rotated = false;

        GetComponent<Image>().sprite = itemData.itemIcon;

        UpdateVisualSize();
        UpdateVisualRotation();
    }

    public void Rotate()
    {
        rotated = !rotated;

        UpdateVisualSize();
        UpdateVisualRotation();

    }

    public void SetRotation(bool isRotated) //used to set rotation of items when loading inventory data
    {
        rotated = isRotated;

        UpdateVisualSize();
        UpdateVisualRotation();
    }

    public void UpdateVisualSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(
            WIDTH * ItemGrid.tileSizeWidth,
            HEIGHT * ItemGrid.tileSizeHeight
            );
    }

    public void UpdateVisualRotation()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, rotated == true ? 90f : 0f); // if its already rotated rotate from 0 -> 90degrees, otherwise rotate from 90 -> 0 degrees on the z axis
    }

}
