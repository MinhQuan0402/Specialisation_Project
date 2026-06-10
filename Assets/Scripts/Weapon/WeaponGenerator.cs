using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour
{
    public event Action OnWeaponGenerating;

    [SerializeField] private Weapon weapon;
    [SerializeField] private CombatInputs combatInput;

    private List<WeaponComponent> componentAlreadyOnWeapon = new();
    private readonly List<WeaponComponent> componentAddedToWeapon = new();
    private List<Type> componentDependencies = new();
    
    private Animator anim;
    private InventorySystem inventorySystem;
    
    private void Start()
    {
        inventorySystem = FindAnyObjectByType<InventorySystem>();
        inventorySystem.OnWeaponDataChanged += HandleWeaponDataChanged;

        anim = GetComponentInChildren<Animator>();

        if (inventorySystem.TryGetWeapon((int)combatInput, out var data))
        {
            GenerateWeapon(data);
        }
    }

    private void OnDestroy()
    {
        inventorySystem.OnWeaponDataChanged -= HandleWeaponDataChanged;
    }

    public void GenerateWeapon(WeaponData data)
    {
        OnWeaponGenerating?.Invoke();

        weapon.SetData(data);

        if (data == null)
        {
            weapon.SetCanEnterAttack(false);
            return;
        }

        componentAddedToWeapon.Clear();
        componentAlreadyOnWeapon.Clear();
        componentDependencies.Clear();

        componentAlreadyOnWeapon = GetComponents<WeaponComponent>().ToList();
        componentDependencies = data.GetAllDependencies();
        foreach (var dependency in componentDependencies)
        {
            if(componentAddedToWeapon.FirstOrDefault(component => component.GetType() == dependency))
                continue;
                
            var weaponComponent = 
                componentAlreadyOnWeapon.FirstOrDefault(component => component.GetType() == dependency);

            if (weaponComponent == null)
            {
                weaponComponent = gameObject.AddComponent(dependency) as WeaponComponent;
            }
            
            weaponComponent.Init();
            
            componentAddedToWeapon.Add(weaponComponent);
        }
            
        var componentsToRemove = componentAlreadyOnWeapon.Except(componentAddedToWeapon);

        foreach (var componentToRemove in componentsToRemove)
        {
            Destroy(componentToRemove);
        }

        anim.runtimeAnimatorController = data.AnimatorController;

        weapon.SetCanEnterAttack(true);
    }

    private void HandleWeaponDataChanged(int inputIndex, WeaponData data)
    {
        if (inputIndex != (int)combatInput)
            return;

        GenerateWeapon(data);
    }
}