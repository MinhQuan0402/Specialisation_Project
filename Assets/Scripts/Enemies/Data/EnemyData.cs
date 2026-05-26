using Combat.Knockback;
using System;
using UnityEngine;

[Serializable]
public class AttackDetails
{
    public string attackName;
    public float  damageAmount = 10f;
    public float  attackRange = 1.5f;
    public float  attackCooldown = 0.5f;
    public float  knockbackStrength = 5f;
    public Vector2  knockbackAngle = Vector2.zero;
}

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
    public float patrolSpeed = 10.0f;
    public float minPatrolTime = 3.0f;
    public float maxPatrolTime = 5.0f;

    [Header("Player Detected State")]
    public float playerDetectionRange = 9.0f;
    [Tooltip("This is only for Flying Enemy")]
    public float playerDetectionAngle = 90.0f;
    public float maxDetectionHeight = 3.0f;
    public float chaseSpeed = 5.0f;

    [Header("Attacking State")]
    public float closeRange = 1.0f;
    public AttackDetails[] attackDetails = new AttackDetails[1];

    [Header("Look For Player State")]
    public float maxLookingTime = 3.0f;

    [Header("Detection Masks")]
    public LayerMask whatAreDetectibles;
    public LayerMask whatIsObstacles;
}
