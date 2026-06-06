using UnityEngine;
using System.Collections;

public class TutorialStepTrigger: MonoBehaviour
{
    [SerializeField] private TutorialStep stepToActivate;

    public Vector2 SpawnPoint {  get; private set; } = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.down, 100.0f, LayerMask.GetMask("Ground"));
        if (hit2D.collider != null) SpawnPoint = new(hit2D.point.x, hit2D.point.y + Player.Instance.Collider.size.y * 0.5f);
        else SpawnPoint = new(transform.position.x, transform.position.y);

        TutorialSceneController.Instance.AdvanceToStep(stepToActivate);

        // Disable so it only fires once
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
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
}