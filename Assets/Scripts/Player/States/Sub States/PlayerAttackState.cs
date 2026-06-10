using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Attack")]
public class PlayerAttackState : PlayerAbilityState
{
    private Weapon weapon;
    private WeaponGenerator weaponGenerator;

    private int inputIndex;

    private bool canInterrupt;

    private bool checkFlip;
    
    public void Init(Weapon weapon, CombatInputs input)
    {
        this.weapon = weapon;
        if(weapon == null ) return;

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
        weaponGenerator.OnWeaponGenerating -= HandleWeaponGenerating;
        weapon.Exit();

        player.RB.linearDamping = 0.0f;
    }

    private void HandleWeaponGenerating()
    {
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

    public bool CanTransitionToAttackState() => weapon.CanEnterAttack && weapon.Data != null;

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
}