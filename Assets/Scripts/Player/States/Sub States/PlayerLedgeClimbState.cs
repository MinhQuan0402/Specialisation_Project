using Unity.Android.Gradle.Manifest;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Ledge Climb")]
public class PlayerLedgeClimbState : PlayerState
{
    private bool jumpInput;

    private Vector2 detectedPos;
    private Vector2 cornerPos;
    private Vector2 startPos;
    private Vector2 stopPos;

    private bool isHanging;
    private bool isClimbing;

    private int yInput;

    private bool prediction;

    public override void Init()
    {
        base.Init();

        animBoolName = "ledgeClimb";
    }

    public override void Enter()
    {
        base.Enter();

        Movement.SetVelocityZero();
        player.transform.position = startPos;
    }

    public override void Exit()
    {
        base.Exit();

        if (prediction) return;

        isHanging = false;

        if(isClimbing)
        {
            player.transform.position = stopPos;
            isClimbing = false;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        jumpInput = player.InputHandler.JumpInput;

        if(isAnimationFinished)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else
        {
            Movement.SetVelocityZero();
            yInput = player.InputHandler.NormInputY;
            player.Stats.Comp.Stamina.Decrease(playerData.ledgeGrabStamina * Time.deltaTime);

            player.transform.position = startPos;  

            if (yInput == 1 && isHanging && !isClimbing)
            {
                isClimbing = true;
                player.Anim.SetBool("climbLedge", true);
            }
            else if ((yInput == -1 && isHanging && !isClimbing) || player.IsInteruptible ||
                      player.Stats.Comp.Stamina.CurrentValue < playerData.ledgeGrabStamina)
            {
                stateMachine.ChangeState(player.inAirState);
            }
            else if(jumpInput && !isClimbing)
            {
                if(player.IsWallJumpExist) player.wallJumpState.DetermineWallJumpDirection(true);
                stateMachine.ChangeState(player.wallJumpState);
            }
        }
    }
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        player.Anim.SetBool("climbLedge", false);
    }
    public void SetDetectedPosition(Vector2 detectedPos) => this.detectedPos = detectedPos;
    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        isHanging = true;
    }

    public bool PositionProjection()
    {
        cornerPos = Movement.DetermineCornerPosition();
        startPos.Set(cornerPos.x - (Movement.FacingDirection * playerData.startOffset.x), cornerPos.y - playerData.startOffset.y);
        stopPos.Set(cornerPos.x + (Movement.FacingDirection * playerData.stopOffset.x), cornerPos.y + playerData.stopOffset.y);
        DebugUtils.DrawBox2D(stopPos, new Vector2(0.5f, 0.5f), Color.blue, 5.0f);
        prediction = Physics2D.BoxCast(stopPos, new Vector2(0.5f, 0.5f), 0, Vector2.zero, 0, CollisionSenses.WhatIsGround);
        if (prediction) return false;

        return true;
    }
}
