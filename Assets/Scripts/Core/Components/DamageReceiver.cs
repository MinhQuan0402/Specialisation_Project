using UnityEngine;
using Combat.Damage;
using System;

public class DamageReceiver : CoreComponent, IDamageable
{
    [SerializeField] private GameObject damageParticles;

    public event Action<GameObject> OnTakingDamage;

    // Modifiers to apply to incoming damage, such as damage reduction or damage over time effects
    public Modifiers<Modifier<DamageData>, DamageData> Modifiers { get; } = new();

    private CoreComp<Stats> stats;
    private CoreComp<ParticleManager> particleManager;
    
    public void Damage(DamageData data)
    {
        data = Modifiers.ApplyAllModifiers(data);

        print($"Damage Amount: {data.Amount}");

        if(data.Amount <= 0)
            return;

        stats.Comp?.Health.Decrease(data.Amount);
        if(damageParticles != null)
            particleManager.Comp?.StartParticleWithRandomRotation(damageParticles);

        OnTakingDamage?.Invoke(data.Source);
    }

    protected override void Awake()
    {
        base.Awake();

        stats = new CoreComp<Stats>(core);
        particleManager = new CoreComp<ParticleManager>(core);
    }
}