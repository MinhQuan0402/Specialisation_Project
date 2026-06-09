using Assets.Scripts.Common;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CommonAttackData))]
public class CommonAttackDataDrawer : PropertyDrawer
{
    readonly float line = EditorGUIUtility.singleLineHeight;
    const float space = 2f;

    public override void OnGUI(
        Rect pos,
        SerializedProperty prop,
        GUIContent label)
    {
        Rect r = pos;
        r.height = line;

        EditorGUI.BeginProperty(pos, label, prop);

        prop.isExpanded = EditorGUI.Foldout(
            r,
            prop.isExpanded,
            label,
            true
        );


        if (prop.isExpanded)
        {
            r.y += line + space;

            SerializedProperty useDamage = prop.FindPropertyRelative("UseDamage");
            SerializedProperty damage = prop.FindPropertyRelative("damageAmount");

            EditorGUI.PropertyField(r, useDamage);
            r.y += EditorGUI.GetPropertyHeight(useDamage) + 2;

            if (useDamage.boolValue)
            {
                EditorGUI.PropertyField(r, damage);
                r.y += EditorGUI.GetPropertyHeight(damage) + 2;
            }

            SerializedProperty useKnockback = prop.FindPropertyRelative("UseKnockBack");
            SerializedProperty knockbackAmount = prop.FindPropertyRelative("knockbackAmount");
            SerializedProperty knockbackAngle = prop.FindPropertyRelative("knockbackAngle");

            EditorGUI.PropertyField(r, useKnockback);
            r.y += EditorGUI.GetPropertyHeight(useKnockback) + 2;

            if (useKnockback.boolValue)
            {
                EditorGUI.PropertyField(r, knockbackAmount);
                r.y += EditorGUI.GetPropertyHeight(knockbackAmount) + 2;

                EditorGUI.PropertyField(r, knockbackAngle);
                r.y += EditorGUI.GetPropertyHeight(knockbackAngle) + 2;
            }

            SerializedProperty usePoise = prop.FindPropertyRelative("UsePoise");
            SerializedProperty poiseAmount = prop.FindPropertyRelative("poiseAmount");

            EditorGUI.PropertyField(r, usePoise);
            r.y += EditorGUI.GetPropertyHeight(usePoise) + 2;

            if (usePoise.boolValue)
            {
                EditorGUI.PropertyField(r, poiseAmount);
                r.y += EditorGUI.GetPropertyHeight(poiseAmount) + 2;
            }
        }

         EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(
        SerializedProperty prop,
        GUIContent label)
    {
        if (!prop.isExpanded)
            return line;

        float height = (line + space) * 2;
        SerializedProperty useDamage = prop.FindPropertyRelative("UseDamage");
        if (useDamage.boolValue)
        {
            height += line + space;
        }

        height += line + space;
        SerializedProperty useKnockback = prop.FindPropertyRelative("UseKnockBack");
        if (useKnockback.boolValue)
        {
            height += line + space;
            height += line + space;
        }

        height += line + space;
        SerializedProperty usePoise = prop.FindPropertyRelative("UsePoise");
        if (usePoise.boolValue)
        {
            height += line + space;
        }

        return height;
    }

}
