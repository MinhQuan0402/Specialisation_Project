using Combat.Damage;
using Combat.Knockback;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttackAction", menuName = "PluggableAI/Actions/MeleeAttackAction")]
public class MeleeAttackAction : EnemyAction
{
    public override void Act(EnemyController controller)
    {
        if(controller.isAttacking)
        {
            // Check for player in attack range and apply damage
            if (EnemyUtilities.PlayerInSight(controller, controller.TryGetAttackRange(), 
                                             controller.Data.whatAreDetectibles))
            {
                // Apply damage to player
                // This is a placeholder for the actual damage application logic
                Debug.Log("Player hit by melee attack!");
                if (Player.Instance.Core.TryGetCoreComponent(out DamageReceiver damageReceiver))
                {
                    DamageData damageData = new(controller.Data.attackDetails[controller.currentAttackIndex].damageAmount, controller.gameObject);
                    damageReceiver.Damage(damageData);
                }

                if(Player.Instance.Core.TryGetCoreComponent(out KnockbackReceiver knockbackReceiver))
                {
                    KnockBackData knockbackData = new(controller.Data.attackDetails[controller.currentAttackIndex].knockbackAngle,
                                                      controller.Data.attackDetails[controller.currentAttackIndex].knockbackStrength,
                                                      controller.Movement.FacingDirection,
                                                      controller.gameObject);
                    knockbackReceiver.KnockBack(knockbackData);
                }

                controller.isAttacking = false; // Reset attacking state after applying damage
            }
        }
    }
}