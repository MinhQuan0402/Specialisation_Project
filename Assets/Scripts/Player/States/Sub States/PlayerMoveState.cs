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

        Vector2 groundNormal = CollisionSenses.GroundNormal;
        Vector2 surfaceDirection = new (groundNormal.y, -groundNormal.x);
        Vector2 moveDirection = surfaceDirection.normalized * xInput;
        Movement.SetVelocity(playerData.movementVelocity, moveDirection);
    }
}
