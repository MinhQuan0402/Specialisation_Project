using System;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public event Action OnFinish;
    public event Action OnStartMovement;
    public event Action OnStopMovement;
    public event Action OnAttackAction;
    public event Action OnMinHoldPassed;
    public event Action<AttackPhases> OnEnterAttackPhase;
    public event Action<bool> OnSetOptionalSpriteActive;
    
    public event Action<bool> OnFlipSetActive;

    public void AnimationFinishedTrigger() => OnFinish?.Invoke();
    public void StartMovementTrigger() => OnStartMovement?.Invoke();
    public void StopMovementTrigger() => OnStopMovement?.Invoke();
    public void AttackAction() => OnAttackAction?.Invoke();
    public void MinHoldPassedTrigger() => OnMinHoldPassed?.Invoke();
    public void EnterAttackPhaseTrigger(AttackPhases phase) => OnEnterAttackPhase?.Invoke(phase);
    
    public void SetFlipActive() => OnFlipSetActive?.Invoke(true);
    public void SetFlipInactive() => OnFlipSetActive?.Invoke(false);
    
    public void SetOptionalSpriteEnabled() => OnSetOptionalSpriteActive?.Invoke(true);
    public void SetOptionalSpriteDisabled() => OnSetOptionalSpriteActive?.Invoke(false);
}
