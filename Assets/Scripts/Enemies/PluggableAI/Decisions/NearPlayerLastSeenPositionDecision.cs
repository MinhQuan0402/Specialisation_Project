using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/NearPlayerLastSeenPosition")]
public class NearPlayerLastSeenPositionDecision : EnemyDecision
{
    [SerializeField] private float distanceToLastSeenPosition;

    public override bool MakeDecision(EnemyController controller) =>
        Vector3.Distance(controller.transform.position, controller.lastSeenPlayerPoint) <= distanceToLastSeenPosition;
}
