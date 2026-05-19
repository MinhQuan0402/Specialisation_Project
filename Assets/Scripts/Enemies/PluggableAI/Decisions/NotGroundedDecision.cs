using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/NotGroundedDecision")]
public class NotGroundedDecision : EnemyDecision
{
    public override bool MakeDecision(EnemyController controller) =>
        !controller.CollisionSenses.Grounded;
}