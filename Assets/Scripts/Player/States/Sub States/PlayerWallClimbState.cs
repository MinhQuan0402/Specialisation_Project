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
            Movement.SetVelocityY(playerData.wallClimbVelocity);
    }
}
