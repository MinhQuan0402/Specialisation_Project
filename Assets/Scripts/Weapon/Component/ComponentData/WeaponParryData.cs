using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeaponParryData : ComponentData<AttackParry>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponParry);
    }
}