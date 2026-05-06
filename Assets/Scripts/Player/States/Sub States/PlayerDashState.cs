using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Dash")]
public class PlayerDashState : PlayerAbilityState
{
    private float lastDashTime;

    public override void Init()
    {
        base.Init();

        animBoolName = "dash";

        lastDashTime = Time.time;
    }

    public override void Enter()
    {
        base.Enter();

        player.InputHandler.UseDashInput();
    }

    public override void Exit()
    {
        base.Exit();

        if (Movement.CurrentVelocity.y > 0f)  Movement.SetVelocityY(Movement.CurrentVelocity.y * playerData.dashVelocity);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!isExitingState)
        {
            Movement.SetVelocity(playerData.dashVelocity, Vector2.right * Movement.FacingDirection);

            if((Time.time >= startTime + playerData.dashTime) || !isGrounded)
            {
                player.RB.linearDamping = 0.0f;
                isAbilityDone = true;
                lastDashTime = Time.time;
            }
        }
    }

    public bool CheckIfCanDash()
    {
        return Time.time > lastDashTime + playerData.dashCooldown;
    }
}
