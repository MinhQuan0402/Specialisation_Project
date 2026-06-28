using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Attack")]
public class PlayerAttackState : PlayerAbilityState
{
    private Weapon weapon;
    private WeaponGenerator weaponGenerator;

    private int inputIndex;

    private bool canInterrupt;

    private bool checkFlip;

    private float cooldown = 0.0f;
    
    public void Init(Weapon weapon, CombatInputs input)
    {
        this.weapon = weapon;
        cooldown = 0.0f;
        if (weapon == null ) return;

        weaponGenerator = weapon.GetComponent<WeaponGenerator>();
        inputIndex = (int)input;

        weapon.OnUseInput += HandleUseInput;

        weapon.EventHandler.OnEnableInterrupt += HandleEnableInterrupt;
        weapon.EventHandler.OnFinish += HandleFinish;
        weapon.EventHandler.OnFlipSetActive += HandleFlipSetActive;

        weapon.SetCore(Core);
    }

    private void HandleFlipSetActive(bool value)
    {
        checkFlip = value;
    }

    public override void Enter()
    {
        base.Enter();
        if (weapon.Equals(null)) return;
        weaponGenerator.OnWeaponGenerating += HandleWeaponGenerating;
        
        Movement movement = player.Core.GetComponent<Movement>();
        movement.SetVelocityZero();

        checkFlip = true;
        canInterrupt = false;
        weapon.Enter();

        player.RB.linearDamping = playerData.attackDrag;
    }

    public override void Exit()
    {
        base.Exit();
        cooldown = 1 / weapon.Data.AttackSpeed;
        Player.Instance.OnUpdatePlayer += UpdateCooldown;
        weaponGenerator.OnWeaponGenerating -= HandleWeaponGenerating;
        weapon.Exit();

        player.RB.linearDamping = 0.0f;
    }

    public void UpdateCooldown()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0.0f)
        {
            cooldown = 0.0f;
            Player.Instance.OnUpdatePlayer -= UpdateCooldown;
        }
    }

    private void HandleWeaponGenerating()
    {
        cooldown = 0;
        stateMachine.ChangeState(player.StateMachine.GetState<PlayerIdleState>());
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        weapon.CurrentInput = player.InputHandler.AttackInputs[inputIndex];
        var xInput = player.InputHandler.NormInputX;
        var attackInputs = player.InputHandler.AttackInputs;

        if (checkFlip)
        {
            Movement.CheckIfShouldFlip(xInput);
        }

        if (!canInterrupt)
            return;

        if (xInput != 0 || Player.Instance.IsInteruptible)
        {
            isAbilityDone = true;
            player.InputHandler.UseAttackInput(inputIndex);
        }
    }

    public override void Init()
    {
        base.Init();

        animBoolName = "attack";
    }

    public bool CanAttack() => weapon.CanEnterAttack && weapon.Data != null && cooldown <= 0.0f;

    private void HandleEnableInterrupt() => canInterrupt = true;

    private void HandleUseInput()
    {
        player.InputHandler.UseAttackInput(inputIndex);
    }

    private void HandleFinish()
    {
        AnimationFinishTrigger();
        isAbilityDone = true;
    }

    public override void UseStamina()
    {
        if (weapon.Data.WeaponStamina == 0) return;
        player.Stats.Comp.Stamina.Decrease(weapon.Data.WeaponStamina);
    }
}