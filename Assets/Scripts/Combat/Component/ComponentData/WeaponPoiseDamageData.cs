using UnityEngine;
[System.Serializable]
public class WeaponPoiseDamageData : ComponentData<AttackPoiseDamage>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponPoiseDamage);
    }
}