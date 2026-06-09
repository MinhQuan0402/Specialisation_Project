using System;
using UnityEngine;

public static class ZoneEventBus
{
    public static event Action<StepData, GameObject> OnZoneEntered;
    public static event Action<StepData, GameObject> OnZoneExited;

    public static void TriggerZoneEntered(StepData step, GameObject who)
        => OnZoneEntered?.Invoke(step, who);

    public static void TriggerZoneExited(StepData step, GameObject who)
        => OnZoneExited?.Invoke(step, who);
}