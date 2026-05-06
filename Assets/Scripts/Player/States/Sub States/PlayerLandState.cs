using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Land")]
public class PlayerLandState : PlayerGroundedState
{
    public override void Init()
    {
        base.Init();

        animBoolName = "land";
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(!isExitingState)
        {
            if (xInput != 0)
            {
                stateMachine.ChangeState(player.moveState);
            }
            else if (isAnimationFinished)
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
