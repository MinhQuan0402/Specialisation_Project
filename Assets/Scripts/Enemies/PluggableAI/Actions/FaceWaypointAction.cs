using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "PluggableAI/Actions/FaceWaypoint")]
public class FaceWaypointAction : EnemyAction
{
    public override void Act(EnemyController controller)
    {
        int newFacingDirection = Vector2.Dot(controller.AIPath.desiredVelocity.normalized, Vector2.right) > 0 ? 1 : -1;
        if(controller.Movement.FacingDirection != newFacingDirection )
        {
            controller.Movement.Flip();
        }
    }
}