using UnityEngine.Events;
using UnityEngine;
using Utilities;
using Combat.Knockback;

public class ProjectileKnockBackComponent : ProjectileComponent
{
    public UnityEvent OnKnockBack;

    [field: SerializeField] public LayerMask LayerMask { get; private set; }

    private ProjectileHitboxComponent hitBox;

    private int direction;
    private float strength;
    private Vector2 angle;
    
    private void HandleRaycastHit2D(RaycastHit2D[] hits)
    {
        if (!Active)
            return;

        direction = (int)Mathf.Sign(transform.right.x);
            
        foreach (var hit in hits)
        {
            // Is the object under consideration part of the LayerMask that we can damage?
            if (!LayerMaskUtilities.IsLayerInMask(hit, LayerMask))
                continue;

            // NOTE: We need to use .collider.transform instead of just .transform to get the GameObject the collider we detected is attached to, otherwise it returns the parent
            if (!hit.collider.transform.gameObject.TryGetComponent(out IKnockBackable knockBackable))
                continue;

            knockBackable.KnockBack(new KnockBackData(angle, strength, direction, projectile.gameObject));

            OnKnockBack?.Invoke();
                
            return;
        }
    }
    
    protected override void HandleReceiveDataPackage(ProjectileDataPackage dataPackage)
    {
        base.HandleReceiveDataPackage(dataPackage);

        if (dataPackage is not ProjectileKnockBackDataPackage knockBackDataPackage)
            return;

        strength = knockBackDataPackage.Strength;
        angle = knockBackDataPackage.Angle;
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