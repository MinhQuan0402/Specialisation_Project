using System;
using UnityEngine;

public class WeaponSpriteData : ComponentData<AttackSprites>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponSprite);
    }
}