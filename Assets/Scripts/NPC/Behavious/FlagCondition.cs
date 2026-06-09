using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class FlagCondition : MonoBehaviour, INPCCondition
{
    [SerializeField] private string flagID;
    public bool IsMet => GameFlagRegistry.IsSet(flagID);
}