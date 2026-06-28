using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentUtilities
{
    public static bool IsInteractable(this Component component, out IInteractable interactable)
    {
        return component.TryGetComponent(out interactable);
    }
}