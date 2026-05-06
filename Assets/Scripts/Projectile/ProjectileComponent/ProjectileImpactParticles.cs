using UnityEngine;

public class ProjectileImpactParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem impactParticles;

    public void SpawnImpactParticles(Vector3 position, Quaternion rotation)
    {
        if(impactParticles == null) return;
        Instantiate(impactParticles, position, rotation);
    }

    public void SpawnImpactParticles(RaycastHit2D hit)
    {
        if(impactParticles == null) return;
        var rotation = Quaternion.FromToRotation(transform.right, hit.normal);
            
        SpawnImpactParticles(hit.point, rotation);
    }

    public void SpawnImpactParticles(RaycastHit2D[] hits)
    {
        if(impactParticles == null) return;
        if(hits.Length <= 0 )
            return;
            
        SpawnImpactParticles(hits[0]);
    }
}