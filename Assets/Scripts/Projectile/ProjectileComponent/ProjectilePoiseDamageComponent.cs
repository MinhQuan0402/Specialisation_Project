using Combat.PoiseDamage;
using UnityEngine;
using Utilities;

public class ProjectilePoiseDamageComponent : ProjectileComponent
{
    public UnityEngine.Events.UnityEvent OnPoiseDamage;

    [field: SerializeField] public LayerMask LayerMask { get; private set;}

    private float amount;
    
    private ProjectileHitboxComponent hitBox;

    private void HandleRaycastHit2D(RaycastHit2D[] hits)
    {
        if (!Active) return;

        foreach (var hit in hits)
        {
            if(!LayerMaskUtilities.IsLayerInMask(hit, LayerMask)) continue;
            
            if(!hit.collider.transform.TryGetComponent(out IPoiseDamageable poiseDamageable)) continue;
            
            poiseDamageable.PoiseDamage(new PoiseDamageData(amount, projectile.gameObject));
        }
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