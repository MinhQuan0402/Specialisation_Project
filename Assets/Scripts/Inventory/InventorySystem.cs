using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class InventorySystem : CoreComponent
{
    public event Action<int, WeaponData> OnWeaponDataChanged;
    public event Action<WeaponSwapChoiceRequest> OnChoiceRequested;
    public event Action<ItemInstance> OnWeaponDiscarded;

    private InteractableDetector interactableDetector;
    [SerializeField] private Inventory inventory;

    private ItemData newItemData;

    private Item itemPickup;

    protected override void Awake()
    {
        base.Awake();
        inventory.ClearAll();
        interactableDetector = core.GetCoreComponent<InteractableDetector>();
    }

    public int TotalKey => inventory.GetTotalKeys();

    public bool TryToAddItem(ItemInstance item)
    {
        if (inventory == null) return false;

        bool result = inventory.TryAddItem(item);

        if (!result) return false;

        UIManager.Instance.UpdateItemSlot(item.itemData, inventory.TryGetItemQuatity(item));
        return true;
    }

    public bool TryToAddWeapon(WeaponData weaponData)
    {
        if (inventory == null) return false;

        if (inventory.TryGetEmptyIndex(out var index))
        {
            OnWeaponDataChanged?.Invoke(index, weaponData);
            UIManager.Instance.UpdateWeaponSlot(weaponData, index);
            inventory.TrySetWeapon(weaponData, index, out _);
            return true;
        }

        OnChoiceRequested?.Invoke(new WeaponSwapChoiceRequest(
            HandleWeaponSwapChoice,
            inventory.GetWeaponSwapChoices(),
            weaponData
        ));

        
        return false;
    }

    private void HandleTryInteract(IInteractable interactable)
    {
        if (interactable is not Item pickup)
            return;

        itemPickup = pickup;

        newItemData = itemPickup.GetItemContext.itemData;

        bool success = false;
        switch (newItemData.objectType)
        {
            case ItemData.ObjectType.Consumable:
                success = TryToAddItem(itemPickup.GetItemContext);
                break;
            case ItemData.ObjectType.Equipment:
                WeaponData weaponData = newItemData as WeaponData;
                success = TryToAddWeapon(weaponData);
                break;
            case ItemData.ObjectType.Key:
                success = true;
                inventory.AddKey(newItemData);
                UIManager.Instance.UpdateKeysUI(inventory.GetTotalKeys());
                break;
        }

        if (success)
        {
            newItemData = null;
            interactable.OnInteract();
        }
    }

    private void HandleWeaponSwapChoice(WeaponSwapChoice choice)
    {
        WeaponData weaponData = newItemData as WeaponData;
        if (!inventory.TrySetWeapon(weaponData, choice.Index, out var oldData))
            return;

        OnWeaponDataChanged?.Invoke(choice.Index, weaponData);
        UIManager.Instance.UpdateWeaponSlot(weaponData, choice.Index);

        newItemData = null;

        OnWeaponDiscarded?.Invoke(new ItemInstance(oldData, null));

        if (itemPickup == null)
            return;

        itemPickup.OnInteract();
    }

    public void TryToUsePrimaryItemSlot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ItemInstance item = inventory.TryGetItem((int)CombatInputs.primary);
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
            ItemInstance item = inventory.TryGetItem((int)CombatInputs.secondary);
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

    private void OnEnable()
    {
        interactableDetector.OnTryInteract += HandleTryInteract;
    }


    private void OnDisable()
    {
        interactableDetector.OnTryInteract -= HandleTryInteract;
    }
}
