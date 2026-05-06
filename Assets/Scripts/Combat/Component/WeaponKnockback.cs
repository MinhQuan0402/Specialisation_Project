using Combat.Knockback;
using UnityEngine;
using static Combat.Knockback.CombatKnockBackUtilities;

public class WeaponKnockback : WeaponComponent<WeaponKnockbackData, AttackKnockback>
{
    private WeaponActionHitbox hitbox;
    
    private CoreComp<Movement> movement;

    private void HandleDetectedColliders2D(Collider2D[] colliders)
    {
        TryKnockBack(colliders, new KnockBackData(currentAttackData.Angle, currentAttackData.Strength, movement.Comp.FacingDirection, Core.Root), out _);
    }

    protected override void HandleEnter()
    {
        base.HandleEnter();

        movement ??= new CoreComp<Movement>(Core);
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