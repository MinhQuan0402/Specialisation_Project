using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ZoneCondition : MonoBehaviour, INPCCondition
{
    [SerializeField] private StepData requiredStepData;
    private bool playerInZone = false;

    public bool IsMet => playerInZone;

    private void OnEnable()
    {
        ZoneEventBus.OnZoneEntered += OnZoneEntered;
        ZoneEventBus.OnZoneExited += OnZoneExited;
    }

    private void OnDisable()
    {
        ZoneEventBus.OnZoneEntered -= OnZoneEntered;
        ZoneEventBus.OnZoneExited -= OnZoneExited;
    }

    private void OnZoneEntered(StepData data, GameObject who)
    {
        if (requiredStepData == data) playerInZone = true;
    }

    private void OnZoneExited(StepData data, GameObject who)
    {
        if (requiredStepData == data) playerInZone = false;
    }
}