using System;
using UnityEditor;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public event Action OnFinish;
    public event Action OnStartMovement;
    public event Action OnStopMovement;
    public event Action OnAttackAction;
    public event Action OnMinHoldPassed;

    /*
    * This trigger is used to indicate in the weapon animation when the input should be "used" meaning the player has to release the input key and press it down again to trigger the next attack.
    * Generally this animation event is added to the first "action" frame of an animation. e.g the first sword strike frame, or the frame where the bow is released.
    */
    public event Action OnUseInput;
    public event Action OnEnableInterrupt;
    public event Action<bool> OnSetOptionalSpriteActive;
    public event Action<bool> OnFlipSetActive;
    public event Action<AttackPhases> OnEnterAttackPhase;

    /*
    * Animations events used to indicate when a specific time window starts and stops in an animation. These windows are identified using the
    * AnimationWindows enum. These windows include things like when the shield's block is active and when it can parry.
    */
    public event Action<AnimationWindows> OnStartAnimationWindow;
    public event Action<AnimationWindows> OnStopAnimationWindow;

    public void AnimationFinishedTrigger() => OnFinish?.Invoke();
    public void StartMovementTrigger() => OnStartMovement?.Invoke();
    public void StopMovementTrigger() => OnStopMovement?.Invoke();
    public void AttackAction() => OnAttackAction?.Invoke();
    public void MinHoldPassedTrigger() => OnMinHoldPassed?.Invoke();
    public void UseInputTrigger() => OnUseInput?.Invoke();
    public void EnterAttackPhaseTrigger(AttackPhases phase) => OnEnterAttackPhase?.Invoke(phase);
    
    public void SetFlipActive() => OnFlipSetActive?.Invoke(true);
    public void SetFlipInactive() => OnFlipSetActive?.Invoke(false);
    
    public void SetOptionalSpriteEnabled() => OnSetOptionalSpriteActive?.Invoke(true);
    public void SetOptionalSpriteDisabled() => OnSetOptionalSpriteActive?.Invoke(false);

    public void EnableInterrupt() => OnEnableInterrupt?.Invoke();

    public void StartAnimationWindow(AnimationWindows window) => OnStartAnimationWindow?.Invoke(window);
    public void StopAnimationWindow(AnimationWindows window) => OnStopAnimationWindow?.Invoke(window);
}
