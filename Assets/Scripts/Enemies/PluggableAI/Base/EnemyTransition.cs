using UnityEngine;

[System.Serializable]
public class EnemyTransition
{
    public enum Priority
    {
        Default,
        High
    }

    public Priority priority = Priority.Default;
    public EnemyDecision[] decisions;
    public EnemyState trueState; // go here when decision == true
    public EnemyState falseState; // go here when decision == false (set to RemainState to stay)

    public bool GetAllDecisions(EnemyController controller)
    {
        bool result = false;
        foreach(EnemyDecision decision in decisions)
        {
            result = decision.Decide(controller);
        }

        return result;
    }
}
