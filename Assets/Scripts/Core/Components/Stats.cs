using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Stats : CoreComponent 
{
    [field: SerializeField] public Stat Health { get; private set; }
    [field: SerializeField] public Stat Poise { get; private set; }
    [field: SerializeField] public Stat Stamina { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Health.Init();
        Stamina.Init();

        Health.OnValueDecrease += HandleValueDecrease;
        Poise.OnValueDecrease += HandleValueDecrease;
        Stamina.OnValueDecrease += HandleValueDecrease;
    }

    protected void OnDestroy()
    {
        Health.OnValueDecrease -= HandleValueDecrease;
        Poise.OnValueDecrease -= HandleValueDecrease;
        Stamina.OnValueDecrease -= HandleValueDecrease;
    }

    private void HandleValueDecrease(Stat stat)
    {
        if (stat.RecoveryCoroutine != null) 
            StopCoroutine(stat.RecoveryCoroutine);
        stat.RecoveryCoroutine = StartCoroutine(Recovery(stat));
    }

    IEnumerator Recovery(Stat stat)
    {
        yield return new WaitForSeconds(stat.RecoveryDelay);
        while (stat.RecoveryEnabled && stat.CurrentValue < stat.MaxValue)
        {
            stat.Increase(stat.RecoveryRate * Time.deltaTime);
            yield return null;
        }
    }
}
