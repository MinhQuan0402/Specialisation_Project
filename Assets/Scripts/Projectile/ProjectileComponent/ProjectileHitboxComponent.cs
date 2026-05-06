using UnityEngine;
using UnityEngine.Events;

public class ProjectileHitboxComponent : ProjectileComponent
{
    public UnityEvent<RaycastHit2D[]> OnRaycastHit2D;
    
    [field: SerializeField] public Rect HitboxRect { get; private set; }
    [field: SerializeField] public LayerMask LayerMask { get; private set; }
    
    private RaycastHit2D[] hits;
    private float checkDistance;
    
    private Transform _transform;

    private void CheckHitBox()
    {
        hits = Physics2D.BoxCastAll(_transform.TransformPoint(HitboxRect.center), HitboxRect.size,
            _transform.rotation.eulerAngles.z, _transform.right, checkDistance, LayerMask);
        if(hits.Length <= 0) return;
        OnRaycastHit2D?.Invoke(hits);
    }
    
    protected override void Awake()
    {
        base.Awake();
        _transform = transform;;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        checkDistance = rb.linearVelocity.magnitude * Time.deltaTime;
        CheckHitBox();
    }
    
    private void OnDrawGizmosSelected()
    {
        // The following is some code that ChatGPT Generated for me to visualize the HitBoxRect based on the rotation.
        // Set up gizmo color
        Gizmos.color = Color.red;

        // Create a new matrix that applies the projectile's rotation
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position,
            Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z), Vector3.one);
        Gizmos.matrix = rotationMatrix;

        // Draw the wireframe cube
        Gizmos.DrawWireCube(HitboxRect.center, HitboxRect.size);
    }
}