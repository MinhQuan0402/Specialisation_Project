using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


[CustomEditor(typeof(EnemyState))]
public class EnemyStateEditor : Editor
{
    private EnemyState data;

    private bool transibleToAttack = false;

    private void OnEnable()
    {
        data = target as EnemyState;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        transibleToAttack = GUILayout.Toggle(data.IsTransibleToAttack, "Transible To Attack State");
        if (transibleToAttack && data.AttackTransition == null)
        {
            data.GenerateAttackTransition();
        }

        if(!transibleToAttack && data.AttackTransition != null)
        {
            data.RemoveAttackTransition();
        }

        if(GUILayout.Button("Sort Transition"))
        {
            data.SortTransition();
        }
    }
}
