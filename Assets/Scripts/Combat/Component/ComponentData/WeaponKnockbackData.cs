using UnityEngine;

public class WeaponKnockbackData : ComponentData<AttackKnockback>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponKnockback);
    }
}