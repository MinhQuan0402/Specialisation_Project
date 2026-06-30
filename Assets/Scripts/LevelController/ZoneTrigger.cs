using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[RequireComponent(typeof(BoxCollider2D))]
public class ZoneTrigger: MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private StepData step;
    [SerializeField] private TimelineSequence timelineSequence;
    [SerializeField] private bool triggerOnEnter = true;
    [SerializeField] private bool triggerOnExit = false;
    [SerializeField] private bool oneShot = true;
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private EnemyController bossController;

    [SerializeField] private UnityEvent onZoneEntered;
    [SerializeField] private UnityEvent onZoneExited;

    public Vector2 SpawnPoint { get; private set; } = Vector2.zero;

    private bool triggered = false;

    public StepData Step => step;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnEnter) return;
        if (!other.CompareTag(requiredTag)) return;
        if (oneShot && triggered) return;

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.down, 100.0f, LayerMask.GetMask("Ground"));
        if (hit2D.collider != null) SpawnPoint = new(hit2D.point.x, hit2D.point.y + Player.Instance.Collider.size.y * 0.5f);
        else SpawnPoint = new(transform.position.x, transform.position.y);

        triggered = true;
        ZoneEventBus.TriggerZoneEntered(step, other.gameObject);
        onZoneEntered?.Invoke();
        if (timelineSequence != null) CutsceneManager.Instance.Play(timelineSequence);
        if (bossController   != null) GameManager.Instance.StartBossState(bossController);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!triggerOnExit) return;
        if (!other.CompareTag(requiredTag)) return;
        ZoneEventBus.TriggerZoneExited(step, other.gameObject);
        onZoneExited?.Invoke();
        if (oneShot) gameObject.SetActive(false);
    }

    public void ResetTrigger()
    {
        triggered = false;
        gameObject.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        if (step == null) return;

        Gizmos.color = triggered
            ? new Color(0.5f, 0.5f, 0.5f, 0.25f)
            : new Color(1f, 0.85f, 0f, 0.25f);

        var col = GetComponent<BoxCollider2D>();
        if (col)
        {
            Gizmos.DrawCube(transform.position + (Vector3)col.offset, col.size);
            Gizmos.color = new Color(1f, 0.85f, 0f, 0.9f);
            Gizmos.DrawWireCube(transform.position + (Vector3)col.offset, col.size);
        }

        if (step.hasRespawnPoint)
        {
            Gizmos.color = Color.red;
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.down, 100.0f, LayerMask.GetMask("Ground"));
            if (hit2D.collider != null)
            {
                Gizmos.DrawLine(transform.position, hit2D.point);
            }
            else
            {
                Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 100.0f);
            }
        }

#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.6f,
            $"[{step.displayName}]");
#endif
    }
}