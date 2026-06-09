using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Dash")]
public class PlayerDashState : PlayerAbilityState
{
    public bool CanDash { get; private set; }
    private bool isHolding;
    private bool dashInputStop;

    private float lastDashTime;

    private Vector2 dashDirection;
    private Vector2 dashDirectionInput;
    private Vector2 lastAIPos;

    public override void Init()
    {
        base.Init();
        animBoolName = "inAir";
        lastDashTime = Time.time;
    }

    public override void Enter()
    {
        base.Enter();

        CanDash = false;
        player.InputHandler.UseDashInput();

        isHolding = true;
        dashDirection = Vector2.right * Movement.FacingDirection;

        Time.timeScale = playerData.dashHoldTimeScale;
        startTime = Time.unscaledTime;

        player.DashIndicator.gameObject.SetActive(true);
        player.Anim.SetFloat("xVelocity", dashDirection.x);
    }

    public override void Exit()
    {
        base.Exit();

        if (Movement.CurrentVelocity.y > 0f)  
            Movement.SetVelocityY(Movement.CurrentVelocity.y * playerData.dashEndYMultiplier);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            player.Anim.SetFloat("yVelocity", Movement.CurrentVelocity.y);
            player.Anim.SetFloat("xVelocity", Mathf.Abs(Movement.CurrentVelocity.x));

            if (isHolding)
            {
                dashDirectionInput = player.InputHandler.RawMovementInput;
                dashInputStop = player.InputHandler.DashInputStop;

                if (dashDirectionInput != Vector2.zero)
                {
                    dashDirection = dashDirectionInput;
                    dashDirection.Normalize();
                }

                float angle = Vector2.SignedAngle(Vector2.right, dashDirection);
                player.DashIndicator.rotation = Quaternion.Euler(0f, 0f, angle - 45f);

                if (dashInputStop || Time.unscaledTime >= startTime + playerData.dashMaxHoldInputTime)
                {
                    isHolding = false;
                    Time.timeScale = 1f;
                    startTime = Time.time;
                    Movement.CheckIfShouldFlip(Mathf.RoundToInt(dashDirection.x));
                    player.RB.linearDamping = playerData.dashDrag;
                    Movement.SetVelocity(playerData.dashVelocity, dashDirection);
                    player.DashIndicator.gameObject.SetActive(false);
                    PlaceAfterImage();
                }
            }
            else
            {
                Movement.SetVelocity(playerData.dashVelocity, dashDirection);
                CheckIfShouldPlaceAfterImage();

                if (Time.time >= startTime + playerData.dashTime)
                {
                    player.RB.linearDamping = 0f;
                    isAbilityDone = true;
                    lastDashTime = Time.time;
                }
            }
        }
    }

    public bool CheckIfCanDash()
    {
        return Time.time > lastDashTime + playerData.dashCooldown && CanDash;
    }

    private void PlaceAfterImage()
    {
        AfterImagePool.GetInstance(player.gameObject.GetEntityId()).GetFromPool();
        lastAIPos = player.transform.position;
    }

    private void CheckIfShouldPlaceAfterImage()
    {
        if (Vector2.Distance(player.transform.position, lastAIPos) >= 
            playerData.distBetweenAfterImages)
        {
            PlaceAfterImage();
        }
    }

    public void ResetCanDash() => CanDash = true;
}
