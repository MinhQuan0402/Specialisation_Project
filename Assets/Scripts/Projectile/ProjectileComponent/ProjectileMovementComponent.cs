using UnityEngine;

public class ProjectileMovementComponent : ProjectileComponent
{
    [field: SerializeField] public bool ApplyContinuousMovement { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    
    private void SetVelocity() => rb.linearVelocity = Speed * transform.right;

    protected override void Init()
    {
        base.Init();
        SetVelocity();
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(!ApplyContinuousMovement || rb.bodyType == RigidbodyType2D.Static) return;
        SetVelocity();
    }
}