using static Combat.Damage.CombatDamageUtilities;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using Combat.Damage;

public class ProjectileDamageComponent : ProjectileComponent
{
    public UnityEvent<IDamageable> OnDamage;
    public UnityEvent<RaycastHit2D> OnRaycastHit;
    
    [field: SerializeField] public LayerMask LayerMask { get; private set; }
    [field: SerializeField] public bool SetInactiveAfterDamage { get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }

    private ProjectileHitboxComponent hitBox;

    private float amount;
    private float critChance;
    private float critMultiplier;
    
    private float lastDamageTime;

    protected override void Init()
    {
        base.Init();
        lastDamageTime = Mathf.NegativeInfinity;
    }

    private void HandleRaycastHit2D(RaycastHit2D[] hits)
    {
        if(!Active) return;
        
        if(Time.time < lastDamageTime + Cooldown) return;


        foreach (var hit in hits)
        {
            if (!LayerMaskUtilities.IsLayerInMask(hit, LayerMask)) continue;

            float finalDamage = amount;
            if (Random.value <= critChance) finalDamage *= critMultiplier;

            if (!TryDamage(hit.collider.gameObject, 
                           new DamageData(finalDamage, projectile.gameObject), 
                           out IDamageable damageable)) continue;
            
            OnDamage?.Invoke(damageable);
            OnRaycastHit?.Invoke(hit);
            lastDamageTime = Time.time;
            
            if(SetInactiveAfterDamage)
                SetActive(false);

            return;
        }
    }
    
    protected override void HandleReceiveDataPackage(ProjectileDataPackage dataPackage)
    {
        base.HandleReceiveDataPackage(dataPackage);

        if (dataPackage is not ProjectileDamageDataPackage package)
            return;

        amount         = package.Amount;
        critChance     = package.CritChance;
        critMultiplier = package.CritMultiplier;
    }

    protected override void Awake()
    {
        base.Awake();
        hitBox = GetComponent<ProjectileHitboxComponent>();
        hitBox.OnRaycastHit2D.AddListener(HandleRaycastHit2D);
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        hitBox.OnRaycastHit2D.RemoveListener(HandleRaycastHit2D);
    }
}