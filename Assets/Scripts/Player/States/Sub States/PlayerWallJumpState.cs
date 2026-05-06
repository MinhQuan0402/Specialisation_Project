using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Wall Jump")]
public class PlayerWallJumpState : PlayerAbilityState
{
    private int wallJumpDirection;

    public override void Init()
    {
        base.Init();

        animBoolName = "inAir";
    }

    public override void Enter()
    {
        base.Enter();

        player.InputHandler.UseJumpInput();
        if (player.IsJumpExist) player.jumpState.ResetNumberOfJumpsLeft();
        Movement.SetVelocity(playerData.wallJumpVelocity, playerData.wallJumpJumpAngle, wallJumpDirection);
        Movement.CheckIfShouldFlip(wallJumpDirection);
        if(player.IsJumpExist) player.jumpState.DecreaseNumberOfJumpsLeft();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.Anim.SetFloat("yVelocity", Movement.CurrentVelocity.y);
        player.Anim.SetFloat("xVelocity", Mathf.Abs(Movement.CurrentVelocity.x));

        if(Time.time >= startTime + playerData.wallJumpTime)
        {
            isAbilityDone = true;
        }
    }

    public void DetermineWallJumpDirection(bool isTouchingWall)
    {
        if(isTouchingWall)
        {
            wallJumpDirection = -Movement.FacingDirection;
        }
        else
        {
            wallJumpDirection = Movement.FacingDirection;
        }
    }
}
