using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Check")]
public class CheckAction : EnemyAction
{
    [SerializeField]
    private float checkDuration = 0.5f;
    [SerializeField]
    private int maxNumChecks = 3;

    public override void Act(EnemyController controller)
    {
        if (controller.numChecks == 0)
        {
            controller.isCheckingDone = false;
            controller.lastCheckTime = Time.time - checkDuration;
        }

        if (controller.numChecks < maxNumChecks && 
            Time.time - controller.lastCheckTime >= checkDuration)
        {
            controller.lastCheckTime = Time.time;
            controller.numChecks++;
            controller.Movement.Flip();
        }

        if (controller.numChecks >= maxNumChecks)
        {
            controller.isCheckingDone = true;
        }
    }
}