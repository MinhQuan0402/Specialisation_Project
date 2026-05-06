using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField] private UIInventoryItem itemPrefab;

    [SerializeField] private RectTransform contentPanel;

    [SerializeField] private UIInventoryDescription itemDescription;

    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    private void Awake()
    {
        itemDescription.ResetDescription();
    }

    public void InitializeInventoryList(int size)
    {
        for(int i = 0; i < size; i++)
        {
            UIInventoryItem item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(contentPanel, false);
            listOfUIItems.Add(item);
            item.OnItemClick += HandleItemSelection;
            item.OnItemBeginDrag += HandleItemBeginDrag;
            item.OnItemDropOn += HandleItemDropOn;
            item.OnItemEndDrag += HandleItemEndDrag;
            item.OnRightMouseBtnClicked += HandleShowItemActions;
        }
    }

    private void HandleShowItemActions(UIInventoryItem item)
    {
        if (item.itemImage == null || item.itemImage.sprite == null)
        {
            return;
        }

        if (item.ItemInstance.itemData.objectType == ItemData.ObjectType.Equipment)
        {
            WeaponEffect weaponEffect = item.ItemInstance.itemEffect as WeaponEffect;

            if(weaponEffect.weapon == null)
            {
                item.Select();
                weaponEffect.Use(Player.Instance.gameObject);
                weaponEffect.weapon.GetComponent<Weapon>().IsEquipped = true;
                return;
            }

            Weapon weapon = weaponEffect.weapon.GetComponent<Weapon>();
           
            if(!weapon.IsEquipped)
            {
                item.Select();
                weaponEffect.Use(Player.Instance.gameObject);
                weapon.IsEquipped = true;
            }
            else
            {
                item.Deselect();
                weaponEffect.Unuse(Player.Instance.gameObject);
                weapon.IsEquipped = false;
            }
        }

        if(item.ItemInstance.itemData.objectType == ItemData.ObjectType.Consumable)
        {
            ItemEffect consumableEffect = item.ItemInstance.itemEffect as ItemEffect;
            if (consumableEffect != null)
            {
                consumableEffect.Use(Player.Instance.gameObject);
                //Player.Instance.InventorySystem.inventory.RemoveItem(item.ItemInstance);
                item.ResetData();
                itemDescription.ResetDescription();
            }
        }
    }

    private void HandleItemEndDrag(UIInventoryItem item)
    {
        throw new NotImplementedException();
    }

    private void HandleItemDropOn(UIInventoryItem item)
    {
        throw new NotImplementedException();
    }

    private void HandleItemBeginDrag(UIInventoryItem item)
    {
        throw new NotImplementedException();
    }

    private void HandleItemSelection(UIInventoryItem item)
    {
        if (item.itemImage == null || item.itemImage.sprite == null)
        {
            itemDescription.ResetDescription();
            return;
        }
        itemDescription.SetDescription(item.ItemInstance.itemData.itemImage, item.ItemInstance.itemData.itemName, item.ItemInstance.itemData.itemDescription);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        itemDescription.ResetDescription();

        /*Inventory inventory = Player.Instance.InventorySystem.inventory;

        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i] == null) break;

            if (i < listOfUIItems.Count)
            {
                listOfUIItems[i].SetData(inventory.items[i].itemData.itemImage);
                listOfUIItems[i].ItemInstance = inventory.items[i];
            }
        }*/
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
