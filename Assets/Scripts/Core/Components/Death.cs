using UnityEngine;

public class Death : CoreComponent
{
    public enum OnZeroHealth
    {
        None,
        Destroy,
        Disable
    }

    public OnZeroHealth onZeroHealth;

    [SerializeField] private GameObject[] deathParticles;
    private ParticleManager ParticleManager => particleManager ? particleManager : core.GetCoreComponent(ref particleManager);
    private Stats Stats => stats ? stats : core.GetCoreComponent(ref stats);

    private ParticleManager particleManager;
    private Stats stats;
    public override void LogicUpdate() { }

    public void Die()
    {
        foreach (var particle in deathParticles)
        {
            if(particle) ParticleManager.StartParticle(particle);
        }

        if (onZeroHealth == OnZeroHealth.Disable)
        {
            core.transform.parent.gameObject.SetActive(false);
        }
        else if (onZeroHealth == OnZeroHealth.Destroy)
        {
            Destroy(core.transform.parent.gameObject);
        }
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
