using UnityEngine;
using Utilities;

public class WeaponCharge : WeaponComponent<WeaponChargeData, AttackCharge>
{
    private int currentCharge;
    
    private TimeNotifier timeNotifier;
    
    private ParticleManager particleManager;

    public int TakeFinalChargeReading()
    {
        timeNotifier.Disable();
        return currentCharge;
    }

    protected override void HandleEnter()
    {
        base.HandleEnter();
        currentCharge = currentAttackData.InitialChargeAmount;
        timeNotifier.Init(currentAttackData.ChargeTime, true);
    }
    
    private void HandleNotify()
    {
        currentCharge++;

        if (currentCharge >= currentAttackData.NumOfCharges)
        {
            currentCharge = currentAttackData.NumOfCharges;
            timeNotifier.Disable();
            
            if(!currentAttackData.FullyChargedIndicatorParticlePrefab.Equals(null))
                particleManager.StartParticlesRelative(currentAttackData.FullyChargedIndicatorParticlePrefab,
                    currentAttackData.ParticlesOffset, Quaternion.identity);
        }
        else
        {
            if(!currentAttackData.ChargeIncreaseIndicatorParticlePrefab.Equals(null))
                particleManager.StartParticlesRelative(currentAttackData.ChargeIncreaseIndicatorParticlePrefab,
                    currentAttackData.ParticlesOffset, Quaternion.identity);
        }
    }

    protected override void HandleExit()
    {
        base.HandleExit();
        timeNotifier.Disable();
    }

    protected override void Awake()
    {
        base.Awake();
        timeNotifier = new TimeNotifier();
        timeNotifier.OnNotify += HandleNotify;
    }

    protected override void Start()
    {
        base.Start();
        particleManager = Core.GetComponent<ParticleManager>();
    }

    private void Update()
    {
        timeNotifier.Tick();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        timeNotifier.OnNotify -= HandleNotify;
    }
}