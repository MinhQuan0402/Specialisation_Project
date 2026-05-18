using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/NotGroundedDecision")]
public class NotGroundedDecision : EnemyDecision
{
    public override bool Decide(EnemyController controller) =>
        !controller.CollisionSenses.Grounded;
}