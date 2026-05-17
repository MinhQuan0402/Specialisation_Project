using Combat.Knockback;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WeaponParry))]
public class WeaponKnockBackOnParry : WeaponComponent<WeaponKnockBakOnParryData, AttackKnockback>
{
    private WeaponParry parry;

    private Movement movement;

    private void HandleParry(GameObject parriedGameObject)
    {
        CombatKnockBackUtilities.TryKnockBack(parriedGameObject,
            new KnockBackData(currentAttackData.Angle, currentAttackData.Strength,
                movement.FacingDirection, Core.Root), out _);
    }

    protected override void Start()
    {
        base.Start();

        movement = Core.GetCoreComponent<Movement>();

        parry = GetComponent<WeaponParry>();

        parry.OnParry += HandleParry;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        parry.OnParry -= HandleParry;
    }
}