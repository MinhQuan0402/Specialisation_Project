using UnityEngine;

public class ParticleManager : CoreComponent
{
    private Transform particleContainer;

    private Movement movement;
    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);

    protected override void Awake()
    {
        base.Awake();
        
        var container = GameObject.FindWithTag("ParticleManager");
        container ??= new GameObject("ParticleManager");
        container.tag = "ParticleManager";
        particleContainer = container.transform;
    }
    public override void LogicUpdate() { }

    public GameObject StartParticle(GameObject particlePrefab, Vector2 position, Quaternion rotation)
    {
        return Instantiate(particlePrefab, (Vector3)position, rotation, particleContainer);
    }

    public GameObject StartParticle(GameObject particlePrefab)
    {
        return StartParticle(particlePrefab, transform.position, Quaternion.identity);
    }
    
    public GameObject StartParticles(GameObject particlePrefab, Vector2 position, Quaternion rotation)
    {
        return Instantiate(particlePrefab, position, rotation, particleContainer);
    }

    public GameObject StartParticleWithRandomRotation(GameObject particlePrefab)
    {
        var randomRotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        return StartParticle(particlePrefab, transform.position, randomRotation);
    }

    public GameObject StartParticleWithRandomRotation(GameObject prefab, Vector2 offset)
    {
        var randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        return StartParticles(prefab, FindRelativePoint(offset), randomRotation);
    }

    public GameObject StartParticlesRelative(GameObject particlePrefab, Vector2 offset, Quaternion rotation)
    {
        var pos = FindRelativePoint(offset);

        return StartParticles(particlePrefab, pos, rotation);
    }
    
    private Vector2 FindRelativePoint(Vector2 offset) => Movement.FindRelativePoint(offset);
}
