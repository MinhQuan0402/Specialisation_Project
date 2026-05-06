using UnityEngine;
using Combat.Damage;

public class DamageReceiver : CoreComponent, IDamageable
{
    [SerializeField] private GameObject damageParticles;

    private CoreComp<Stats> stats;
    private CoreComp<ParticleManager> particleManager;
    
    public void Damage(DamageData data)
    {
        Debug.Log($"{core.transform.parent.gameObject.name} is hit with damage of {data.Amount}!");
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