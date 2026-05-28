using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryManager inventoryManager;
    ItemGrid itemGrid;
    private void Awake()
    {
        inventoryManager = FindFirstObjectByType(typeof(InventoryManager)) as InventoryManager;
        itemGrid = GetComponent<ItemGrid>();
    }

    public void OnPointerEnter(PointerEventData eventData) //trigger event when cursor enters itemGrid
    {
        inventoryManager.SelectedItemGrid = itemGrid;
    }

    public void OnPointerExit(PointerEventData eventData) //trigger event when cursor exits itemGrid
    {
        inventoryManager.SelectedItemGrid = null;
    }
}
