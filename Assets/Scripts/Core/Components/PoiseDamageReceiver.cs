using UnityEngine;
using Combat.PoiseDamage;

public class PoiseDamageReceiver : CoreComponent, IPoiseDamageable
{
    private CoreComp<Stats> stats;

    protected override void Awake()
    {
        base.Awake();
        stats = new CoreComp<Stats>(core);
    }


    public void PoiseDamage(PoiseDamageData data)
    {
        stats.Comp.Poise.Decrease(data.Amount);
    }
}