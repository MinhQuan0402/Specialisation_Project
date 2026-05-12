using UnityEngine;
using System.Collections;
using System;

public class WeaponBlock : WeaponComponent<WeaponBlockData, AttackBlock>
{
    // Event fired off when an attack is blocked, passing the GameObject of the entity that was blocked as a parameter
    public event Action<GameObject> OnBlock;

    // Players DamageReceiver, KnockbackReceiver, and PoiseDamageReceiver components to apply block modifiers to when blocking an attack
    private DamageReceiver damageReceiver;
    private KnockbackReceiver knockbackReceiver;
    private PoiseDamageReceiver poiseDamageReceiver;

    // Modifiers to apply to the player's damage, knockback, and poise damage when blocking an attack
    private DamageModifier damageModifier;
    private BlockKnockBackModifier blockKnockBackModifier;
    private BlockPoiseDamageModifier blockPoiseDamageModifier;

    private Movement movement;
    private ParticleManager particleManager;

    private bool isBlockWindowActive;
    private bool shouldUpdate;

    private float nextWindowTriggerTime;

    // Starts the block window by passing modifiers to receivers
    private void StartBlockWindow()
    {
        Debug.Log("Start Block");

        isBlockWindowActive = true;
        shouldUpdate = false;

        damageModifier.OnModified += HandleModified;

        damageReceiver.Modifiers.AddModifier(damageModifier);
        knockbackReceiver.Modifiers.AddModifier(blockKnockBackModifier);
        poiseDamageReceiver.Modifiers.AddModifier(blockPoiseDamageModifier);
    }

    // Stops the block window by removing modifiers from receivers
    private void StopBlockWindow()
    {
        Debug.Log("End Block");

        isBlockWindowActive = false;
        shouldUpdate = false;

        damageModifier.OnModified -= HandleModified;

        damageReceiver.Modifiers.RemoveModifier(damageModifier);
        knockbackReceiver.Modifiers.RemoveModifier(blockKnockBackModifier);
        poiseDamageReceiver.Modifiers.RemoveModifier(blockPoiseDamageModifier);
    }

    // Check if source falls within any blocked regions for the current attack. Also return the block information
    private bool IsAttackBlocked(Transform source, out DirectionalInformation directionalInformation)
    {
        var angleOfAttacker = AngleUtilities.AngleFromFacingDirection(
            Core.Root.transform,
            source,
            movement.FacingDirection
            );

        return currentAttackData.IsBlocked(angleOfAttacker, out directionalInformation);
    }

    // Handler for when the damage modifier is modified, which happens when an attack is blocked. Plays block particles and fires OnBlock event.
    private void HandleModified(GameObject source)
    {
        particleManager.StartParticleWithRandomRotation(currentAttackData.Particles, currentAttackData.ParticleOffset);
        OnBlock?.Invoke(source);
    }

    private void HandleEnterAttackPhase(AttackPhases phase)
    {
        shouldUpdate = isBlockWindowActive
            ? currentAttackData.BlockWindowEnd.TryGetTriggerTime(phase, out nextWindowTriggerTime)
            : currentAttackData.BlockWindowStart.TryGetTriggerTime(phase, out nextWindowTriggerTime);
    }

    protected override void Start()
    {
        base.Start();

        movement = Core.GetCoreComponent<Movement>();
        particleManager = Core.GetCoreComponent<ParticleManager>();

        damageReceiver = Core.GetCoreComponent<DamageReceiver>();
        knockbackReceiver = Core.GetCoreComponent<KnockbackReceiver>();
        poiseDamageReceiver = Core.GetCoreComponent<PoiseDamageReceiver>();

        damageModifier = new DamageModifier(IsAttackBlocked);
        blockKnockBackModifier = new BlockKnockBackModifier(IsAttackBlocked);
        blockPoiseDamageModifier = new BlockPoiseDamageModifier(IsAttackBlocked);

        eventHandler.OnEnterAttackPhase += HandleEnterAttackPhase;
    }

    private void Update()
    {
        if (!shouldUpdate || !IsPastTriggerTime())
            return;

        if (isBlockWindowActive)
        {
            StopBlockWindow();
        }
        else
        {
            StartBlockWindow();
        }
    }

    private bool IsPastTriggerTime()
    {
        return Time.time >= nextWindowTriggerTime;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        eventHandler.OnEnterAttackPhase -= HandleEnterAttackPhase;
    }
}