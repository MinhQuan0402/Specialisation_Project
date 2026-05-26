using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Pathfinding")]
public class PathfindingAction : EnemyAction
{
    private enum PathfindingType
    {
        Waypoint,
        Player
    }

    [SerializeField] private PathfindingType pathfindingType;

    private enum SpeedType
    {
        PatrolSpeed,
        ChasingSpeed
    }

    [SerializeField] private SpeedType speedType;

    public override void Act(EnemyController controller)
    {
        controller.DestinationSetter.target = pathfindingType == PathfindingType.Waypoint ? 
                                              controller.Waypoints[controller.currentWaypoint] :
                                              controller.player;
        controller.AIPath.maxSpeed = speedType == SpeedType.PatrolSpeed ?
                                     controller.Data.patrolSpeed :
                                     controller.Data.chaseSpeed;
    }
}