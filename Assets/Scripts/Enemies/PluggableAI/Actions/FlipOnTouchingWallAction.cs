using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "PluggableAI/Actions/FlipOnTouchingWall")]
public class FlipOnTouchingWallAction : EnemyAction
{
    public override void Act(EnemyController controller)
    {
        if (controller.CollisionSenses.WallFront)
        {
            controller.Movement.Flip();
        }
    }
}