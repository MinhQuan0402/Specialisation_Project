using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/NearWaypoint")]
public class NearWaypointDecision : EnemyDecision
{
    public override bool MakeDecision(EnemyController controller)
    {
        return Vector2.Distance(controller.transform.position, 
                                controller.Waypoints[controller.currentWaypoint].position) <= 
                                controller.AIPath.pickNextWaypointDist;
    }
}