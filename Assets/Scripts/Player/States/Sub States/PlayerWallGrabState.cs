using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Wall Grab")]
public class PlayerWallGrabState : PlayerTouchingWallState
{
    public override void Enter()
    {
        base.Enter();

        Movement.RB.simulated = false;
    }

    public override void Exit()
    {
        base.Exit();

        Movement.RB.simulated = true;
        Vector3 playerRotation = player.transform.rotation.eulerAngles;
        playerRotation.z = 0.0f;
        player.transform.rotation = Quaternion.Euler(playerRotation);
    }

    public override void Init()
    {
        base.Init();

        animBoolName = "wallGrab";
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        bool isTouchingTopWall = Physics2D.Raycast(CollisionSenses.LedgeCheckHorizontal.position,
                                                    Vector2.right * Movement.FacingDirection,
                                                    CollisionSenses.WallCheckDistance,
                                                    CollisionSenses.WhatIsWall);
        if (!isExitingState)
        {
            if (yInput > 0f && isTouchingTopWall)
            {
                stateMachine.ChangeState(player.wallClimbState);
            }
            else if (yInput < 0f || !grabInput)
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
        }
    }
}
