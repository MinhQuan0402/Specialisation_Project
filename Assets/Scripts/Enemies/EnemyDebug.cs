using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyDebug : MonoBehaviour
{
    [SerializeField] private EnemyController m_EnemyController;
    [SerializeField] private EnemyData data;
    [SerializeField] private Movement movement;

    [SerializeField] private int segments = 30;

    [SerializeField] private bool showGizmos = true;

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos || m_EnemyController == null) return;
        if(data.isFlying)
        {
            DrawVisionCone(data.playerDetectionRange, Color.yellow);
            DrawVisionCone(m_EnemyController.TryGetAttackRange(), Color.red);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, 
                            transform.position + data.playerDetectionRange * 
                            movement.FacingDirection * Vector3.right);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, 
                            transform.position + m_EnemyController.TryGetAttackRange() * 
                            movement.FacingDirection * Vector3.right);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(m_EnemyController.lastSeenPlayerPoint, 0.5f);
    }

    private void DrawVisionCone(float playerDetectionRange, Color color)
    {
        Gizmos.color = color;

        Vector3 origin = transform.position;

        // Draw left and right boundaries
        Vector3 leftDir = GetDirection(-data.playerDetectionAngle / 2) * 
                          movement.FacingDirection;
        Vector3 rightDir = GetDirection(data.playerDetectionAngle / 2) * 
                           movement.FacingDirection;

        Gizmos.DrawLine(origin, origin + leftDir * playerDetectionRange);
        Gizmos.DrawLine(origin, origin + rightDir * playerDetectionRange);

        // Draw arc
        Vector3 previousPoint = origin + leftDir * playerDetectionRange;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -data.playerDetectionAngle / 2 + 
                         (data.playerDetectionAngle * i / segments);

            Vector3 nextPoint =
                origin + playerDetectionRange * 
                movement.FacingDirection * GetDirection(angle);

            Gizmos.DrawLine(previousPoint, nextPoint);

            previousPoint = nextPoint;
        }
    }

    Vector3 GetDirection(float angle)
    {
        float finalAngle =
            transform.eulerAngles.z + angle;

        float rad = finalAngle * Mathf.Deg2Rad;

        return new Vector3(
            Mathf.Cos(rad),
            Mathf.Sin(rad),
            0f
        );
    }
}
