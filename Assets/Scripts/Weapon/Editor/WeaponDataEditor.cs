using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.Callbacks;
using System.Linq;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    private static List<Type> dataCompTypes = new();

    private WeaponData data;

    private bool showForceUpdateButtons;
    private bool showAddComponentButtons;

    private void OnEnable()
    {
        data = target as WeaponData;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Set Number Of Attacks"))
        {
            foreach(var item in data.ComponentData)
            {
                item.InitializeAttackData(data.NumberOfAttacks);
            }
        }

        showAddComponentButtons = EditorGUILayout.Foldout(showAddComponentButtons, "Add Components");

        if(showAddComponentButtons)
        {
            foreach (var type in dataCompTypes)
            {
                if (GUILayout.Button(type.Name.Replace("Weapon", "")))
                {
                    if (Activator.CreateInstance(type) is not ComponentData comp) return;
                    comp.InitializeAttackData(data.NumberOfAttacks);
                    data.AddData(comp);
                    EditorUtility.SetDirty(data);
                }
            }
        }

        showForceUpdateButtons = EditorGUILayout.Foldout(showForceUpdateButtons, "Force Update Buttons");

        if(showForceUpdateButtons)
        {
            if (GUILayout.Button("Force Update Component Names"))
            {
                foreach (var item in data.ComponentData)
                {
                    item.SetComponentName();
                }
            }

            if (GUILayout.Button("Force Update Attack Names"))
            {
                foreach (var item in data.ComponentData)
                {
                    item.SetAttackDataNames();
                }
            }
        }
    }

    [DidReloadScripts]
    private static void OnRecompile()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(assembly  => assembly.GetTypes());
        var filteredTypes = types.Where(
            type => type.IsSubclassOf(typeof(ComponentData)) && !type.ContainsGenericParameters && type.IsClass
        );

        dataCompTypes = filteredTypes.ToList();
    }
}
