using UnityEngine;

/// <summary>
/// Base class for all decisions. Returns bool to drive Transitions.
/// </summary>
public abstract class EnemyDecision : ScriptableObject
{
    public abstract bool Decide(EnemyController controller);
}
