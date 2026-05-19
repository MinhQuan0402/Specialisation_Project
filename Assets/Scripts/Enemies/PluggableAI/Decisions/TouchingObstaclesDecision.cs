using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/TouchingObstacles")]
public class TouchingObstaclesDecision : EnemyDecision
{
    public override bool MakeDecision(EnemyController controller) =>
        controller.CollisionSenses.WallFront;
}