using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Wall Climb")]
public class PlayerWallClimbState : PlayerTouchingWallState
{
    public override void Init()
    {
        base.Init();

        animBoolName = "wallClimb";
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(!isExitingState)
        {
            if (yInput != 1)
            {
                stateMachine.ChangeState(player.wallGrabState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if(!isExitingState)
            Movement.SetVelocityY(playerData.wallClimbVelocity);
    }
}
