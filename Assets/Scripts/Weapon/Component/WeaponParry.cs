using System;
using System.Collections;
using UnityEngine;

public class WeaponParry : WeaponComponent<WeaponParryData, AttackParry>
{
    private static readonly int ParryHash = Animator.StringToHash("parry");

    public event Action<GameObject> OnParry;

    private DamageReceiver damageReceiver;
    private KnockbackReceiver knockBackReceiver;
    private PoiseDamageReceiver poiseDamageReceiver;

    private DamageModifier damageModifier;
    private BlockKnockBackModifier knockBackModifier;
    private BlockPoiseDamageModifier poiseDamageModifier;

    private Movement movement;
    private ParticleManager particleManager;

    private bool isBlockWindowActive;
    private bool shouldUpdate;

    private float nextWindowTriggerTime;

    private void StartParryWindow()
    {
        isBlockWindowActive = true;
        shouldUpdate = false;

        damageModifier.OnModified += HandleParry;

        damageReceiver.Modifiers.AddModifier(damageModifier);
        knockBackReceiver.Modifiers.AddModifier(knockBackModifier);
        poiseDamageReceiver.Modifiers.AddModifier(poiseDamageModifier);
    }

    private void StopParryWindow()
    {
        isBlockWindowActive = false;
        shouldUpdate = false;

        damageModifier.OnModified -= HandleParry;

        damageReceiver.Modifiers.RemoveModifier(damageModifier);
        knockBackReceiver.Modifiers.RemoveModifier(knockBackModifier);
        poiseDamageReceiver.Modifiers.RemoveModifier(poiseDamageModifier);
    }

    protected override void HandleExit()
    {
        base.HandleExit();

        damageReceiver.Modifiers.RemoveModifier(damageModifier);
        knockBackReceiver.Modifiers.RemoveModifier(knockBackModifier);
        poiseDamageReceiver.Modifiers.RemoveModifier(poiseDamageModifier);
    }

    private bool IsAttackParried(Transform source, out DirectionalInformation directionalInformation)
    {
        float angleOfAttacker = AngleUtilities.AngleFromFacingDirection(
            Core.Root.transform,
            source,
            movement.FacingDirection
        );

        return currentAttackData.IsBlocked(angleOfAttacker, out directionalInformation);
    }

    private void HandleParry(GameObject parriedGameObject)
    {
        /*
         * The modifier is only used to detect an enemy making contact with the player from allowed directions.
         * If that happens we still need to inform the entity that it has been parried.
         */
        if (!CombatParryUtilities.TryParry(parriedGameObject, new ParryData(Core.Root), out _, out _))
        {
            return;
        }

        weapon.Anim.SetTrigger(ParryHash);

        OnParry?.Invoke(parriedGameObject);

        particleManager.StartParticleWithRandomRotation(currentAttackData.Particles, currentAttackData.ParticlesOffset);
    }

    private void HandleEnterAttackPhase(AttackPhases phase)
    {
        shouldUpdate = isBlockWindowActive
            ? currentAttackData.ParryWindowEnd.TryGetTriggerTime(phase, out nextWindowTriggerTime)
            : currentAttackData.ParryWindowStart.TryGetTriggerTime(phase, out nextWindowTriggerTime);
    }

    protected override void Start()
    {
        base.Start();

        damageReceiver = Core.GetCoreComponent<DamageReceiver>();
        knockBackReceiver = Core.GetCoreComponent<KnockbackReceiver>();
        poiseDamageReceiver = Core.GetCoreComponent<PoiseDamageReceiver>();

        movement = Core.GetCoreComponent<Movement>();
        particleManager = Core.GetCoreComponent<ParticleManager>();

        damageModifier = new DamageModifier(IsAttackParried);
        knockBackModifier = new BlockKnockBackModifier(IsAttackParried);
        poiseDamageModifier = new BlockPoiseDamageModifier(IsAttackParried);

        eventHandler.OnEnterAttackPhase += HandleEnterAttackPhase;
    }

    private void Update()
    {
        if (!shouldUpdate || !IsPastTriggerTime())
            return;

        if (isBlockWindowActive)
        {
            StopParryWindow();
        }
        else
        {
            StartParryWindow();
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