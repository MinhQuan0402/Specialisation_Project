using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using Unity.Collections;

public class Core : MonoBehaviour
{
    [SerializeField] private List<CoreComponent> CoreComponents = new();
    [field: SerializeField] public GameObject Root { get; private set; }

    private void Awake()
    {
        Root = Root ? Root : transform.parent.gameObject;
    }
    
    public void LogicUpdate()
    {
        foreach(CoreComponent components in CoreComponents)
        {
            components.LogicUpdate();
        }
    }

    public void AddComponent(CoreComponent component)
    {
        if (CoreComponents.Contains(component)) return;
        CoreComponents.Add(component);
    }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    //To get component from core component list
    public T GetComponent<T>() where T:CoreComponent //Contraint the type of T
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        var component = CoreComponents.OfType<T>().FirstOrDefault();
        if(component) return component;

        component = GetComponentInChildren<T>();
        if(component) return component;

        Debug.LogWarning($"{typeof(T)} not found on {transform.parent.name}");
        return null;
    }

    //To get core component and assign the reference variable as the return value
    public T GetCoreComponent<T>(ref T value) where T:CoreComponent
    {
        value = GetComponent<T>();
        return value;
    }
}
