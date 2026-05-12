using UnityEngine;
using Combat.Damage;

public class DamageReceiver : CoreComponent, IDamageable
{
    [SerializeField] private GameObject damageParticles;

    // Modifiers to apply to incoming damage, such as damage reduction or damage over time effects
    public Modifiers<Modifier<DamageData>, DamageData> Modifiers { get; } = new();

    private CoreComp<Stats> stats;
    private CoreComp<ParticleManager> particleManager;
    
    public void Damage(DamageData data)
    {
        print($"Damage Amount Before Modifiers: {data.Amount}");

        data = Modifiers.ApplyAllModifiers(data);

        print($"Damage Amount After Modifiers: {data.Amount}");

        if(data.Amount <= 0)
            return;

        stats.Comp?.Health.Decrease(data.Amount);
        if(damageParticles != null)
            particleManager.Comp?.StartParticleWithRandomRotation(damageParticles);    
    }

    protected override void Awake()
    {
        base.Awake();

        stats = new CoreComp<Stats>(core);
        particleManager = new CoreComp<ParticleManager>(core);
    }
}