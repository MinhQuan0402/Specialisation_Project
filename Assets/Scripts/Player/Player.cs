using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public static Player Instance {  get; private set; }

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

    #endregion

    #region Components
    public Core Core {  get; private set; }
    public StateMachine StateMachine {  get; private set; }
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    
    private CoreComp<Stats> stats;
    
    #endregion

    #region Unity Functions
    private void Awake()
    {
        if(Instance != null) 
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Core = GetComponentInChildren<Core>();

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
        
        stats = new CoreComp<Stats>(Core);
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponentInChildren<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();

        foreach (KeyValue<string, State> pairs in StatesDictionary.pairs) { if (pairs.Value) pairs.Value.Init(); }
        StateMachine.InitializeStartingState(StateMachine.GetState<PlayerIdleState>()); //Enter idle state as default

        primaryWeapon = transform.Find("PrimaryWeapon").GetComponent<Weapon>();
        secondaryWeapon = transform.Find("SecondaryWeapon").GetComponent<Weapon>();
        if (IsPrimaryAttackExist)
            primaryAttackState.SetWeapon(primaryWeapon, CombatInputs.primary);
        if (IsSecondaryAttackExist)
            secondaryAttackState.SetWeapon(secondaryWeapon, CombatInputs.secondary);
        
        stats.Comp.Poise.OnCurrentValueZero += HandlePoiseZero;
    }

    private void HandlePoiseZero()
    {
        StateMachine.ChangeState("Stun");
    }
    
    private void OnDestroy()
    {
        stats.Comp.Poise.OnCurrentValueZero -= HandlePoiseZero;
    }

    private void Update()
    {
        StateMachine.LogicUpdate();
        Core.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.PhysicsUpdate();
    }
    #endregion

#pragma warning disable IDE0051 // Remove unused private members
    private void AnimationTrigger() => ((PlayerState)StateMachine.CurrentState).AnimationTrigger();

    private void AnimationFinishTrigger() => ((PlayerState)StateMachine.CurrentState).AnimationFinishTrigger();
#pragma warning restore IDE0051 // Remove unused private members

}
