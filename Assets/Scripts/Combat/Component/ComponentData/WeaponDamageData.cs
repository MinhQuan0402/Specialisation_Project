using UnityEngine;

[System.Serializable]
public class WeaponDamageData : ComponentData<AttackDamage>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponDamage);
    }
}