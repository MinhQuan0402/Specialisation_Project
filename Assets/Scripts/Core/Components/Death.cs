using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Death : CoreComponent
{
    public enum OnZeroHealth
    {
        None,
        Destroy,
        Disable
    }

    public OnZeroHealth onZeroHealth;
    public bool useAnimation = false;

    [HideInInspector]
    public bool isAnimationFinished = false;

    [SerializeField] private GameObject[] deathParticles;

    [SerializeField] private UnityEvent OnDeath;

    private ParticleManager ParticleManager => particleManager ? particleManager : core.GetCoreComponent(ref particleManager);
    private Stats Stats => stats ? stats : core.GetCoreComponent(ref stats);

    private ParticleManager particleManager;
    private Stats stats;
    public override void LogicUpdate() { }

    public void Die()
    {
        if(useAnimation && !isAnimationFinished)
        {
            return;
        }

        foreach (var particle in deathParticles)
        {
            if (particle) ParticleManager.StartParticle(particle);
        }

        if (onZeroHealth == OnZeroHealth.Disable)
        {
            core.transform.parent.gameObject.SetActive(false);
        }
        else if (onZeroHealth == OnZeroHealth.Destroy)
        {
            Destroy(core.transform.parent.gameObject);
        }

        OnDeath.Invoke();
    }

    public void SetParticles(GameObject[] deathParticles)
    {
        this.deathParticles = deathParticles;
    }
    private void OnEnable()
    {
        Stats.Health.OnCurrentValueZero += Die;
    }

    private void OnDisable()
    {
        Stats.Health.OnCurrentValueZero -= Die;
    }
}
