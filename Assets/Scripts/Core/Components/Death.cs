using UnityEngine;

public class Death : CoreComponent
{
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
        core.transform.parent.gameObject.SetActive(false);
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
