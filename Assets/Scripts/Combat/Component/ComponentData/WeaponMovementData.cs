using UnityEngine;

public class WeaponMovementData : ComponentData<AttackMovement>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponMovement);
    }
}
