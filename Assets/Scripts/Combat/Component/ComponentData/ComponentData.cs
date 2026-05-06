using System;
using UnityEngine;

[Serializable]
public abstract class ComponentData
{
    [field: SerializeField, HideInInspector] public string Name { get; private set; }
    
    public Type ComponentDependency { get; protected set; }
    
    public ComponentData()
    {
        SetComponentName();
        SetComponentDenpendency();
    }

    public void SetComponentName() => Name = GetType().Name.Replace("Weapon", "");
    
    protected abstract void SetComponentDenpendency();
    
    public virtual void SetAttackDataNames() { }
    public virtual void InitializeAttackData(int numberOfAttack) { }
}

[Serializable]
public abstract class ComponentData<T> : ComponentData where T : AttackData
{
    [SerializeField] private bool repeatData;
    
    [SerializeField] private T[] attackData;

    public T GetAttackData(int index) => attackData[repeatData ? 0 : index];
    
    public T[] GetAllAttackData() => attackData;

    public override void SetAttackDataNames()
    {
        base.SetAttackDataNames();
        for (var i = 0; i < attackData.Length; i++)
        {
            attackData[i].SetAttackName(i + 1);
        }
    }

    public override void InitializeAttackData(int numberOfAttacks)
    {
        base.InitializeAttackData(numberOfAttacks);

        var newLen = repeatData ? 1 : numberOfAttacks;
            
        var oldLen = attackData != null ? attackData.Length : 0;
            
        if(oldLen == newLen)
            return;
            
        Array.Resize(ref attackData, newLen);

        if (oldLen < newLen)
        {
            for (var i = oldLen; i < attackData.Length; i++)
            {
                var newObj = Activator.CreateInstance(typeof(T)) as T;
                attackData[i] = newObj;
            }
        }
            
        SetAttackDataNames();
    }
}
