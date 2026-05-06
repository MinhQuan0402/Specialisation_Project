using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Wall Slide")]
public class PlayerWallSlideState : PlayerTouchingWallState
{
    public override void Init()
    {
        base.Init();

        animBoolName = "wallSlide";
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(!isExitingState)
        {
            if (grabInput && yInput == 0 && !isExitingState)
            {
                stateMachine.ChangeState(stateMachine.GetState<PlayerWallGrabState>());
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if(!isExitingState) Movement.SetVelocityY(-playerData.wallSlideVelocity);
    }
}
