using UnityEngine;

public class WeaponPoiseDamage : WeaponComponent<WeaponPoiseDamageData, AttackPoiseDamage>
{
    private WeaponActionHitbox hitbox;

    private void HandleDetectedColliders2D(Collider2D[] colliders)
    {
        
    }
    
    protected override void Start()
    {
        base.Start();
        hitbox = GetComponent<WeaponActionHitbox>();
        hitbox.OnDetectedColliders2D += HandleDetectedColliders2D;
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        hitbox.OnDetectedColliders2D -= HandleDetectedColliders2D;
    }
}