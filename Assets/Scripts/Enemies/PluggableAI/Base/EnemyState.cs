using System.Linq;
using UnityEngine;

/// <summary>
/// A State asset holds composable Actions and Transitions.
/// Right-click in Project → Create → PluggableAI2D → State
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI2D/State")]
public class EnemyState : ScriptableObject
{
    public string animHash;
    public bool isAttackState;

    [Tooltip("All run every Update frame while in this state. Stack multiple for composable behaviour.")]
    public EnemyAction[] actions;

    [Tooltip("Checked top-to-bottom each frame. First decision that fires wins.")]
    public EnemyTransition[] transitions;

    public Color gizmoColor = Color.grey;

    private bool isSorted = false;

    public void EnterState(EnemyController controller)
    {
        // Play animation
        controller.Anim.CrossFade(animHash, 
            controller.Data.animationTransitionTime);

        SortTransition();
    }

    private void SortTransition()
    {
        if (isSorted) return;

        transitions = transitions.OrderBy(t => t.priority).ToArray();
        isSorted = true;
    }

    public void UpdateState(EnemyController controller)
    {
        DoActions(controller);
        CheckTransitions(controller);
    }

    void DoActions(EnemyController controller)
    {
        foreach (var a in actions) a.Act(controller);
    }

    void CheckTransitions(EnemyController controller)
    {
        foreach (var t in transitions)
        {
            bool result = t.decision.Decide(controller);
            controller.TransitionToState(result ? t.trueState : t.falseState);
        }
    }
}
