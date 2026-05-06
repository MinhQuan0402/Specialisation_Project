using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public class ProjectileDirectTowardsTarget : ProjectileComponent
{
    [SerializeField] private float minStep = 0.1f;
    [SerializeField] private float maxStep = 0.5f;
    [SerializeField] private float timeToMaxStep = 0.5f;
    
    private List<Transform> targets = new();
    private Transform currentTarget;
    
    private float step;
    private float startTime;
    
    private Vector2 direction;

    private float timer = 0;
    
    private ObjectPoolItem objectPoolItem;
    private ProjectileStickToLayer stickToLayer;

    protected override void Init()
    {
        base.Init();
        currentTarget = null;
        startTime = Time.time;
        step = minStep;
        timer = 0;
    }
    
    protected override void Awake()
    {
        base.Awake();
        objectPoolItem = GetComponent<ObjectPoolItem>();
        stickToLayer = GetComponent<ProjectileStickToLayer>();
    }

    protected override void Update()
    {
        base.Update();

        if (!HasTarget())
        {
            if (stickToLayer == null && objectPoolItem != null)
            {
                if(Time.time >= startTime + 5f)
                    objectPoolItem.ReturnItem(0);
            }
            return;
        }

        if (Time.time >= startTime + 2f && step >= maxStep)
        {
            timer += Time.deltaTime;
            step += timer;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!HasTarget())
            return;
        
        if(step < maxStep)
            step = Mathf.Lerp(minStep, maxStep, (Time.time - startTime) / timeToMaxStep);
        direction = (currentTarget.position - transform.position).normalized;

        Rotate(direction);
    }

    private bool HasTarget()
    {
        if (currentTarget)
            return true;

        targets.RemoveAll(item => item == null);

        if (targets.Count <= 0)
            return false;

        targets = targets.OrderBy(target => (target.position - transform.position).sqrMagnitude).ToList();
        currentTarget = targets[0];
        return true;
    }

    private void Rotate(Vector2 dir)
    {
        if (dir.Equals(Vector2.zero))
            return;

        var toRotation = QuaternionExtensions.Vector2ToRotation(dir);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, step * Time.deltaTime);
    }

    protected override void HandleReceiveDataPackage(ProjectileDataPackage dataPackage)
    {
        base.HandleReceiveDataPackage(dataPackage);

        if (dataPackage is not ProjectileTargetsDataPackage targetsDataPackage)
            return;

        targets = targetsDataPackage.Targets;
    }

    private void OnDrawGizmos()
    {
        if (!currentTarget)
            return;

        Gizmos.DrawLine(transform.position, currentTarget.position);
    }
}