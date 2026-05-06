using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State/Attack")]
public class PlayerAttackState : PlayerAbilityState
{
    private Weapon equppedWeapon;
    private int inputIndex;

    private bool checkFlip;
    
    public void SetWeapon(Weapon weapon, CombatInputs input)
    {
        equppedWeapon = weapon;
        if(weapon == null ) return;
        equppedWeapon.OnExit += ExitHandler;
        equppedWeapon.SetCore(Core);
        inputIndex = (int)input;
        equppedWeapon.EventHandler.OnFlipSetActive += HandleFlipSetActive;
    }

    private void HandleFlipSetActive(bool value)
    {
        checkFlip = value;
    }

    public override void Enter()
    {
        base.Enter();
        if (equppedWeapon.Equals(null)) return;
        equppedWeapon.Enter();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        equppedWeapon.CurrentInput = player.InputHandler.AttackInputs[inputIndex];
        var xInput = player.InputHandler.NormInputX;

        if (checkFlip)
        {
            Movement.CheckIfShouldFlip(xInput);
        }
    }

    private void ExitHandler()
    {
        AnimationFinishTrigger();
        isAbilityDone = true;
    }

    public override void Init()
    {
        base.Init();

        animBoolName = "attack";
    }
}
