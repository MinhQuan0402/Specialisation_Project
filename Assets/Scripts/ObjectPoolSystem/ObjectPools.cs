using System.Collections.Generic;
using UnityEngine;

public class ObjectPools
{
    private readonly Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();
        
    public ObjectPool<T> GetPool<T>(T prefab, int startCount = 1) where T : Component
    {
        if (!pools.ContainsKey(prefab.name))
        {
            pools[prefab.name] = new ObjectPool<T>(prefab, startCount);
        }

        return (ObjectPool<T>)pools[prefab.name];
    }

    public T GetObject<T>(T prefab, int startCount = 1) where T: Component
    {
        return GetPool(prefab, startCount).GetObject();
    }
        
    public void ReturnObject<T>(T obj) where T : Component
    {
        var objPool = GetPool(obj);
        objPool.ReturnObject(obj);
    }

    public void Release()
    {
        foreach (var pool in pools)
        {
            pool.Value.Release();
        }
    }
}