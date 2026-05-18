using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/TouchingObstacles")]
public class TouchingObstaclesDecision : EnemyDecision
{
    public override bool Decide(EnemyController controller) =>
        controller.CollisionSenses.WallFront;
}