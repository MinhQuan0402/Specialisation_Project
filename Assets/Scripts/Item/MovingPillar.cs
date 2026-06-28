using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class MovingPillar : MonoBehaviour
{
    private enum MovingDirection
    {
        Up, Down, Left, Right
    }


    [Header("Movement")]
    [Tooltip("How many units the pillar moves downward.")]
    [SerializeField] float dropDistance = 4f;
    [SerializeField] MovingDirection movingDirection = MovingDirection.Up;

    [Tooltip("Movement duration in seconds.")]
    [SerializeField] float moveDuration = 0.8f;

    [Tooltip("Easing curve — leave as EaseInOut for a natural feel.")]
    [SerializeField] AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Behaviour")]
    [Tooltip("Seconds to wait after lever pull before the pillar starts moving.")]
    [SerializeField] float delayBeforeMove = 0.2f;

    [Tooltip("If true the pillar rises back up after raiseDuration seconds.")]
    [SerializeField] bool autoRaise = false;
    [SerializeField] float autoRaiseDelay = 3f;

    [SerializeField] bool useCameraShake = false;
    [SerializeField] float shakeAmplitude = 11.0f;
    [SerializeField] float shakeFrequency = 5.5f;

    [SerializeField] GameObject cinematicCamera;

    [Header("Events")]
    [SerializeField] UnityEvent OnTriggerEnter;
    [SerializeField] UnityEvent OnMoveEnter;
    [SerializeField] UnityEvent OnMoveFinished;
    [SerializeField] UnityEvent OnReverseFinshed;

    // ── Runtime ───────────────────────────────────────────────────────────────
    private Vector3 _originalPosition;      // original position = raised state
    private Vector3 _movingPosition;    // original Y minus dropDistance
    private bool _isMoving = false;
    private bool _isInFinalPos = false;
    private Coroutine _moveRoutine;

    void Awake()
    {
        Vector3 movingDirection = GetDirection;
        _originalPosition = transform.position;
        _movingPosition   = _originalPosition + movingDirection * dropDistance;
    }

    Vector3 GetDirection => movingDirection == MovingDirection.Up ? Vector3.up :
                            movingDirection == MovingDirection.Down ? Vector3.down :
                            movingDirection == MovingDirection.Left ? Vector3.left : Vector3.right;

    public void GoFinal()
    {
        if (_isMoving || _isInFinalPos) return;

        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(Move(_originalPosition, _movingPosition, moveFinalPos: true));
    }

    public void Return()
    {
        if (_isMoving || !_isInFinalPos) return;

        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(Move(_movingPosition, _originalPosition, moveFinalPos: false));
    }

    /// <summary>Instantly snap the pillar up or down without animation.</summary>
    public void SnapDown() { transform.position = _movingPosition; _isInFinalPos = true; }
    public void SnapUp() { transform.position = _originalPosition; _isInFinalPos = false; }

    // ─────────────────────────────────────────────────────────────────────────
    //  MOVEMENT COROUTINE
    // ─────────────────────────────────────────────────────────────────────────

    IEnumerator Move(Vector3 from, Vector3 to, bool moveFinalPos)
    {
        _isMoving = true;
        OnTriggerEnter?.Invoke();
        if (cinematicCamera != null) cinematicCamera.SetActive(true);
        Player.Instance.Paused();
        UIManager.Instance.ActivateCinematicBar(delayBeforeMove);

        if (useCameraShake) 
            CameraShakeCinemachine.Shake(moveDuration, shakeAmplitude, shakeFrequency, delayBeforeMove);

        // Optional delay before the pillar starts moving
        if (delayBeforeMove > 0f)
            yield return new WaitForSeconds(delayBeforeMove);

        OnMoveEnter?.Invoke();

        // Start movement sound
        /*if (audioSource != null && moveSound != null)
            audioSource.PlayOneShot(moveSound);*/

        // Smooth movement over moveDuration seconds
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(Mathf.Clamp01(elapsed / moveDuration));
            transform.position = Vector3.LerpUnclamped(from, to, t);
            yield return null;
        }

        // Snap exactly to target in case of floating-point drift
        transform.position = to;

        // Landing sound
        /*if (audioSource != null && landSound != null)
            audioSource.PlayOneShot(landSound);*/

        _isMoving = false;
        _isInFinalPos = moveFinalPos;
        if (cinematicCamera != null) cinematicCamera.SetActive(false);
        Player.Instance.Unpaused(delayBeforeMove);
        UIManager.Instance.DeactivateCinematicBar(delayBeforeMove);

        // Fire completion events
        if (moveFinalPos)
        {
            OnMoveFinished?.Invoke();

            if (autoRaise)
            {
                yield return new WaitForSeconds(autoRaiseDelay);
                Return();
            }
        }
        else
        {
            OnReverseFinshed?.Invoke();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  GIZMO — preview up/down positions in scene view
    // ─────────────────────────────────────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        Vector3 orignalPosition = Application.isPlaying ? _originalPosition : transform.position;
        Vector3 finalPosition   = Application.isPlaying ? _movingPosition : transform.position + GetDirection * dropDistance;

        Collider2D collider = GetComponent<Collider2D>();

        // Orignal position — green outline
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(orignalPosition, collider.bounds.size);

        // Final position — red outline
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(finalPosition, collider.bounds.size);

        // Travel line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(orignalPosition, finalPosition);
    }
}
