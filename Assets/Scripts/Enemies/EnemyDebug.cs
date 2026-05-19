using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyDebug : MonoBehaviour
{
    [SerializeField] private EnemyController m_EnemyController;
    [SerializeField] private EnemyData data;
    [SerializeField] private Movement movement;

    [SerializeField] private bool showGizmos = true;

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos || m_EnemyController == null) return;
        if(data.isFlying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.playerDetectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.attackRange);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * movement.FacingDirection * data.playerDetectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * movement.FacingDirection * data.attackRange);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(m_EnemyController.lastSeenPlayerPoint, 0.5f);
    }
}
