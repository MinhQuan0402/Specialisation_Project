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
    private bool isTouchingLedge;

    public override void DoChecks()
    {
        base.DoChecks();

        isGroundded = CollisionSenses.Grounded;
        isTouchingWall = CollisionSenses.WallFront;
        isTouchingLedge = CollisionSenses.LedgeHorizontal;
    }

    public override void Enter()
    {
        base.Enter();

        player.RB.linearDamping = 10.0f;
        if (stateMachine.GetState<PlayerJumpState>() != null) { stateMachine.GetState<PlayerJumpState>().ResetNumberOfJumpsLeft(); }
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

        if (primaryAttackInput && player.IsPrimaryAttackExist && !Player.Instance.IsInteruptible)
            stateMachine.ChangeState(player.primaryAttackState);
        else if(secondaryAttackInput && player.IsSecondaryAttackExist && !Player.Instance.IsInteruptible)
            stateMachine.ChangeState(player.secondaryAttackState);
        else if(jumpInput && player.IsJumpExist && player.jumpState.CanJump())
            stateMachine.ChangeState(player.jumpState);
        else if(!isGroundded)
        {
            if (player.IsInAirExist) { player.inAirState.StartCoyoteTime(); }
            stateMachine.ChangeState(player.inAirState);
        }
        else if(isTouchingWall && grabInput && isTouchingLedge)
        {
            stateMachine.ChangeState(player.wallGrabState);
        }
        else if(dashInput && player.IsDashExist && player.dashState.CheckIfCanDash())
        {
            stateMachine.ChangeState(player.dashState);
        }
    }
}
