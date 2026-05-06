using UnityEngine;

public class PlayerTouchingWallState : PlayerState
{
    protected bool isGrounded;
    protected bool isTouchingWall;
    protected bool isTouchingLedge;
    protected bool jumpInput;
    protected int xInput;
    protected int yInput;
    protected bool grabInput;

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = CollisionSenses.Grounded;
        isTouchingWall = CollisionSenses.WallFront;
        isTouchingLedge = CollisionSenses.LedgeHorizontal;

        if(isTouchingWall && !isTouchingLedge) player.ledgeClimbState.SetDetectedPosition(player.transform.position);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;
        jumpInput = player.InputHandler.JumpInput;

        if(jumpInput)
        {
            if (player.IsWallJumpExist) player.wallJumpState.DetermineWallJumpDirection(isTouchingWall);
            stateMachine.ChangeState(player.wallJumpState);
        }
        else if(isGrounded && !grabInput)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if(!isTouchingWall || (xInput != Movement.FacingDirection && !grabInput))
        {
            stateMachine.ChangeState(player.inAirState);
        }
        else if(isTouchingWall && !isTouchingLedge)
        {
            stateMachine.ChangeState(player.ledgeClimbState);
        }
    }
}
