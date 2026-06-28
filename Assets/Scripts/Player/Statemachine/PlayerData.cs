using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : EntityData
{
    [Header("Move State")]
    public float movementVelocity = 10.0f;

    [Header("Jump State")]
    public float jumpVelocity = 15.0f;
    public int maxNumberOfJumps = 1;

    [Header("In Air State")]
    public float maxCoyoteTime = 0.2f;
    public float variableJumpHeightMultiplier = 0.5f;

    [Header("Wall Slide State")]
    public float wallSlideVelocity = 20.0f;

    [Header("Wall Climb State")]
    public float wallClimbVelocity = 2.0f;

    [Header("Wall Jump State")]
    public float wallJumpVelocity = 20f;
    public float wallJumpTime = 0.4f;
    public Vector2 wallJumpJumpAngle = new(1, 2);

    [Header("Ledge Climb State")]
    public float ledgeGrabStamina = 1f;
    public Vector2 startOffset;
    public Vector2 stopOffset;

    [Header("Dash State")]
    public float dashCooldown = 0.5f;
    public float dashTime = 0.2f;
    public float dashVelocity = 30.0f;
    public float dashHoldTimeScale = 0.5f;
    public float dashMaxHoldInputTime = 1.0f;
    public float distBetweenAfterImages = 0.5f;
    public float dashDrag = 0.5f;
    public float dashEndYMultiplier = 0.2f;

    [Header("Attack State")]
    public float attackDrag = 2.0f;
}
