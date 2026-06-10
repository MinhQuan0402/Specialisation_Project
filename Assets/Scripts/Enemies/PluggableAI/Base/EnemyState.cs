using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A State asset holds composable Actions and Transitions.
/// Right-click in Project → Create → PluggableAI → State
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/State/Normal State")]
public class EnemyState : ScriptableObject
{
    public string animHash;

    [Tooltip("All run once. Stack multiple for composable behaviour.")]
    public EnemyAction[] startActions;

    [Tooltip("All run every Update frame while in this state. Stack multiple for composable behaviour.")]
    public EnemyAction[] updateActions;

    [Tooltip("Checked top-to-bottom each frame. First decision that fires wins.")]
    public EnemyTransition[] transitions;

    [field: SerializeField, ReadOnlyInspector] public bool IsTransibleToAttack { get; private set; }
    public EnemyTransition AttackTransition { get; private set; }

    public void EnterState(EnemyController controller)
    {
        // Play animation
        controller.Anim.CrossFade(animHash,
            controller.Data.animationTransitionTime);
        foreach (EnemyAction action in startActions) action.Act(controller);
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
            // TransitionToState will set the current state to the new state, so if we are no longer in this state, break out of the loop
            if (controller.CurrentState != this) break;
        }
    }

    public void GenerateAttackTransition()
    {
#if UNITY_EDITOR
        EnemyTransition enemyTransition = new()
        {
            priority = EnemyTransition.Priority.High,
            decisions = new EnemyDecision[1],
            falseState = AssetDatabase.LoadAssetAtPath<EnemyState>("Assets/ScriptableObjects/Enemies/Enemy State/States/Remain State.asset")
        };

        string[] searchFolders = new string[] { "Assets/ScriptableObjects/Enemies/Enemy State/Decisions" };
        string[] guids = AssetDatabase.FindAssets("t:TryGetAttackDecision", searchFolders);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EnemyDecision asset = AssetDatabase.LoadAssetAtPath<EnemyDecision>(path);
            enemyTransition.decisions[0] = asset;
        }

        transitions = transitions.Append(enemyTransition).ToArray();
        IsTransibleToAttack = true;
        AttackTransition = enemyTransition;
#endif
    }

    public void RemoveAttackTransition()
    {
        IsTransibleToAttack = false;
        transitions = transitions.Where(val  => val != AttackTransition).ToArray();
        AttackTransition = null;
    }

    public void SortTransition()
    {
        static int getPriority(EnemyTransition t) => (int)t.priority;
        transitions = transitions.OrderByDescending(getPriority).ToArray();
    }

    public bool IsAttackState => typeof(EnemyAttackState) == GetType();
}
