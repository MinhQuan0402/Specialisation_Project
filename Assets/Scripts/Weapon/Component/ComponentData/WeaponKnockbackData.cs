using UnityEngine;
[System.Serializable]
public class WeaponKnockbackData : ComponentData<AttackKnockback>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponKnockback);
    }
}