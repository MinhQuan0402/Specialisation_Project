using Unity.VisualScripting;
using UnityEngine;

public class Movement : CoreComponent
{
    private CollisionSenses collisionSenses;
    private CollisionSenses CollisionSenses => collisionSenses ? collisionSenses : core.GetCoreComponent(ref collisionSenses);

    public Rigidbody2D RB { get; private set; }

    [field: SerializeField, Range(-1, 1)] 
    public int FacingDirection { get; private set; } = 1;

    public bool CanSetVelocity { get; set; }
    public Vector2 CurrentVelocity { get; private set; } //the velocity attracted from rigidbody
    private Vector2 workspace; //temp velocity variable

    protected override void Awake()
    {
        base.Awake();

        if(FacingDirection == 0 ) FacingDirection = 1;
        RB = GetComponentInParent<Rigidbody2D>(); //Get Rigidbody2D from the main entity
        CanSetVelocity = true;
    }

    public override void LogicUpdate()
    {
        CurrentVelocity = RB.linearVelocity;
    }
    
    public Vector2 FindRelativePoint(Vector2 offset)
    {
        offset.x *= FacingDirection;

        return transform.position + (Vector3)offset;
    }

    #region Set Functions
    public void SetVelocityZero()
    {
        workspace = Vector2.zero;
        SetFinalVelocity();
    }

    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        SetFinalVelocity();
    }

    public void SetVelocity(float velocity, Vector2 direction)
    {
        workspace = direction * velocity;
        SetFinalVelocity();
    }

    public void AddVelocity(float velocity, Vector2 direction)
    {
        RB.AddForce(direction * velocity, ForceMode2D.Impulse);
        CurrentVelocity = RB.linearVelocity;
    }

    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);
        if (CanSetVelocity)
        {
            RB.linearVelocityX = workspace.x;
            CurrentVelocity = workspace;
        }
    }

    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        if (CanSetVelocity)
        {
            RB.linearVelocityY = workspace.y;
            CurrentVelocity = workspace;
        }
    }

    private void SetFinalVelocity()
    {
        if (CanSetVelocity)
        {
            RB.linearVelocity = workspace;
            CurrentVelocity = workspace;
        }
    }

    public Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(CollisionSenses.WallCheck.position, Vector2.right * FacingDirection, CollisionSenses.WallCheckDistance, CollisionSenses.WhatIsWall);
        float xDist = xHit.distance;
        workspace.Set((xDist + 0.5f) * FacingDirection, 0.0f);
        float downDist = CollisionSenses.LedgeCheckHorizontal.position.y - CollisionSenses.WallCheck.position.y + 0.5f;
        RaycastHit2D yHit = Physics2D.Raycast(CollisionSenses.LedgeCheckHorizontal.position + (Vector3)(workspace), Vector2.down, CollisionSenses.LedgeCheckHorizontal.position.y - CollisionSenses.WallCheck.position.y + 0.5f, CollisionSenses.WhatIsWall);
        float yDist = yHit.distance;

        workspace.Set(CollisionSenses.WallCheck.position.x + (xDist * FacingDirection), CollisionSenses.LedgeCheckHorizontal.position.y - yDist);
        return workspace;
    }

    public void CheckIfShouldFlip(int xInput) //When entity allow to flip
    {
        if (xInput != 0 && xInput != FacingDirection) 
        { 
            Flip();
        }
    }

    public void Flip() //To rotate the entity
    { 
        FacingDirection *= -1;
        RB.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    public bool IsInFront(Vector3 targetPosition)
    {
        float deltaX = targetPosition.x - RB.position.x;
        return Mathf.Sign(deltaX) == FacingDirection;
    }

    #endregion
}
