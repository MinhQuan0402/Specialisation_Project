using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private EntityData data;
    public EnemyState currentState;
    public EnemyState remainState;   // no-op state — keeps enemy in current state

    [Header("State Refs (for TakeDamage routing)")]
    public EnemyState hurtState;
    public EnemyState deathState;

    [Space(10)]
    [SerializeField] private AnimationEventHandler animationEventHandler;

    public EnemyData Data { get; private set; }

    [Space(10)]
    [ReadOnlyInspector] public Transform player;
    [ReadOnlyInspector] public Vector2 spawnPoint;
    [ReadOnlyInspector] public Vector2 lastSeenPlayerPoint;
    [ReadOnlyInspector] public float stateEnterTime = 0;
    [ReadOnlyInspector] public float lastCheckTime  = 0;
    [ReadOnlyInspector] public float lastAttackTime = -999f;
    [ReadOnlyInspector] public bool isDead = false;
    [ReadOnlyInspector] public bool isAnimationFinished = false;
    [ReadOnlyInspector] public bool isCheckingDone = false;

    public Core Core { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }


    public void AnimationFinishTrigger()
    {
        isAnimationFinished = true;
        if (isDead) DeathAnimationDone();
    }

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

    private Stats stats = null;
    public Stats Stats
    {
        get => stats != null ? stats : stats = Core.GetComponent<Stats>();
    }

    void Awake()
    {
        Data = (EnemyData)data;

        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();
        Core = GetComponentInChildren<Core>();

        spawnPoint = transform.position;
        stateEnterTime = Time.time;

        if (Data.isFlying) RB.gravityScale = 0f;

        player = Player.Instance.transform;

        Stats stats = Core.GetComponent<Stats>();
        stats.Health.SetCurrentValue(Data.maxHealth);
        stats.Health.OnCurrentValueZero += HandleDeathState;

        Core.GetComponent<DamageReceiver>().OnTakingDamage += HandleHurtState;
        animationEventHandler.OnFinish += AnimationFinishTrigger;
    }

    private void OnDestroy()
    {
        Stats stats = Core.GetComponent<Stats>();
        stats.Health.OnCurrentValueZero -= HandleDeathState;
        if(Core.TryGetCoreComponent(out DamageReceiver damageReceiver))
            damageReceiver.OnTakingDamage -= HandleHurtState;
        animationEventHandler.OnFinish -= AnimationFinishTrigger;
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
        lastSeenPlayerPoint = player.position;
        TransitionToState(hurtState);
    }
}