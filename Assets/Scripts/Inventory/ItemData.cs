using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ObjectType
    {
        Consumable,
        Equipment,
        Soul,
    }

    [Header("Item Properties")]
    public string itemName;
    [TextArea(3, 5)]
    public string itemDescription;
    public Sprite itemImage;
    public ObjectType objectType;
}
