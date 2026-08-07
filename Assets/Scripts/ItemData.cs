using UnityEngine;
//RESOURCE SINGLETON
[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public int width = 1; //how many grids w x h does the item take up
    public int height = 1;

    public Sprite itemIcon;
    public ItemType itemType;
    public enum ItemType
    {
        Weapon,
        Consumable
    }
}

