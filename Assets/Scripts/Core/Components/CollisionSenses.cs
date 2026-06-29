using UnityEngine;

public class CollisionSenses : CoreComponent
{
    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);

    private Movement movement;

    #region Check Trasform
    public Transform GroundCheck 
    { 
        get => GenericNotImplementError<Transform>.TryGet(groundCheck, transform.parent.name); 
        private set => groundCheck = value; 
    }

    public Transform WallCheck
    {
        get => GenericNotImplementError<Transform>.TryGet(wallCheck, transform.parent.name);
        private set => wallCheck = value;
    }

    public Transform LedgeCheckHorizontal
    {
        get => GenericNotImplementError<Transform>.TryGet(ledgeCheckHorizontal, transform.parent.name);
        private set => ledgeCheckHorizontal = value;
    }

    public Transform LedgeCheckVertical
    {
        get => GenericNotImplementError<Transform>.TryGet(ledgeCheckVertical, transform.parent.name);
        private set => ledgeCheckVertical = value;
    }

    public LayerMask WhatIsGround
    {
        get => GenericNotImplementError<LayerMask>.TryGet(whatIsGround, transform.parent.name);
        private set => whatIsGround = value;
    }

    public LayerMask WhatIsWall
    {
        get => GenericNotImplementError<LayerMask>.TryGet(whatIsWall, transform.parent.name);
        private set => whatIsWall = value;
    }

    public float GroundCheckRaidus
    {
        get => groundCheckRadius;
        private set => groundCheckRadius = value;
    }

    public float WallCheckDistance
    {
        get => wallCheckDistance;
        private set => wallCheckDistance = value;
    }
    #endregion

    [Header("Ground Check Details")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 1.0f;
    [SerializeField] LayerMask whatIsGround;

    [Header("Wall Check Details")]
    [SerializeField] Transform wallCheck;
    [SerializeField] float wallCheckDistance = 1.0f;
    [SerializeField] LayerMask whatIsWall;

    [Header("Ledge Check Details")]
    [SerializeField] Transform ledgeCheckHorizontal;
    [SerializeField] Transform ledgeCheckVertical;
    [SerializeField] float ledgeCheckDistance = 1.0f;
    [SerializeField] LayerMask whatIsLedge;

    public override void LogicUpdate() { }

    public Vector2 GroundNormal
    {
        get
        {
            Vector2 groundNormal = Vector2.up;
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, whatIsGround);
            if (hit.collider != null) groundNormal = hit.normal;
            return groundNormal;
        }
    }

    public Vector2 WallNormal
    {
        get
        {
            Vector2 wallNormal = new (-Movement.FacingDirection, 0.0f);
            RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsWall);
            if (hit.collider != null) wallNormal = hit.normal;
            return wallNormal;
        }
    }

    public bool Grounded => Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround); 
    public bool WallFront => Physics2D.Raycast(wallCheck.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsWall);
    public bool WallBack => Physics2D.Raycast(wallCheck.position, Vector2.right * -Movement.FacingDirection, wallCheckDistance, whatIsWall);
    public bool LedgeHorizontalTop => Physics2D.Raycast(ledgeCheckHorizontal.position, Vector2.right * Movement.FacingDirection, ledgeCheckDistance, whatIsLedge);
    public bool LedgeHorizontalBot => Physics2D.Raycast(wallCheck.position, Vector2.right * Movement.FacingDirection, ledgeCheckDistance, whatIsLedge);
    public bool LedgeVertical => Physics2D.Raycast(ledgeCheckVertical.position, Vector2.down, ledgeCheckDistance, whatIsLedge);

    private void OnDrawGizmos()
    {
        if(groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        int facingDirection = (core) ? Movement.FacingDirection : 1;

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheck.position, (Vector2)wallCheck.position + facingDirection * wallCheckDistance * Vector2.right);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheck.position, (Vector2)wallCheck.position + -facingDirection * wallCheckDistance * Vector2.right);
        }

        if(ledgeCheckHorizontal != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(ledgeCheckHorizontal.position, (Vector2)ledgeCheckHorizontal.position + facingDirection * ledgeCheckDistance * Vector2.right);
        }

        if(ledgeCheckVertical != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(ledgeCheckVertical.position, (Vector2)ledgeCheckVertical.position + ledgeCheckDistance * Vector2.down);
        }
    }
}
