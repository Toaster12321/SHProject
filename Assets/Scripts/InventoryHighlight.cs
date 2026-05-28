using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] RectTransform highlighter;

    public void Show(bool b)
    {
        highlighter.gameObject.SetActive(b);
    }

    public void SetSize(InventoryItem targetItem)
    {
        Vector2 size = new Vector2();
        size.x = targetItem.itemData.width * ItemGrid.tileSizeWidth; //get item's width and height then set rect transform to it
        size.y = targetItem.itemData.height * ItemGrid.tileSizeHeight;
        highlighter.sizeDelta = size;
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem) //gets item location
    {
        SetParent(targetGrid);

        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, targetItem.onGridPositionX, targetItem.onGridPositionY);

        highlighter.localPosition = pos;
    }

    public void SetParent(ItemGrid targetGrid)
    {
        if (targetGrid == null)
            return;

        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

        highlighter.localPosition = pos;

    }
}
