using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Returns true after the enemy has been in the current state for [duration] seconds.
/// Useful for IdleState → PatrolState after a wait.
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decisions/RandomTimer")]
public class RandomTimerDecision : EnemyDecision
{

    // Keyed by (controller, stateEnterTime) so the roll resets on every state entry
    private readonly Dictionary<(EnemyController, float), float> _durations = new();

    public override bool Decide(EnemyController controller)
    {
        var key = (controller, controller.stateEnterTime);

        if (!_durations.TryGetValue(key, out float duration))
        {
            duration = Random.Range(controller.Data.minIdleTime, controller.Data.maxIdleTime);
            _durations[key] = duration;
            PruneOldKeys(controller); // keep dict from growing unbounded
        }

        return (Time.time - controller.stateEnterTime) >= duration;
    }

    // Remove stale entries for this controller (old state entries)
    void PruneOldKeys(EnemyController c)
    {
        var toRemove = new List<(EnemyController, float)>();
        foreach (var key in _durations.Keys)
            if (key.Item1 == c && key.Item2 != c.stateEnterTime)
                toRemove.Add(key);
        foreach (var key in toRemove)
            _durations.Remove(key);
    }
}