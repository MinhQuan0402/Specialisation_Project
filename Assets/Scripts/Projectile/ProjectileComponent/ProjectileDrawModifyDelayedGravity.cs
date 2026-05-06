using UnityEngine;

public class ProjectileDrawModifyDelayedGravity : ProjectileComponent
{
    private ProjectileDelayedGravityComponent delayedGravity;

    protected override void HandleReceiveDataPackage(ProjectileDataPackage dataPackage)
    {
        base.HandleReceiveDataPackage(dataPackage);

        if (dataPackage is not ProjectileDrawModifierDataPackage drawModifierDataPackage)
            return;

        // Modify the delayed gravity distance multiplier based on draw percentage received from the weapon
        delayedGravity.distanceMultiplier = drawModifierDataPackage.DrawPercentage;
    }
    
    protected override void Awake()
    {
        base.Awake();

        delayedGravity = GetComponent<ProjectileDelayedGravityComponent>();
    }
}