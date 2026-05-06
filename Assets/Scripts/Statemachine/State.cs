using UnityEngine;

public abstract class State : ScriptableObject
{
    public Core Core {  get; protected set; }
    protected StateMachine stateMachine;
    protected Animator anim;

    protected float startTime;

    protected string animBoolName;

    protected bool isAnimationFinished;
    protected bool isExitingState;
    public virtual void Init() { }

    public virtual void Enter()
    {
        DoChecks();
        startTime = Time.time;
        anim.SetBool(animBoolName, true);

        isAnimationFinished = false;
        isExitingState = false;
    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
        isExitingState = true;
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }

    public virtual void AnimationTrigger() { }

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
}
