using UnityEngine;
using UnityEngine.InputSystem;
public class InventorySystem : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    public bool TryToAddItem(ItemInstance item)
    {
        if (inventory == null) return false;

        bool result = inventory.AddItem(item);

        if (!result)
        {
            switch(item.itemData.objectType)
            {
                case ItemData.ObjectType.Equipment:
                    //Swape weapon;
                    break;
            }

            return false;
        }

        switch (item.itemData.objectType)
        {
            case ItemData.ObjectType.Equipment:
                //Swape weapon;
                break;
            case ItemData.ObjectType.Consumable:
                UIManager.Instance.UpdateItemSlot(item.itemData, inventory.GetItemQuatity(item));
                break;
        }

        return result;
    }


}
