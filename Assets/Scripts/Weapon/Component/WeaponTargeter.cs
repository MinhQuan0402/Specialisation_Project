using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponTargeter : WeaponComponent<WeaponTargeterData, AttackTargeter>
{
    private List<Transform> targets = new();
    private Movement movement;
    private bool isActive;

    protected override void HandleEnter()
    {
        base.HandleEnter();
        isActive = true;
    }
    
    protected override void HandleExit()
    {
        base.HandleExit();
        isActive = false;
    }

    public List<Transform> GetTargets() => targets;
    
    private void CheckForTargets()
    {
        var pos = transform.position + 
                  new Vector3(currentAttackData.Area.center.x * movement.FacingDirection, currentAttackData.Area.center.y);

        var targetColliders =
            Physics2D.OverlapBoxAll(pos, currentAttackData.Area.size, 0f, currentAttackData.DamageableLayers);
        
        targets = targetColliders.Select(item => item.transform).ToList();
    }

    protected override void Start()
    {
        base.Start();
        movement = Core.GetComponent<Movement>();
    }

    private void FixedUpdate()
    {
        if (!isActive) return;
        CheckForTargets();
    }

    private void OnDrawGizmos()
    {
        if(data == null) return;
        
        foreach (var attackTargeter in data.GetAllAttackData())
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)attackTargeter.Area.center, attackTargeter.Area.size);
        }

        Gizmos.color = Color.red;
        foreach (var target in targets)
        {
            if (target == null) continue;

            Gizmos.DrawWireSphere(target.position, 0.25f);
        }
    }
}