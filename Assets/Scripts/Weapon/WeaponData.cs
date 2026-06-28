using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WeaponType
{
    Melee = 0,
    Ranged = 1,
    Defense = 2,
}

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Weapon Data/Basic Weapon Data")]
public class WeaponData : ItemData
{
    [Header("Weapon Properties")]
    [field: SerializeField] public WeaponType WeaponType { get; private set; }

    [field: SerializeField] public float WeaponStamina { get; private set; } = 15.0f;
    [field: SerializeField] public float AttackSpeed { get; private set; } = 1.4f;

    [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; private set; }
    
    [field: SerializeField] public int NumberOfAttacks {  get; private set; }

    [field: SerializeReference] public List<ComponentData> ComponentData { get; private set; }

    [field: SerializeField] public string PassiveName { get; private set; }
    [field: SerializeField, TextArea(2, 4)] public string PassiveDesc { get; private set; }

    public T GetData<T>()
    {
        return ComponentData.OfType<T>().FirstOrDefault();
    }

    public List<Type> GetAllDependencies()
    {
        return ComponentData.Select(component => component.ComponentDependency).ToList();
    }

    public void AddData(ComponentData data)
    {
        if (ComponentData.FirstOrDefault(t => t.GetType() == data.GetType()) != null)
            return;

        ComponentData.Add(data);
    }
}
