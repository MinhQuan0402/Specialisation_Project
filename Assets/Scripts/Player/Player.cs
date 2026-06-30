using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

[RequireComponent(typeof(PlayerInput))]
public class Player : SingletonTemplate<Player>
{
    #region State Variable
    [SerializeField] private SerializableDictionary<string, State> StatesDictionary = new();

    [SerializeField] private PlayerData playerData;
    public PlayerData PlayerData { get => playerData; private set => playerData = value; }

    [HideInInspector] public PlayerIdleState idleState;
    [HideInInspector] public PlayerMoveState moveState;
    [HideInInspector] public PlayerJumpState jumpState;
    [HideInInspector] public PlayerInAirState inAirState;
    [HideInInspector] public PlayerLandState landState;
    [HideInInspector] public PlayerWallClimbState wallClimbState;
    [HideInInspector] public PlayerWallGrabState wallGrabState;
    [HideInInspector] public PlayerWallSlideState wallSlideState;
    [HideInInspector] public PlayerWallJumpState wallJumpState;
    [HideInInspector] public PlayerLedgeClimbState ledgeClimbState;
    [HideInInspector] public PlayerDashState dashState;
    [HideInInspector] public PlayerAttackState primaryAttackState;
    [HideInInspector] public PlayerAttackState secondaryAttackState;

    [HideInInspector] public Weapon primaryWeapon;
    [HideInInspector] public Weapon secondaryWeapon;

    [field: SerializeField] public Transform DashIndicator { get; private set; }

    public bool IsIdleExist => idleState ? true : StateMachine.GetStateWithRef(ref idleState);
    public bool IsMoveExist => moveState ? true : StateMachine.GetStateWithRef(ref moveState);
    public bool IsJumpExist => jumpState ? true : StateMachine.GetStateWithRef(ref jumpState);
    public bool IsInAirExist => inAirState ? true : StateMachine.GetStateWithRef(ref inAirState);
    public bool IsLandExist => landState ? true : StateMachine.GetStateWithRef(ref landState);
    public bool IsWallClimbExist => wallClimbState ? true : StateMachine.GetStateWithRef(ref wallClimbState);
    public bool IsWallGrabExist => wallGrabState ? true : StateMachine.GetStateWithRef(ref wallGrabState);
    public bool IsWallSlideExist => wallSlideState ? true : StateMachine.GetStateWithRef(ref wallSlideState);
    public bool IsWallJumpExist => wallJumpState ? true : StateMachine.GetStateWithRef(ref wallJumpState);
    public bool IsLedgeClimbExist => ledgeClimbState ? true : StateMachine.GetStateWithRef(ref ledgeClimbState);
    public bool IsDashExist => dashState ? true : StateMachine.GetStateWithRef(ref dashState);
    public bool IsPrimaryAttackExist => primaryAttackState ? true : StateMachine.GetStateWithRef("primaryAttack", ref primaryAttackState);
    public bool IsSecondaryAttackExist => secondaryAttackState ? true : StateMachine.GetStateWithRef("secondaryAttack", ref secondaryAttackState);

    public bool IsPrimaryWeaponExist => primaryWeapon ? true : false;
    public bool IsSecondaryWeaponExist => secondaryWeapon ? true : false;

    [field: SerializeField, ReadOnlyInspector] public bool IsInteruptible { get; private set; } = false;
    private Coroutine disableInteruption;

    public Action OnUpdatePlayer;

    private PlayerInput playerInput;

    #endregion

    #region Components
    public Core Core 
    {  
        get
        {
            if (core == null)
                core = GetComponentInChildren<Core>();
            return core;
        }
        private set
        {
            core = value;
        }
    }
    private Core core;

    public StateMachine StateMachine {  get; private set; }
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public CapsuleCollider2D Collider { get; private set; }
    public InventorySystem InventorySystem { get; private set; }
    public InteractableDetector InteractableDetector { get; private set; }

    public CoreComp<Stats> Stats { get; private set; }
    public CoreComp<Movement> Movement { get; private set; }

    public bool IsFreezing { get; private set; } = false;

    private Coroutine hurtCoroutine;
    private SpriteRenderer SR;

    #endregion

    #region Unity Functions

    protected override void Awake()
    {
        base.Awake();

        Core ??= GetComponentInChildren<Core>();

        StateMachine = new StateMachine();
        StateMachine.InitializeStates(ref StatesDictionary);

        _ = IsIdleExist;
        _ = IsMoveExist;
        _ = IsJumpExist;
        _ = IsWallJumpExist;
        _ = IsWallSlideExist;
        _ = IsLandExist;
        _ = IsInAirExist;
        _ = IsWallClimbExist;
        _ = IsWallGrabExist;
        _ = IsLedgeClimbExist;
        _ = IsDashExist;
        
        Stats ??= new CoreComp<Stats>(Core);
        Movement ??= new CoreComp<Movement>(Core);
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        Collider = GetComponent<CapsuleCollider2D>();
        InputHandler = Core.GetCoreComponent<PlayerInputHandler>();
        InventorySystem = Core.GetCoreComponent<InventorySystem>();
        InteractableDetector = Core.GetCoreComponent<InteractableDetector>();

        foreach (KeyValue<string, State> pairs in StatesDictionary.pairs) { if (pairs.Value) pairs.Value.Init(); }
        StateMachine.InitializeStartingState(StateMachine.GetState<PlayerIdleState>()); //Enter idle state as default

        primaryWeapon = transform.Find("PrimaryWeapon").GetComponent<Weapon>();
        secondaryWeapon = transform.Find("SecondaryWeapon").GetComponent<Weapon>();
        if (IsPrimaryAttackExist) primaryAttackState.Init(primaryWeapon, CombatInputs.primary);
        if (IsSecondaryAttackExist) secondaryAttackState.Init(secondaryWeapon, CombatInputs.secondary);
        
        Stats.Comp.Poise.OnCurrentValueZero += HandlePoiseZero;

        if (Core.TryGetCoreComponent(out DamageReceiver dmgReceiver))
        {
            dmgReceiver.OnTakingDamage += HandleTakingDamage;        
        }

        Stats.Comp.Health.Init(playerData.maxHealth);
        Stats.Comp.Stamina.Init(playerData.maxStatmina);
        UIManager.Instance.SetHealthBar(Stats.Comp.Health.MaxValue);
        UIManager.Instance.SetStaminaBar(Stats.Comp.Stamina.MaxValue);

        Stats.Comp.Health.OnValueChanged += UIManager.Instance.Health_OnValueChanged;
        Stats.Comp.Stamina.OnValueChanged += UIManager.Instance.Stamina_OnValueChanged;

        InputHandler.OnInteractInputChanged += InteractableDetector.TryInteract;
    }

