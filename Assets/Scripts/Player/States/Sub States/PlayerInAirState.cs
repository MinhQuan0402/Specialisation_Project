using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/InAir")]
public class PlayerInAirState : PlayerState
{
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isTouchingWallBack;
    private bool isTouchingTopLedge;
    private bool isTouchingBotLedge;
    private bool oldIsTouchingWall;
    private bool oldIsTouchingWallBack;
    private int xInput;
    private bool jumpInput;
    private bool grabInput;
    private bool dashInput;
    private bool jumpInputStop;
    private bool jumpCoyoteTime;
    private bool wallJumpCoyoteTime;
    private bool isJumping;

    private float startWallJumpCoyoteTime;

    public override void Init()
    {
        base.Init();

        animBoolName = "inAir";
    }

    public override void DoChecks()
    {
        base.DoChecks();

        oldIsTouchingWall = isTouchingWall;
        oldIsTouchingWallBack = isTouchingWallBack;

        isGrounded = CollisionSenses.Grounded;
        isTouchingWall = CollisionSenses.WallFront;
        isTouchingWallBack = CollisionSenses.WallBack;
        isTouchingTopLedge = CollisionSenses.LedgeHorizontalTop;
        isTouchingBotLedge = CollisionSenses.LedgeHorizontalBot;

        if(isTouchingBotLedge && !isTouchingTopLedge) 
        { 
            if(player.IsLedgeClimbExist) 
                player.ledgeClimbState.SetDetectedPosition(player.transform.position); 
        }

        if(!wallJumpCoyoteTime && !isTouchingWall && !isTouchingWallBack && (oldIsTouchingWall || oldIsTouchingWallBack))
        {
            StartWallJumpCoyoteTime();
        }
    }

    public override void Enter()
    {
        base.Enter();

        player.RB.linearDamping = 0.0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckJumpCoyoteTime();
        CheckWallJumpCoyoteTime();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputStop = player.InputHandler.JumpInputStop;
        grabInput = player.InputHandler.GrabInput;
        dashInput = player.InputHandler.DashInput;

        bool primaryAttackInput = player.InputHandler.AttackInputs[(int)CombatInputs.primary];
        bool secondaryAttackInput = player.InputHandler.AttackInputs[(int)CombatInputs.secondary];

        CheckJumpMultiplier();

        if (primaryAttackInput && player.IsPrimaryAttackExist &&
            !Player.Instance.IsInteruptible && player.primaryAttackState.CanPerform &&
            player.primaryAttackState.CanAttack())
        {
            stateMachine.ChangeState(player.primaryAttackState);
        }
        else if (secondaryAttackInput && player.IsSecondaryAttackExist &&
            !Player.Instance.IsInteruptible && player.secondaryAttackState.CanPerform &&
            player.secondaryAttackState.CanAttack())
        {
            stateMachine.ChangeState(player.secondaryAttackState);
        }
        else if (dashInput && player.IsDashExist && 
            player.dashState.CheckIfCanDash() && 
            player.dashState.CanDash)
        {
            stateMachine.ChangeState(player.dashState);
        }
        else if (isGrounded && Movement.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.landState);
        }
        else if (isTouchingBotLedge && !isTouchingTopLedge &&
            player.IsLedgeClimbExist && player.ledgeClimbState.PositionProjection())
        {
            stateMachine.ChangeState(player.ledgeClimbState);
        }
        else if (jumpInput && (isTouchingWall || isTouchingWallBack || wallJumpCoyoteTime) && 
            player.wallJumpState.CanPerform)
        {
            StoptWallJumpCoyoteTime();
            isTouchingWall = CollisionSenses.WallFront;
            if (player.IsWallJumpExist) player.wallJumpState.DetermineWallJumpDirection(isTouchingWall);
            stateMachine.ChangeState(player.wallJumpState);
        }
        else if(jumpInput && player.IsJumpExist && player.jumpState.CanJump() && player.jumpState.CanPerform)
        {
            stateMachine.ChangeState(player.jumpState);
        }
        else if(isTouchingWall)
        {
            if(grabInput && isTouchingTopLedge)
            {
                stateMachine.ChangeState(player.wallGrabState);
            }
            else if (xInput == Movement.FacingDirection && Movement.CurrentVelocity.y <= 0f)
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
        }
        else
        {
            Movement.CheckIfShouldFlip(xInput);
            Movement.SetVelocityX(playerData.movementVelocity * xInput);

            anim.SetFloat("yVelocity", Movement.CurrentVelocity.y);
            anim.SetFloat("xVelocity", Mathf.Abs(Movement.CurrentVelocity.x));
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void CheckJumpMultiplier()
    {
        if (isJumping)
        {
            if (jumpInputStop)
            {
                Movement.SetVelocityY(Movement.CurrentVelocity.y * playerData.variableJumpHeightMultiplier);
                isJumping = false;
            }
            else if (Movement.CurrentVelocity.y <= 0.0f)
            {
                isJumping = false;
            }
        }
    }

    private void CheckJumpCoyoteTime()
    {
        if(jumpCoyoteTime && Time.time > startTime + playerData.maxCoyoteTime)
        {
            jumpCoyoteTime = false;
            player.jumpState.DecreaseNumberOfJumpsLeft();
        }
    }

    private void CheckWallJumpCoyoteTime()
    {
        if(wallJumpCoyoteTime && Time.time > startWallJumpCoyoteTime + playerData.maxCoyoteTime)
        {
            wallJumpCoyoteTime = false;
        }
    }

    public void StartCoyoteTime() => jumpCoyoteTime = true;

    public void StartWallJumpCoyoteTime() => wallJumpCoyoteTime = true;
    public void StoptWallJumpCoyoteTime()
    {
        wallJumpCoyoteTime = false;
        startWallJumpCoyoteTime = Time.time;
    }

    public void SetIsJumping() => isJumping = true;
}
