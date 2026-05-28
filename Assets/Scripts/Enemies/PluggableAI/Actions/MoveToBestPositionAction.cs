using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "PluggableAI/Actions/MoveToBestPositionAction")]
public class MoveToBestPositionAction : EnemyAction
{
    [SerializeField] private float movementSpeed = 5.0f;

    public override void Act(EnemyController controller)
    {
        controller.AIPath.canMove = false;

        Vector2 directionToPlayer = controller.player.position - controller.transform.position;
        Vector2 movementDir = -directionToPlayer.normalized;
        if (directionToPlayer.y < controller.Data.closeRange)
        {
            movementDir.y = 1;
        }

        controller.Movement.SetVelocity(movementSpeed, movementDir);
    }
}