using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private EntityData data;
    public EnemyState currentState;
    public EnemyState remainState;   // no-op state — keeps enemy in current state

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

    [ReadOnlyInspector] public bool isAnimationFinished = false;
    public void AnimationFinishTrigger() => isAnimationFinished = true;

    public void DeathAnimationDone()
    {
        if(Core.TryGetCoreComponent(out Death death))
        {
            death.isAnimationFinished = true;
            death.Die();
        }
    }

    private Movement movement = null;
    public Movement Movement
    {
        get => movement != null ? movement : movement = Core.GetComponent<Movement>();
    }

    private CollisionSenses collisionSenses = null;
    public CollisionSenses CollisionSenses
    {
        get => collisionSenses != null ? collisionSenses : collisionSenses = Core.GetComponent<CollisionSenses>();
    }

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


        Stats stats = Core.GetComponent<Stats>();
        stats.Health.SetCurrentValue(Data.maxHealth);
        stats.Health.OnCurrentValueZero += HandleDeathState;

        Core.GetComponent<DamageReceiver>().OnTakingDamage += HandleHurtState;
    }

    private void OnDestroy()
    {
        Stats stats = Core.GetComponent<Stats>();
        stats.Health.OnCurrentValueZero -= HandleDeathState;
        if(Core.TryGetCoreComponent(out DamageReceiver damageReceiver))
            damageReceiver.OnTakingDamage -= HandleHurtState;
    }

    void Update()
    {
        Core.LogicUpdate();
        if (!isDead && currentState != null) 
            currentState.UpdateState(this);
    }

    public void TransitionToState(EnemyState next)
    {
        if (next == null || next == remainState || next == currentState) return;
        currentState = next;
        stateEnterTime = Time.time;
        currentState.EnterState(this);
        isAnimationFinished = false;
    }

    private void HandleDeathState()
    {
        Debug.Log($"{gameObject.name} has died.");
        TransitionToState(deathState);
        isDead = true;

        if(Core.TryGetCoreComponent(out DamageReceiver damageReceiver))
        {
            Destroy(damageReceiver);
        }

        if (Core.TryGetCoreComponent(out KnockbackReceiver knockbackReceiver    ))
        {
            Destroy(knockbackReceiver);
        }

        if(Core.TryGetCoreComponent(out PoiseDamageReceiver poiseDamageReceiver))
        {
            Destroy(poiseDamageReceiver);
        }
    }

    private void HandleHurtState(GameObject source)
    {
        if (isDead) return;
        if (currentState.isAttackState && 
            !isAnimationFinished) return;
        Debug.Log($"{gameObject.name} has been hurt by {source.name}.");
        TransitionToState(hurtState);
    }
}