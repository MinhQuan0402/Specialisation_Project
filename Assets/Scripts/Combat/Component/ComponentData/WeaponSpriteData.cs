using System;
using UnityEngine;
[System.Serializable]
public class WeaponSpriteData : ComponentData<AttackSprites>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponSprite);
    }
}