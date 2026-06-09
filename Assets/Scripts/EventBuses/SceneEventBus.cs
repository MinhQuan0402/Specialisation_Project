using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public static class SceneEventBus
{
    public static event Action OnPlayerDied;

    public static void TriggerPlayerDied() => OnPlayerDied?.Invoke();
}