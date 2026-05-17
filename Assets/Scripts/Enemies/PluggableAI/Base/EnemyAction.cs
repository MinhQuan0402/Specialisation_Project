using UnityEngine;

/// <summary>
/// Base class for all enemy actions (verbs: Patrol, Chase, Shoot, etc).
/// Create a ScriptableObject subclass for each behaviour.
/// </summary>
public abstract class EnemyAction :ScriptableObject
{
    public abstract void Act(EnemyController controller);
}
