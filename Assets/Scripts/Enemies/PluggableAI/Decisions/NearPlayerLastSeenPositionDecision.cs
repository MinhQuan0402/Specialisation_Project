using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/NearPlayerLastSeenPosition")]
public class NearPlayerLastSeenPositionDecision : EnemyDecision
{
    public override bool MakeDecision(EnemyController controller) =>
        Vector3.Distance(controller.transform.position, 
            new Vector2(controller.lastSeenPlayerPoint.x, 
                controller.transform.position.y)) <= controller.Data.closeRange;
}
