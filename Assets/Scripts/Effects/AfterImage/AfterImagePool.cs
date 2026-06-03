using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    private static readonly Dictionary<EntityId, AfterImagePool> instances = new();
    public static AfterImagePool GetInstance(EntityId id)
    {
        if (instances.ContainsKey(id)) 
            return instances[id];
        Debug.LogError("Does not found instance. Please create one!");
        return null;
    }

    [SerializeField]
    private GameObject ownerOfThePool;

    [SerializeField]
    private AfterImageSprite afterImagePrefab;

    [SerializeField, Min(1)]
    private int growSize = 10;

    private readonly Queue<AfterImageSprite> availableObjects = new();

    private void Awake()
    {
        if (ownerOfThePool == null) Debug.LogError("Owner is empty");

        if (instances.ContainsKey(ownerOfThePool.GetEntityId()))
        {
            Debug.LogError($"This {ownerOfThePool.name} is " +
                $"already have a image pool [Duplicate]");
            return;
        }

        instances.Add(ownerOfThePool.GetEntityId(), this);
        GrowPool();
    }

    private void GrowPool()
    {
        for(int i = 0; i < growSize; ++i)
        {
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.OnCreate(ownerOfThePool);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(AfterImageSprite instance)
    {
        instance.gameObject.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public AfterImageSprite GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue();
        instance.gameObject.SetActive(true);
        return instance;
    }
}
