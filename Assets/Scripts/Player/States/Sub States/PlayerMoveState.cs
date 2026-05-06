using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Move")]
public class PlayerMoveState : PlayerGroundedState
{
    public override void Init()
    {
        base.Init();
        animBoolName = "move";
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

        Movement.CheckIfShouldFlip(xInput);

        if(xInput == 0f)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Movement.SetVelocityX(playerData.movementVelocity * xInput);
    }
}
