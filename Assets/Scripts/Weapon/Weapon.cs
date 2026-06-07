using System;
using UnityEngine;
using Utilities;

public class Weapon : MonoBehaviour
{
    public event Action<bool> OnCurrentInputChange;

    public event Action OnEnter;
    public event Action OnExit;
    public event Action OnUseInput;

    [SerializeField] private float attackCounterResetCooldown = 1f;

    public WeaponData Data { get; private set; } = null;
    
    public bool IsEquipped { get; set; }
    
    public int CurrentAttackCounter
    {
        get => currentAttackCounter;
        private set => currentAttackCounter = value >= Data.NumberOfAttacks ? 0 : value;
    }

    public bool CurrentInput
    {
        get => currentInput;
        set
        {
            if (currentInput == value) return;
            currentInput = value;
            OnCurrentInputChange?.Invoke(currentInput);
        }
    }

    private int currentAttackCounter = 0;
    private bool currentInput = false;

    private TimeNotifier attackCounterResetTimeNotifier;

    private bool initDone;
    private AnimationEventHandler eventHandler;

    public float AttackStartTime { get; private set; }
    public bool CanEnterAttack { get; private set; } = false;

    public Animator Anim { get; private set; }
    public GameObject BaseSpriteGameObject { get; private set; }
    public GameObject WeaponSpriteGameObject { get; private set; }

    public AnimationEventHandler EventHandler
    {
        get
        {
            if (!initDone)
            {
                GetDependencies();
            }
            return eventHandler;
        }
        private set => eventHandler = value;
    }

    public AnimationEventHandler SetAndGetAnimationEventHandler()
    {
        BaseSpriteGameObject = transform.Find("Base").gameObject;
        EventHandler = BaseSpriteGameObject.GetComponent<AnimationEventHandler>();
        return EventHandler;
    }

    public Core Core { get; private set; }

    private void GetDependencies()
    {
        if (initDone)
            return;

        BaseSpriteGameObject = transform.Find("Base").gameObject;
        WeaponSpriteGameObject = transform.Find("WeaponSprite").gameObject;

        Anim = BaseSpriteGameObject.GetComponent<Animator>();

        EventHandler = BaseSpriteGameObject.GetComponent<AnimationEventHandler>();

        initDone = true;
    }

    private void ResetAttackCounter() => CurrentAttackCounter = 0;

    public void SetCanEnterAttack(bool value) => CanEnterAttack = value;

    public void Enter()
    {
        AttackStartTime = Time.time;
        
        attackCounterResetTimeNotifier.Disable();
        
        Anim.SetBool("active", true);
        Anim.SetInteger("counter", CurrentAttackCounter);

        OnEnter?.Invoke();
    }
    public void SetCore(Core core) => Core = core;
    public void SetData(WeaponData data)
    {
        Data = data;
        if (Data == null)
            return;
        ResetAttackCounter();
    }
    public void Exit()
    {
        Anim.SetBool("active", false);
        CurrentAttackCounter++;
        attackCounterResetTimeNotifier.Init(attackCounterResetCooldown);
        OnExit?.Invoke();
    }

    private void Awake()
    {
        GetDependencies();
        attackCounterResetTimeNotifier = new TimeNotifier();
    }

    private void Update()
    {
        attackCounterResetTimeNotifier.Tick();
    }

    private void OnEnable()
    {
        EventHandler.OnFinish += Exit;
        EventHandler.OnUseInput += HandleUseInput;
        attackCounterResetTimeNotifier.OnNotify += ResetAttackCounter;
    }

    private void OnDisable()
    {
        EventHandler.OnFinish -= Exit;
        EventHandler.OnUseInput -= HandleUseInput;
        attackCounterResetTimeNotifier.OnNotify -= ResetAttackCounter;
    }

    private void HandleUseInput() => OnUseInput?.Invoke(); 
}
