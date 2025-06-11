using System;
using RotParams;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RotParams_Quaternion))]
public class RotParamsQuaternionDrawer : PropertyDrawer
{
    private bool initialized = false;

    private SerializedProperty enforceNormalizationProp;
    private SerializedProperty wProp, xProp, yProp, zProp;
    private SerializedProperty wLockedProp, xLockedProp, yLockedProp, zLockedProp;

    private void Initialize(SerializedProperty property)
    {
        if (initialized) return;

        enforceNormalizationProp = property.FindPropertyRelative("enforceNormalisation");

        wProp = property.FindPropertyRelative("_w");
        xProp = property.FindPropertyRelative("_x");
        yProp = property.FindPropertyRelative("_y");
        zProp = property.FindPropertyRelative("_z");

        wLockedProp = wProp.FindPropertyRelative("isLocked");
        xLockedProp = xProp.FindPropertyRelative("isLocked");
        yLockedProp = yProp.FindPropertyRelative("isLocked");
        zLockedProp = zProp.FindPropertyRelative("isLocked");

        initialized = true;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Initialize(property);
        EditorGUI.BeginProperty(position, label, property);

        var rotParams = fieldInfo.GetValue(property.serializedObject.targetObject) as RotParams_Quaternion;
        if (rotParams == null)
        {
            EditorGUI.LabelField(position, "Error: Cannot cast property to RotParams_Quaternion");
            return;
        }

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float lockWidth = 18f;
        float labelWidth = 15f;
        float floatFieldWidth = position.width - lockWidth - labelWidth - 10;

        Rect currentRect = new Rect(position.x, position.y, position.width, lineHeight);

        // Draw enforceNormalization
        EditorGUI.BeginChangeCheck(); 
        bool enforceNormalizationToggle = EditorGUI.Toggle(currentRect, new GUIContent("Enforce Normalization"), enforceNormalizationProp.boolValue);
        currentRect.y += lineHeight + spacing;
        if (EditorGUI.EndChangeCheck())
        {
            rotParams.EnforceNormalisation = enforceNormalizationToggle; 
        }
        
        DrawFloatWithLock(wLockedProp, ref currentRect, "W", rotParams.W, val => rotParams.W = val);
        DrawFloatWithLock(xLockedProp, ref currentRect, "X", rotParams.X, val => rotParams.X = val);
        DrawFloatWithLock(yLockedProp, ref currentRect, "Y", rotParams.Y, val => rotParams.Y = val);
        DrawFloatWithLock(zLockedProp, ref currentRect, "Z", rotParams.Z, val => rotParams.Z = val);

        EditorGUI.EndProperty();
    }

    private void DrawFloatWithLock(SerializedProperty lockProp, ref Rect rect, string label, float currentValue, Action<float> onValueChanged)
    {
        EditorGUI.BeginChangeCheck();

        float labelWidth = 13f;
        float spacing = 2f;
        float lockWidth = 18f;

        Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
        Rect floatRect = new Rect(labelRect.xMax + spacing, rect.y, rect.width - labelWidth - lockWidth - spacing * 2, rect.height);
        Rect lockRect = new Rect(rect.xMax - lockWidth, rect.y, lockWidth, rect.height);

        // Use draggable float field with label
        EditorGUIUtility.labelWidth = labelWidth;
        float newVal = EditorGUI.FloatField(floatRect, new GUIContent(label), currentValue);

        // Lock toggle
        lockProp.boolValue = EditorGUI.Toggle(lockRect, lockProp.boolValue);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(lockProp.serializedObject.targetObject, $"Edit {label}");
            onValueChanged(newVal);
            EditorUtility.SetDirty(lockProp.serializedObject.targetObject);
        }

        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 5 lines: 1 for normalization + 4 for WXYZ
        return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5;
    }
}

}