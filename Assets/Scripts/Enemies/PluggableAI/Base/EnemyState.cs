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

    [Tooltip("All run once. Stack multiple for composable behaviour.")]
    public EnemyAction[] startActions;

    [Tooltip("All run every Update frame while in this state. Stack multiple for composable behaviour.")]
    public EnemyAction[] updateActions;

    [Tooltip("Checked top-to-bottom each frame. First decision that fires wins.")]
    public EnemyTransition[] transitions;

    public Color gizmoColor = Color.grey;

    private bool isSorted = false;

    public void EnterState(EnemyController controller)
    {
        // Play animation
        controller.Anim.CrossFade(animHash, 
            controller.Data.animationTransitionTime);
        foreach(EnemyAction action in startActions) action.Act(controller);
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
        foreach (var a in updateActions) a.Act(controller);
    }

    void CheckTransitions(EnemyController controller)
    {
        foreach (var t in transitions)
        {
            bool result = t.GetAllDecisions(controller);
            controller.TransitionToState(result ? t.trueState : t.falseState);
        }
    }
}
