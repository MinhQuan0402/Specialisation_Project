using UnityEngine;
using System.Collections;

public class WeaponBlockData : ComponentData<AttackBlock>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponBlock);
    }
}