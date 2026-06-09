using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public static class CutsceneEventBus
{
    public static event Action<string> OnCutsceneStarted;
    public static event Action<string> OnCutsceneEnded;

    public static void TriggerCutsceneStarted(string id) => OnCutsceneStarted?.Invoke(id);
    public static void TriggerCutsceneEnded(string id) => OnCutsceneEnded?.Invoke(id);
}