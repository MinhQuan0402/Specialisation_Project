using System.Collections;
using UnityEngine;

[System.Serializable]
public class WeaponKnockBakOnParryData : ComponentData<AttackKnockback>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponKnockBackOnParry);
    }
}