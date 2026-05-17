using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data/Base Data")]
public class EnemyData : EntityData
{
    [Header("Configuration")]
    public bool isFlying = false;
    public float animationTransitionTime = 0.1f;

    [Header("Idle State")]
    public float minIdleTime = 3.0f;
    public float maxIdleTime = 5.0f;

    [Header("Move State")]
    public float maxMoveVelocity = 10.0f;
    public float maxMoveTime = 5.0f;

    [Header("Player Detected State")]
    public float playerDetectionRange = 9.0f;
    public float maxDetectionHeight = 3.0f;

    [Header("Attacking State")]
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    public float default_attackCooldown = 0.5f;
    public float maxCombo_attackCooldown = 1.0f;
    public float defaultDamage = 10f;
    public float maxComboDamage = 25f;

    [Header("Look For Player State")]
    public float maxLookingTime = 3.0f;
}
