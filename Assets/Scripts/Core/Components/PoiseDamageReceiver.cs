using UnityEngine;
using Combat.PoiseDamage;

public class PoiseDamageReceiver : CoreComponent, IPoiseDamageable
{
    private CoreComp<Stats> stats;

    public Modifiers<Modifier<PoiseDamageData>, PoiseDamageData> Modifiers { get; } = new();

    protected override void Awake()
    {
        base.Awake();
        stats = new CoreComp<Stats>(core);
    }


    public void PoiseDamage(PoiseDamageData data)
    {
        data = Modifiers.ApplyAllModifiers(data);

        stats?.Comp?.Poise?.Decrease(data.Amount);
    }
}