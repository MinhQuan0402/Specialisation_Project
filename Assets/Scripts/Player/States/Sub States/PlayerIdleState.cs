using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Idle")]
public class PlayerIdleState : PlayerGroundedState
{
    public override void Init()
    {
        base.Init();

        animBoolName = "idle";
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        Movement.SetVelocityZero();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(xInput != 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
