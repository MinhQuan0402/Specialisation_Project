using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public static class GameFlagRegistry
{
    private static readonly HashSet<string> flags = new();

    public static void Set(string flagID) => flags.Add(flagID);
    public static void Clear(string flagID) => flags.Remove(flagID);
    public static bool IsSet(string flagID) => flags.Contains(flagID);
    public static void ClearAll() => flags.Clear(); // call on new scene
}