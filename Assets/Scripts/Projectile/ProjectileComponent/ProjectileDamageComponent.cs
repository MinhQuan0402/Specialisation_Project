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
            if(!LayerMaskUtilities.IsLayerInMask(hit, LayerMask)) continue;
            
            if(!hit.collider.TryGetComponent(out DamageReceiver damageable)) continue;
            
            damageable.Damage(new DamageData(amount, projectile.gameObject));
            
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

        amount = package.Amount;
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