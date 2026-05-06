using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private WeaponData data;

    private List<WeaponComponent> componentAlreadyOnWeapon = new List<WeaponComponent>();
    private readonly List<WeaponComponent> componentAddedToWeapon = new List<WeaponComponent>();
    private List<Type> componentDependencies = new List<Type>();
    
    private Animator anim;
    
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        GenerateWeapon(data);
    }

    public void GenerateWeapon(WeaponData data)
    {
        weapon.SetData(data); 
            
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
    }
}