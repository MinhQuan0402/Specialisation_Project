using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    public float amountStaminaRequired = 20.0f;

    public bool CanPerform => player.Stats.Comp.Stamina.CurrentValue >= amountStaminaRequired;

    protected bool isAbilityDone;
    protected bool isGrounded;

    public override void Init()
    {
        base.Init();
    }

    public override void Enter()
    {
        base.Enter();

        UseStamina();
        isAbilityDone = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(isAbilityDone)
        {
            if(isGrounded && Movement.CurrentVelocity.y < 0.01f)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.inAirState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = CollisionSenses.Grounded;
    }

    public virtual void UseStamina()
    {
        player.Stats.Comp.Stamina.Decrease(amountStaminaRequired);
    }
}
