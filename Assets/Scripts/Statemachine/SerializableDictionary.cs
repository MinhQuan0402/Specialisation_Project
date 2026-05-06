using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyValue<TKey, TValue>
{
    public TKey Key;
    public TValue Value;

    public KeyValue(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    public List<KeyValue<TKey, TValue>> pairs = new();

    public Dictionary<TKey, TValue> ToDictionary()
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var pair in pairs)
        {
            if (!dict.ContainsKey(pair.Key))
                dict[pair.Key] = pair.Value;
        }
        return dict;
    }

    public void FromDictionary(Dictionary<TKey, TValue> dict)
    {
        pairs.Clear();
        foreach (var kvp in dict)
        {
            pairs.Add(new KeyValue<TKey, TValue>(kvp.Key, kvp.Value));
        }
    }
}
