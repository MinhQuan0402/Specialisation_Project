using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ObjectType
    {
        Consumable,
        Equipment
    }

    [Header("Item Properties")]
    public string itemName;
    public string itemDescription;
    public Sprite itemImage;
    public ObjectType objectType;
}
