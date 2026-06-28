using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Inventory", menuName = "Data/Inventory")]
public class Inventory : ScriptableObject
{
    public int maxStackPerItem = 20;

    private readonly WeaponData[]                  weapons   = new WeaponData[2];
    private readonly List<ItemInstance>            itemSlots = new();
    private readonly Dictionary<ItemInstance, int> items     = new();
    private readonly List<ItemData>                keys      = new();

    public void AddKey(ItemData key) => keys.Add(key);
    public void RemoveAllKeys() => keys.Clear();
    public int GetTotalKeys() => keys.Count;

    public bool TryAddItem(ItemInstance item)
    {
        ItemInstance key = null;
        foreach (KeyValuePair<ItemInstance, int> pair in items)
        {

            if (pair.Key.itemData == item.itemData)
            {
                if (pair.Value == maxStackPerItem) return false;
                key = pair.Key;
                break;
            }
        }

        if (key != null) items[key]++;
        else { itemSlots.Add(item); items.Add(item, 1); }
        return true;
    }

    public int TryGetItemQuatity(ItemInstance itemInstance)
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

    public ItemInstance TryGetItem(int slotIndex)
    {
        if (slotIndex >= itemSlots.Count || slotIndex < 0) return null;
        return itemSlots[slotIndex];
    }

    public void RemoveItem(ItemData itemData)
    {
        if (itemData == null) return;

        ItemInstance instance = null;
        foreach (var pair in items)
        {
            if (pair.Key.itemData == itemData)
            {
                instance = pair.Key;
                break;
            }
        }

        if (instance != null)
        {

            items[instance]--;
            if (items[instance] == 0)
            {
                itemSlots.Remove(instance);
                items.Remove(instance);
            }
        }
    }

    public bool TrySetWeapon(WeaponData newData, int index, out WeaponData oldData)
    {
        if (index >= weapons.Length || index < 0)
        {
            oldData = null;
            return false;
        }

        oldData = weapons[index];
        weapons[index] = newData;

        return true;
    }

    public bool TryGetWeapon(int index, out WeaponData data)
    {
        if (index >= weapons.Length)
        {
            data = null;
            return false;
        }

        data = weapons[index];
        return true;
    }

    public bool TryGetEmptyIndex(out int index)
    {
        for (var i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                continue;

            index = i;
            return true;
        }

        index = -1;
        return false;
    }

    public int TryGetEmptyIndex()
    {
        for (var i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                continue;

            return i;
        }

        return -1;
    }

    public WeaponSwapChoice[] GetWeaponSwapChoices()
    {
        var choices = new WeaponSwapChoice[weapons.Length];

        for (var i = 0; i < weapons.Length; i++)
        {
            var data = weapons[i];

            choices[i] = new WeaponSwapChoice(data, i);
        }

        return choices;
    }

    public void ClearAll()
    {
        for (int i = 0; i < weapons.Length; ++i) weapons[i] = null;
        items.Clear();
        itemSlots.Clear();
        keys.Clear();
    }
}