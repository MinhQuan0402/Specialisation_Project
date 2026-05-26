using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "PluggableAI/Actions/SetNextDestination")]
public class SetNextDestinationAction : EnemyAction
{
    enum GenerateType
    {
        Default = 0,
        Random
    }

    [SerializeField] private GenerateType generateType;

    public override void Act(EnemyController controller)
    {
        if (controller.Waypoints.Length < 2) Debug.LogError("There must be at least 2");

        controller.currentWaypoint = generateType == GenerateType.Default ? 
                                      DefaultWaypoint(controller) : RandomWaypoint(controller);
    }

    private int DefaultWaypoint(EnemyController controller)
    {
        int nextPoint = controller.currentWaypoint + 1;

        if (nextPoint >= controller.Waypoints.Length)
            nextPoint = 0;

        return nextPoint;
    }

    private int RandomWaypoint(EnemyController controller)
    {
        int nextPoint;
        do
        {
            nextPoint = UnityEngine.Random.Range(0, controller.Waypoints.Length);
        } while (nextPoint == controller.currentWaypoint);

        return nextPoint;
    }
}