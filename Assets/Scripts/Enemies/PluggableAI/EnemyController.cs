using Pathfinding;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Config")]
    [field: SerializeField] public EnemyData Data { get; private set; }
    public EnemyState currentState;
    public EnemyState remainState;   // no-op state — keeps enemy in current state

    [Header("State Refs (for TakeDamage routing)")]
    public EnemyState hurtState;
    public EnemyState deathState;

    [Header("Animation Event")]
    [SerializeField] private AnimationEventHandler animationEventHandler;

    [Header("Pathfiding")]
    [field: SerializeField] public Transform[] Waypoints {  get; private set; }
    [field: SerializeField] public AIPath AIPath {  get; private set; }
    [field: SerializeField] public AIDestinationSetter DestinationSetter {  get; private set; }

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
    [ReadOnlyInspector] public bool isAttacking = false;
    [ReadOnlyInspector] public int  numChecks = 0;
    [ReadOnlyInspector] public int  currentAttackIndex = 0;
    [ReadOnlyInspector] public int currentWaypoint = 0;

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
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();
        Core = GetComponentInChildren<Core>();

        AIPath = AIPath == null ? GetComponent<AIPath>() : AIPath;
        DestinationSetter = DestinationSetter == null ? GetComponent<AIDestinationSetter>() : DestinationSetter;

        if (AIPath != null)
        {
            AIPath.maxSpeed = Data.patrolSpeed;
        }

        spawnPoint = transform.position;
        stateEnterTime = Time.time;

        if (Data.isFlying) RB.gravityScale = 0f;

        player = Player.Instance.transform;

        Stats stats = Core.GetComponent<Stats>();
        stats.Health.SetCurrentValue(Data.maxHealth);
        stats.Health.OnCurrentValueZero += HandleDeathState;

        Core.GetComponent<DamageReceiver>().OnTakingDamage += HandleHurtState;
        animationEventHandler.OnFinish += AnimationFinishTrigger;
        animationEventHandler.OnStartAnimationWindow += StartAttackWindow;
        animationEventHandler.OnStopAnimationWindow  += StopAttackWindow;
    }

    private void OnDestroy()
    {
        Stats stats = Core.GetComponent<Stats>();
        stats.Health.OnCurrentValueZero -= HandleDeathState;
        if(Core.TryGetCoreComponent(out DamageReceiver damageReceiver))
            damageReceiver.OnTakingDamage -= HandleHurtState;
        animationEventHandler.OnFinish -= AnimationFinishTrigger;
        animationEventHandler.OnStartAnimationWindow -= StartAttackWindow;
        animationEventHandler.OnStopAnimationWindow  -= StopAttackWindow;
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
        numChecks = 0;
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
        if (currentState == null || isDead) return;
        if (currentState.isAttackState && 
            !isAnimationFinished) return;
        lastSeenPlayerPoint = player.position;
        TransitionToState(hurtState);
    }

    private void StartAttackWindow(AnimationWindows windows)
    {
        if (currentState.isAttackState)
        {
            if (windows == AnimationWindows.Attack)
            {
                lastAttackTime = Time.time;
                isAttacking = true;
            }
        }
    }

    private void StopAttackWindow(AnimationWindows windows)
    {
        if (currentState.isAttackState)
        {
            if (windows == AnimationWindows.Attack)
            {
                isAttacking = false;
            }
        }
    }

    public float TryGetAttackRange()
    {
        float highestAttackRange = 0f;

        if (Data == null) return highestAttackRange;

        foreach (AttackDetails attack in Data.attackDetails)
        {
            if (highestAttackRange < attack.attackRange)
            {
                highestAttackRange = attack.attackRange;
            }
        }
        return highestAttackRange;
    }
}