using System.Collections;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    protected Projectile projectile;
    protected Rigidbody2D rb => projectile.Rigidbody2D;
    public bool Active { get; private set; }
    
    protected virtual void Init()
    {
        SetActive(true);
    }
    
    public virtual void SetActive(bool value) => Active = value;

    protected virtual void ResetProjectile()
    {
    }
    
    protected virtual void HandleReceiveDataPackage(ProjectileDataPackage dataPackage)
    {
            
    }
    
    public virtual void SetActiveNextFrame(bool value)
    {
        StartCoroutine(SetActiveNextFrameCoroutine(value));
    }
        
    public IEnumerator SetActiveNextFrameCoroutine(bool value)
    {
        yield return null;
        SetActive(value);
    }
    
    protected virtual void Awake()
    {
        projectile = GetComponent<Projectile>();

        projectile.OnInit += Init;
        projectile.OnReset += ResetProjectile;
        projectile.OnReceiveDataPackage += HandleReceiveDataPackage;
    }

    protected virtual void Start()
    {
            
    }

    protected virtual void Update()
    {
            
    }

    protected virtual void FixedUpdate()
    {
            
    }

    protected virtual void OnDestroy()
    {
        projectile.OnInit -= Init;
        projectile.OnReset -= ResetProjectile;
        projectile.OnReceiveDataPackage -= HandleReceiveDataPackage;
    }
}