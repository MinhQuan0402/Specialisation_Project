using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Wall Grab")]
public class PlayerWallGrabState : PlayerTouchingWallState
{
    Vector2 newPosition;

    public override void Enter()
    {
        base.Enter();

        newPosition = Movement.RB.position;
    }

    public override void Init()
    {
        base.Init();

        animBoolName = "wallGrab";
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(!isExitingState)
        {
            if (yInput > 0f)
            {
                stateMachine.ChangeState(player.wallClimbState);
            }
            else if (yInput < 0f || !grabInput)
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!isExitingState) HoldPosition();
    }

    private void HoldPosition()
    {
        Movement.SetVelocityX(0f);
        Movement.SetVelocityY(0f);
        Movement.RB.position = newPosition;
    }
}
