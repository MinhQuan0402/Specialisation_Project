using UnityEngine;

/// <summary>
/// Base class for all decisions. Returns bool to drive Transitions.
/// </summary>
public abstract class EnemyDecision : ScriptableObject
{
    [SerializeField] private bool invertDecision = false;

    public abstract bool MakeDecision(EnemyController controller);

    public bool Decide(EnemyController controller)
    {
        bool decision = MakeDecision(controller);
        return invertDecision ? !decision : decision;
    }
}
