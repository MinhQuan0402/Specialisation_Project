using UnityEngine;
using Utilities;

public class ProjectileDelayedGravityComponent : ProjectileComponent
{
    [field: SerializeField] public float Distance { get; private set; }

    private DistanceNotifier distanceNotifier = new DistanceNotifier();
    
    private float gravity;

    [HideInInspector] public float distanceMultiplier = 1f;

    private void HandleNotify()
    {
        rb.gravityScale = gravity;
    }
    
    protected override void Init()
    {
        base.Init();
        rb.gravityScale = 0f;
        distanceNotifier.Init(transform.position, Distance * distanceMultiplier);
        distanceMultiplier = 1;
    }
    
    protected override void Awake()
    {
        base.Awake();

        gravity = rb.gravityScale;
            
        distanceNotifier.OnNotify += HandleNotify;
    }

    protected override void Update()
    {
        base.Update();
            
        distanceNotifier?.Tick(transform.position);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
            
        distanceNotifier.OnNotify -= HandleNotify;
    }
}