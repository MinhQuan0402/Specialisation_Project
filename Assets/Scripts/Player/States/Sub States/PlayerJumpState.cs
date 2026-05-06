using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Jump")]
public class PlayerJumpState : PlayerAbilityState
{
    private int numberOfJumpLeft;

    public override void Init()
    {
        base.Init();

        animBoolName = "inAir";
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.InputHandler.UseJumpInput();
        Movement.SetVelocityY(playerData.jumpVelocity);
        isAbilityDone = true;
        numberOfJumpLeft--;
        if (player.IsInAirExist) player.inAirState.SetIsJumping();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public bool CanJump()
    {
        if(numberOfJumpLeft > 0) return true;
        else return false;
    }

    public void ResetNumberOfJumpsLeft() => numberOfJumpLeft = playerData.maxNumberOfJumps;
    public void DecreaseNumberOfJumpsLeft() => numberOfJumpLeft--;

}
