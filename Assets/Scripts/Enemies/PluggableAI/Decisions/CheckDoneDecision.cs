using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/CheckDone")]
public class CheckDoneDecision : EnemyDecision
{
    public override bool MakeDecision(EnemyController controller) =>
        controller.isCheckingDone;
}
