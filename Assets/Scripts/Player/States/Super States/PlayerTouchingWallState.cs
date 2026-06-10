using Unity.VisualScripting;
using UnityEngine;

public class PlayerTouchingWallState : PlayerState
{
    protected bool isGrounded;
    protected bool isTouchingWall;
    protected bool isTouchingTopLedge;
    protected bool isTouchingBotLedge;
    protected bool jumpInput;
    protected int xInput;
    protected int yInput;
    protected bool grabInput;

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = CollisionSenses.Grounded;
        isTouchingWall = CollisionSenses.WallFront;
        isTouchingTopLedge = CollisionSenses.LedgeHorizontalTop;
        isTouchingBotLedge = CollisionSenses.LedgeHorizontalBot;

        RaycastHit2D hit = Physics2D.Raycast(CollisionSenses.WallCheck.position,
                                            Vector2.right * Movement.FacingDirection, 
                                            CollisionSenses.WallCheckDistance, 
                                            CollisionSenses.WhatIsWall);
        player.transform.SetParent(hit.transform);

        if (isTouchingBotLedge && !isTouchingTopLedge) player.ledgeClimbState.SetDetectedPosition(player.transform.position);
    }

    public override void Exit()
    {
        base.Exit();
        player.transform.SetParent(null);
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;
        jumpInput = player.InputHandler.JumpInput;

        if(jumpInput && player.IsWallJumpExist && player.wallJumpState.CanPerform)
        {
            if (player.IsWallJumpExist) player.wallJumpState.DetermineWallJumpDirection(isTouchingWall);
            stateMachine.ChangeState(player.wallJumpState);
        }
        else if(isGrounded && !grabInput)
        {
            stateMachine.ChangeState(player.idleState); 
        }
        else if(!isTouchingWall || (xInput != Movement.FacingDirection && !grabInput) || !isTouchingTopLedge)
        {
            stateMachine.ChangeState(player.inAirState);
        }
        else if(isTouchingBotLedge && !isTouchingTopLedge &&
            player.IsLedgeClimbExist && player.ledgeClimbState.PositionProjection())
        {
            stateMachine.ChangeState(player.ledgeClimbState);
        }
    }
}
