using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private EntityData data;
    public EnemyState currentState;
    public State remainState;   // no-op state — keeps enemy in current state

    [Header("State Refs (for TakeDamage routing)")]
    public EnemyState hurtState;
    public EnemyState deathState;

    public EnemyData Data { get; private set; }

    [ReadOnlyInspector] public Transform player;
    [ReadOnlyInspector] public Vector2 spawnPoint;
    [ReadOnlyInspector] public bool isDead;
    [ReadOnlyInspector] public float stateEnterTime;
    [ReadOnlyInspector] public float lastAttackTime = -999f;
    [HideInInspector] public Core Core { get; private set; }
    [HideInInspector] public Rigidbody2D RB;
    [HideInInspector] public Animator Anim;

    void Awake()
    {
        Data = (EnemyData)data;

        Anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();
        Core = GetComponentInChildren<Core>();

        spawnPoint = transform.position;
        stateEnterTime = Time.time;

        if (Data.isFlying) RB.gravityScale = 0f;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        Core.GetComponent<Stats>().Health.OnCurrentValueZero += HandleDeathState;
        Core.GetComponent<DamageReceiver>().OnTakingDamage += HandleHurtState;
    }

    private void OnDestroy()
    {
        Core.GetComponent<Stats>().Health.OnCurrentValueZero -= HandleDeathState;
        Core.GetComponent<DamageReceiver>().OnTakingDamage -= HandleHurtState;
    }

    void Update()
    {
        if (!isDead) currentState.UpdateState(this);
    }

    public void TransitionToState(EnemyState next)
    {
        if (next == null || next == remainState || next == currentState) return;
        currentState = next;
        stateEnterTime = Time.time;
        currentState.EnterState(this);
    }

    private void HandleDeathState()
    {
        TransitionToState(deathState);
        isDead = true;
    }

    private void HandleHurtState(GameObject source)
    {
        if (currentState.isAttackState) return;

        TransitionToState(hurtState);
    }
}