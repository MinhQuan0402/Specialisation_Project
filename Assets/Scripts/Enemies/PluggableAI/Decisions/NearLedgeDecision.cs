using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/NearLedgeDecision")]
public class NearLedgeDecision : EnemyDecision
{
    public override bool MakeDecision(EnemyController controller) =>
        !controller.CollisionSenses.LedgeVertical;
}