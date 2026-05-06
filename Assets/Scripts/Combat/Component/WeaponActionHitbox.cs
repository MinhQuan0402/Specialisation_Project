using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class WeaponActionHitbox : WeaponComponent<WeaponActionHitboxData, AttackActionHitbox>
{
    public event Action<Collider2D[]> OnDetectedColliders2D;
    private CoreComp<Movement> movementCore;

    private Vector2 offset;
    private Collider2D[] detectedColliders;

    private void HandleAttackAction()
    {
        movementCore ??= new CoreComp<Movement>(Core);

        offset.Set(
            transform.position.x + (currentAttackData.Hitbox.center.x * movementCore.Comp.FacingDirection),
            transform.position.y + currentAttackData.Hitbox.center.y
        );

        detectedColliders = Physics2D.OverlapBoxAll(offset, currentAttackData.Hitbox.size, 0f, data.DetectableLayers);
        detectedColliders = detectedColliders
            .Where(c => c != null && c.transform != transform && c.transform.root != transform.root)
            .ToArray();

        
        if (detectedColliders.Length == 0)
            return;
        
        OnDetectedColliders2D?.Invoke(detectedColliders);
    }

    protected override void Start()
    {
        base.Start();
        
        eventHandler.OnAttackAction += HandleAttackAction;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        eventHandler.OnAttackAction -= HandleAttackAction;
    }

    private void OnDrawGizmos()
    {
        if(data == null) return;

        foreach(var d in data.GetAllAttackData())
        {
            if(!d.Debug) continue;
            Gizmos.DrawWireCube(transform.position + new Vector3(d.Hitbox.center.x * movementCore.Comp.FacingDirection, d.Hitbox.center.y), d.Hitbox.size);
        }
    }
}
