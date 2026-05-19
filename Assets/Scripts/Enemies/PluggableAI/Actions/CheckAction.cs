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

    private readonly Dictionary<EnemyController, int> _numTurns = new();

    public override void Act(EnemyController controller)
    {
        if (!_numTurns.TryGetValue(controller, out int numTurns))
        {
            _numTurns.Add(controller, 0);
            controller.isCheckingDone = false;
            controller.Movement.Flip();
            controller.lastCheckTime = Time.time;
        }

        if (numTurns < maxNumChecks && Time.time - controller.lastCheckTime >= checkDuration)
        {
            controller.lastCheckTime = Time.time;
            _numTurns[controller] = numTurns + 1;
            controller.Movement.Flip();
        }

        if (numTurns >= maxNumChecks)
        {
            controller.isCheckingDone = true;
            PruneOldKeys(controller);
        }
    }

    void PruneOldKeys(EnemyController c)
    {
        _numTurns.Remove(c);
    }
}