    private void OnEnable()
    {
        Stats ??= new CoreComp<Stats>(Core);
        Movement ??= new CoreComp<Movement>(Core);

        Stats.Comp.Health.OnValueChanged += UIManager.Instance.Health_OnValueChanged;
        Stats.Comp.Stamina.OnValueChanged += UIManager.Instance.Stamina_OnValueChanged;

        Stats.Comp.Health.OnCurrentValueZero += GameManager.Instance.PlayerDied;
    }

    private void OnDisable()
    {
        if (disableInteruption != null)
        {
            StopCoroutine(disableInteruption);
            disableInteruption = null;
        }

        if (hurtCoroutine != null)
        {
            StopCoroutine(hurtCoroutine);
            hurtCoroutine = null;
        }

        Stats.Comp.Health.OnCurrentValueZero -= GameManager.Instance.PlayerDied;

        Stats.Comp.Health.OnValueChanged -= UIManager.Instance.Health_OnValueChanged;
        Stats.Comp.Stamina.OnValueChanged -= UIManager.Instance.Stamina_OnValueChanged;
    }

    private void HandlePoiseZero()
    {
        StateMachine.ChangeState("Stun");
    }

    void HandleTakingDamage(GameObject _)
    {
        HandleInteruption();

        if (gameObject.activeSelf)
            hurtCoroutine = StartCoroutine(OnHurtIndicator());
    }

    private void HandleInteruption()
    {
        if (disableInteruption != null)
        {
            StopCoroutine(disableInteruption);
        }

        IsInteruptible = true;

        if (gameObject.activeSelf)
            disableInteruption = StartCoroutine(OnDisableInteruption());
    }

    IEnumerator OnDisableInteruption()
    {
        float enterTime = Time.time;
        while (Time.time - enterTime < 0.25f)
        {

            yield return null;
        }

        IsInteruptible = false;
        disableInteruption = null;
    }

    IEnumerator OnHurtIndicator()
    {
        float timer = 0.0f;
        float maxTime = 0.15f;
        while (timer < maxTime)
        {
            timer += Time.deltaTime;
            SR.color = new Color(timer / maxTime, 0.0f, 0.0f, 1.0f);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        SR.color = Color.white;
        yield return null;
    }
    
    private void OnDestroy()
    {
        Stats.Comp.Poise.OnCurrentValueZero -= HandlePoiseZero;
        if (Core.TryGetCoreComponent(out DamageReceiver dmgReceiver))
        {
            dmgReceiver.OnTakingDamage -= HandleTakingDamage;
        }
    }

    private void Update()
    {
        StateMachine.LogicUpdate();
        Core.LogicUpdate();
        OnUpdatePlayer?.Invoke();
    }

    private void FixedUpdate()
    {
        StateMachine.PhysicsUpdate();
    }
    #endregion

    public void Respawn(Vector2 spawnPoint)
    {
        gameObject.SetActive(true);
        transform.position = spawnPoint;
        Stats.Comp.Health.Init(Stats.Comp.Health.MaxValue);
        Stats.Comp.Stamina.Init(Stats.Comp.Health.MaxValue);
        RB.linearVelocity = Vector2.zero;
        if (Movement.Comp.FacingDirection != 1)
        {
            Movement.Comp.Flip();
        }
        SR.color = Color.white;
        IsInteruptible = false;

        StateMachine.ChangeState(idleState);
    }

    public void Paused(float delayInSec = 0)
    {
        if (delayInSec <= 0.0f)
        {
            IsFreezing = true;
            playerInput.enabled = false;
            return;
        }

        StartCoroutine(DelayFreeze(delayInSec, true));
    }

    public void Unpaused(float delayInSec = 0)
    {
        if (delayInSec < 0.0f)
        {
            IsFreezing = false;
            playerInput.enabled = true;
            return;
        }

        StartCoroutine(DelayFreeze(delayInSec, false));
    }

    IEnumerator DelayFreeze(float delayDuration, bool isPause)
    {
        yield return new WaitForSeconds(delayDuration);

        if (isPause)
        {
            IsFreezing = true;
            playerInput.enabled = false;
        }
        else
        {
            IsFreezing = false;
            playerInput.enabled = true;
        }
    }

#pragma warning disable IDE0051 // Remove unused private members
    private void AnimationTrigger() => ((PlayerState)StateMachine.CurrentState).AnimationTrigger();

    private void AnimationFinishTrigger() => ((PlayerState)StateMachine.CurrentState).AnimationFinishTrigger();
#pragma warning restore IDE0051 // Remove unused private members

}
