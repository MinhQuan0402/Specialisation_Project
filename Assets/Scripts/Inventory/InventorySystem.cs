using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class InventorySystem : CoreComponent
{
    public event Action<int, WeaponData> OnWeaponDataChanged;

    [SerializeField] private Inventory inventory;

    protected override void Awake()
    {
        base.Awake();
        inventory.ClearAll();
    }

    public bool TryToAddItem(ItemInstance item)
    {
        if (inventory == null) return false;

        int weaponIndex = 0;
        bool result = item.itemData.objectType == ItemData.ObjectType.Consumable ? 
                      inventory.TryAddItem(item) : inventory.TrySetWeapon(item.itemData as WeaponData, 
                                                   weaponIndex = inventory.TryGetEmptyIndex(), 
                                                   out WeaponData oldWeapon);

        // Fail to add
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

        // Successful add
        switch (item.itemData.objectType)
        {
            case ItemData.ObjectType.Equipment:
                OnWeaponDataChanged?.Invoke(weaponIndex, item.itemData as WeaponData);
                UIManager.Instance.UpdateWeaponSlot(item.itemData, weaponIndex);
                break;
            case ItemData.ObjectType.Consumable:
                UIManager.Instance.UpdateItemSlot(item.itemData, inventory.TryGetItemQuatity(item));
                break;
        }

        return true;
    }

    public bool TryToAddWeapon(WeaponData weaponData)
    {
        if (inventory == null) return false;

        int weaponIndex = inventory.TryGetEmptyIndex();
        if (!inventory.TrySetWeapon(weaponData, weaponIndex, out WeaponData oldWeapon))
        {
            //Swap weapon request
            return true;
        }

        OnWeaponDataChanged?.Invoke(weaponIndex, weaponData);
        UIManager.Instance.UpdateWeaponSlot(weaponData, weaponIndex);
        return true;
    }

    public void TryToUsePrimaryItemSlot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ItemInstance item = inventory.TryGetItem(0);
            if (item != null && item.itemEffect != null &&
                item.itemEffect.Use(Player.Instance.gameObject))
            {
                inventory.RemoveItem(item.itemData);
                UIManager.Instance.UpdateItemSlot(item.itemData, inventory.TryGetItemQuatity(item));
            }
        }
    }

    public void TryToUseSecondaryItemSlot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ItemInstance item = inventory.TryGetItem(1);
            if (item != null && item.itemEffect != null &&
                item.itemEffect.Use(Player.Instance.gameObject))
            {
                inventory.RemoveItem(item.itemData);
                UIManager.Instance.UpdateItemSlot(item.itemData, inventory.TryGetItemQuatity(item));
            }
        }
    }

    public bool TryGetWeapon(int combatInput, out WeaponData data)
    {
        return inventory.TryGetWeapon(combatInput, out data);
    }
}
