using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Config")]
    [field: SerializeField] public EnemyData Data { get; private set; }
    [SerializeField] private EnemyState startingState;
    [SerializeField] private EnemyState remainState;   // no-op state — keeps enemy in current state
    [ReadOnlyInspector] public EnemyAttackState currentAttackState;
    [field: SerializeField, ReadOnlyInspector] public EnemyState CurrentState { get; private set; }

    [Header("State Refs (for TakeDamage routing)")]
    public EnemyState hurtState;
    public EnemyState deathState;

    [Header("Animation Event")]
    [SerializeField] private AnimationEventHandler animationEventHandler;

    [Header("Pathfiding")]
    [field: SerializeField] public Transform[] Waypoints {  get; private set; }
    [field: SerializeField] public AIPath AIPath {  get; private set; }
    [field: SerializeField] public AIDestinationSetter DestinationSetter {  get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject enemyUIPrefab;

    public GameObject EnemyUI { get; private set; } = null;
    public bool IsBoss = false;

    [Space(10), Header("Debug Info")]
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
    [ReadOnlyInspector] public float attackRequestTime = 0;

    public Dictionary<EnemyAttackState, float> attacksCDList = new();

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
            AIPath.endReachedDistance = Data.closeRange;
        }

        spawnPoint = transform.position;
        stateEnterTime = Time.time;

        if (Data.isFlying) RB.gravityScale = 0f;

        player = Player.Instance.transform;

        Stats stats = Core.GetComponent<Stats>();
        stats.Health.Init(Data.maxHealth);
        stats.Health.OnCurrentValueZero += HandleDeathState;

        Core.GetComponent<DamageReceiver>().OnTakingDamage += HandleHurtState;
        animationEventHandler.OnFinish += AnimationFinishTrigger;
        animationEventHandler.OnStartAnimationWindow += StartAttackWindow;
        animationEventHandler.OnStopAnimationWindow  += StopAttackWindow;

        isCheckingDone = true;

        if(EnemyUI == null && enemyUIPrefab != null && !IsBoss)
        {
            GameObject enemiesUI = GameObject.Find("EnemiesUICollection");
            if (enemiesUI == null)
            {
                enemiesUI = new GameObject("EnemiesUICollection");
            }

            EnemyUI = Instantiate(enemyUIPrefab, enemiesUI.transform);
            RectTransform enemyUITransform = EnemyUI.GetComponent<RectTransform>();
            enemyUITransform.localPosition = Data.canvaOffset;
            EnemyUI.SetActive(false);

            StatBarUI enemyHealthBar = EnemyUI.GetComponentInChildren<StatBarUI>();
            if(enemyHealthBar != null)
            {
                enemyHealthBar.SetInitValue(Stats.Health.MaxValue, Stats.Health.MaxValue);
            }

            Stats.Health.OnValueChanged += (_, currValue, maxValue) =>
            {
                if (enemyHealthBar != null)
                {
                    EnemyUI.SetActive(true);
                    enemyHealthBar.SetTarget(currValue, maxValue);
                }
            };

            TextMeshProUGUI nameText = EnemyUI.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            nameText.text = Data.enemyType.ToString();
        }

        TransitionToState(startingState);
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

        if (EnemyUI != null)
        {
            Destroy(EnemyUI);
        }
    }

    void Update()
    {
        Core.LogicUpdate();
        if (!isDead && CurrentState != null) 
            CurrentState.UpdateState(this);

        if (EnemyUI != null)
        {
            EnemyUI.transform.position = transform.position + new Vector3(Data.canvaOffset.x, Data.canvaOffset.y, 0.0f);
        }

        UpdateAttackCD();
    }

    void UpdateAttackCD()
    {
        var keys = new List<EnemyAttackState>(attacksCDList.Keys);

        foreach (var key in keys)
        {
            attacksCDList[key] -= Time.deltaTime;
            if (attacksCDList[key] <= 0)
            {
                attacksCDList.Remove(key);
            }
        }
    }

    public void TransitionToState(EnemyState next)
    {
        if (next == null || next == remainState || next == CurrentState) return;
        CurrentState = next;
        stateEnterTime = Time.time;
        CurrentState.EnterState(this);
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

        if (Data.isFlying)
        {
            RB.gravityScale = 1;
        }
    }

    private void HandleHurtState(GameObject source)
    {
        if (CurrentState == null || isDead) return;
        if (CurrentState.IsAttackState && 
            !isAnimationFinished) return;
        lastSeenPlayerPoint = player.position;
        TransitionToState(hurtState);
    }

    private void StartAttackWindow(AnimationWindows windows)
    {
        if (CurrentState.IsAttackState)
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
        if (CurrentState.IsAttackState)
        {
            if (windows == AnimationWindows.Attack)
            {
                isAttacking = false;

                attacksCDList.Add(currentAttackState, 
                                  Data.attackDetails[currentAttackIndex].attackCooldown);
                currentAttackIndex = -1;
                currentAttackState = null;
            }
        }
    } 

    public EnemyAttack TryGetAttack()
    {
        if (currentAttackState != null) return null;
        if (Time.time - attackRequestTime < Data.attackRequestCD) return null;

        EnemyAttack nextAttack = null;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // Check for range
        EnemyAttack[] availableAttacks = Data.attackDetails.Where(attack => distanceToPlayer <= attack.attackRange).ToArray();
        if (availableAttacks.Length == 0) return null;

        // Check for cool down
        availableAttacks = availableAttacks.Where(attack => !attacksCDList.ContainsKey(attack.attackState)).ToArray();
        if (availableAttacks.Length == 0) return null;

        // Check for type
        EnemyAttack[] specialAttacks = availableAttacks.Where(attack => attack.attackType > EnemyAttack.AttackType.Normal).ToArray();
        if(specialAttacks.Length > 0)
        {
            nextAttack = specialAttacks[UnityEngine.Random.Range(0, specialAttacks.Length)];
            return nextAttack;
        }

        nextAttack = availableAttacks[UnityEngine.Random.Range(0, availableAttacks.Length)];
        currentAttackState = nextAttack.attackState;
        currentAttackIndex = Array.IndexOf(Data.attackDetails, nextAttack);
        attackRequestTime = Time.time;
        TransitionToState(currentAttackState);
        return nextAttack;
    }

    public float TryGetAttackRange()
    {
        float highestAttackRange = 0f;

        if (Data == null) return highestAttackRange;

        foreach (EnemyAttack attack in Data.attackDetails)
        {
            if (highestAttackRange < attack.attackRange)
            {
                highestAttackRange = attack.attackRange;
            }
        }
        return highestAttackRange;
    }
}