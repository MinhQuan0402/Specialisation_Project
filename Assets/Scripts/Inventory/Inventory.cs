using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
[CreateAssetMenu(fileName = "Inventory", menuName = "Data/Inventory")]
public class Inventory : ScriptableObject
{
    public int maxStackPerItem = 20;

    private readonly WeaponData[] weapons = new WeaponData[2];

    public readonly Dictionary<ItemInstance, int> items = new ();

    public bool AddItem(ItemInstance item)
    {
        var itemType = item.itemData.objectType;
        switch(itemType)
        {
            case ItemData.ObjectType.Equipment:
                for(int i = 0; i < weapons.Length; ++i)
                {
                    if (weapons[i] == null)
                    {
                        weapons[i] = item.itemData as WeaponData;
                        return true;
                    }
                }
                break;
            case ItemData.ObjectType.Consumable:

                ItemInstance key = null;
                foreach (KeyValuePair<ItemInstance, int> pair in items)
                {

                    if (pair.Key.itemData == item.itemData)
                    {
                        if (pair.Value == maxStackPerItem) return false;
                        key = pair.Key;
                    }
                }

                if (key != null) items[key] = items[key]++;
                else items.Add(item, 1);

                return true;
        }

        return true;
    }
    public void DisplayItems()
    {
        /*foreach (ItemInstance item in items)
        {
            Debug.Log($"Item Name: {item.itemData.itemName}");
        }*/
    }

    public int GetItemQuatity(ItemInstance itemInstance)
    {
        foreach(var pair in items)
        {
            if (pair.Key.itemData == itemInstance.itemData)
            {
                return pair.Value;
            }
        }

        return 0;
    }

    public void SwapeWeapon(CombatInputs combatInputs, WeaponData newData)
    {
        weapons[(int)combatInputs] = newData;
    }

    public void RemoveItem(ItemData itemData = null)
    {
        if (itemData == null) return;
        foreach (var pair in items)
        {
            if (pair.Key.itemData == itemData)
            {
                items[pair.Key]--;
                return;
            }
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < weapons.Length; ++i) weapons[i] = null;
        items.Clear();
    }
}