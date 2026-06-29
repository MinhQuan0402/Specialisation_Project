using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Wall Climb")]
public class PlayerWallClimbState : PlayerTouchingWallState
{
    bool isTouchingTopWall = false;

    public override void Init()
    {
        base.Init();

        animBoolName = "wallClimb";
    }

    public override void DoChecks()
    {
        base.DoChecks();


        isTouchingTopWall = Physics2D.Raycast(CollisionSenses.LedgeCheckHorizontal.position,
                                            Vector2.right * Movement.FacingDirection,
                                            CollisionSenses.WallCheckDistance,
                                            CollisionSenses.WhatIsWall);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (yInput != 1 || !isTouchingTopWall)
            {
                stateMachine.ChangeState(player.wallGrabState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if(!isExitingState && isTouchingTopWall)
        {
            Vector2 wallNormal = CollisionSenses.WallNormal;
            Vector2 wallSurface = new(wallNormal.y, -wallNormal.x);
            wallSurface.y = Mathf.Abs(wallSurface.y);
            Vector2 moveDirection = wallSurface.normalized;
            Movement.SetVelocity(playerData.wallClimbVelocity, moveDirection);
        }
    }
}
