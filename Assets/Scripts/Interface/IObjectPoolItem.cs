using UnityEngine;

public interface IObjectPoolItem
{
    void SetObjectPool<T>(ObjectPool pool, T comp) where T : Component;

    void Release();
}