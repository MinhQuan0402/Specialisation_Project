using UnityEngine;

public class WeaponActionHitboxData : ComponentData<AttackActionHitbox>
{
    [field: SerializeField] public LayerMask DetectableLayers {  get; private set; }
    
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponActionHitbox);
    }
}
