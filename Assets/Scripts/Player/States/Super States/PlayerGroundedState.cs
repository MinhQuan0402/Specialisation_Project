using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected int xInput;
    private bool grabInput;
    private bool jumpInput;
    private bool dashInput;
    private bool primaryAttackInput;
    private bool secondaryAttackInput;

    private bool isGroundded;
    private bool isTouchingWall;
    private bool isTouchingTopLedge;
    private bool isTouchingBotLedge;

    public override void DoChecks()
    {
        base.DoChecks();

        isGroundded = CollisionSenses.Grounded;
        isTouchingWall = CollisionSenses.WallFront;
        isTouchingTopLedge = CollisionSenses.LedgeHorizontalTop;
        isTouchingBotLedge = CollisionSenses.LedgeHorizontalBot;
    }

    public override void Enter()
    {
        base.Enter();

        player.RB.linearDamping = 10.0f;
        if (stateMachine.GetState<PlayerJumpState>() != null) { stateMachine.GetState<PlayerJumpState>().ResetNumberOfJumpsLeft(); }
        if (stateMachine.GetState<PlayerDashState>() != null) { stateMachine.GetState<PlayerDashState>().ResetCanDash(); }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        grabInput = player.InputHandler.GrabInput;
        dashInput = player.InputHandler.DashInput;

        primaryAttackInput = player.InputHandler.AttackInputs[(int)CombatInputs.primary];
        secondaryAttackInput = player.InputHandler.AttackInputs[(int)CombatInputs.secondary];

        if (primaryAttackInput && player.IsPrimaryAttackExist &&
            !Player.Instance.IsInteruptible && player.primaryAttackState.CanPerform &&
            player.primaryAttackState.CanTransitionToAttackState())
        {
            stateMachine.ChangeState(player.primaryAttackState);
        }
        else if (secondaryAttackInput && player.IsSecondaryAttackExist &&
            !Player.Instance.IsInteruptible && player.secondaryAttackState.CanPerform &&
            player.secondaryAttackState.CanTransitionToAttackState())
        {
            stateMachine.ChangeState(player.secondaryAttackState);
        }
        else if (jumpInput && player.IsJumpExist && 
            player.jumpState.CanJump() && 
            player.jumpState.CanPerform)
        {
            stateMachine.ChangeState(player.jumpState);
        }
        else if (!isGroundded)
        {
            if (player.IsInAirExist) { player.inAirState.StartCoyoteTime(); }
            stateMachine.ChangeState(player.inAirState);
        }
        else if (isTouchingWall && grabInput && isTouchingTopLedge)
        {
            stateMachine.ChangeState(player.wallGrabState);
        }
        else if (dashInput && player.IsDashExist && 
            player.dashState.CheckIfCanDash() && 
            player.dashState.CanPerform)
        {
            stateMachine.ChangeState(player.dashState);
        }
    }
}
