using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/FlipOnExitLedge")]
public class FlipOnExitLedgeAction : EnemyAction
{
    public override void Act(EnemyController controller)
    {
        if (!controller.CollisionSenses.LedgeVertical)
        {
            controller.Movement.Flip();
        }
    }
}
