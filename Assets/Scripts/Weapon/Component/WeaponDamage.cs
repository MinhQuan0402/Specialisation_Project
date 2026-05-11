using static Combat.Damage.CombatDamageUtilities;
using UnityEngine;
using Combat.Damage;

public class WeaponDamage : WeaponComponent<WeaponDamageData, AttackDamage>
{
    private WeaponActionHitbox hitbox;
    
    private void HandleDetectedCollider2D(Collider2D[] colliders)
    {
        TryDamage(colliders, new DamageData(currentAttackData.Damage, Core.Root), out _);
    }

    protected override void Start()
    {
        base.Start();
        hitbox = GetComponent<WeaponActionHitbox>();
        hitbox.OnDetectedColliders2D += HandleDetectedCollider2D;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        hitbox.OnDetectedColliders2D -= HandleDetectedCollider2D;
    }
}