using UnityEngine;
using System.Collections;

/// <summary>
/// Returns true after the enemy has been in the current state for [duration] seconds.
/// Useful for IdleState → PatrolState after a wait.
/// Right-click → Create → PluggableAI2D → Decisions → Timer
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI2D/Decisions/Timer")]
public class TimerDecision : EnemyDecision
{
    public float duration = 1.5f;

    public override bool Decide(EnemyController controller) =>
        (Time.time - controller.stateEnterTime) >= duration;
}