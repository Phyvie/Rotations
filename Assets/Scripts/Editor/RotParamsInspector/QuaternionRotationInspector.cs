using System.Collections.Generic;
using RotParams;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RotParams_Quaternion))]
public class QuaternionRotationInspector : PropertyDrawer
{
    private const float LockToggleWidth = 18f;
    private const float LabelWidth = 20f;
    private const float Spacing = 4f;

    // Foldout state per-property
    private static readonly Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string propertyKey = property.propertyPath;

        if (!foldoutStates.ContainsKey(propertyKey))
            foldoutStates[propertyKey] = false;

        foldoutStates[propertyKey] = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            foldoutStates[propertyKey], label, true);

        if (!foldoutStates[propertyKey])
            return;

        EditorGUI.BeginProperty(position, label, property);

        Rect fieldPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

        // Get target object
        var target = fieldInfo.GetValue(property.serializedObject.targetObject) as RotParams_Quaternion;

        if (target == null)
        {
            EditorGUI.LabelField(fieldPosition, "RotParams_Quaternion is null");
            EditorGUI.EndProperty();
            return;
        }

        DrawComponentWithLock(ref fieldPosition, "W", target.W, target.WLocked,
            newVal => target.W = newVal,
            newLock => target.WLocked = newLock);

        DrawComponentWithLock(ref fieldPosition, "X", target.X, target.XLocked,
            newVal => target.X = newVal,
            newLock => target.XLocked = newLock);

        DrawComponentWithLock(ref fieldPosition, "Y", target.Y, target.YLocked,
            newVal => target.Y = newVal,
            newLock => target.YLocked = newLock);

        DrawComponentWithLock(ref fieldPosition, "Z", target.Z, target.ZLocked,
            newVal => target.Z = newVal,
            newLock => target.ZLocked = newLock);

        fieldPosition.y += EditorGUIUtility.singleLineHeight + Spacing;
        target.EnforceNormalisation = EditorGUI.ToggleLeft(fieldPosition, "Enforce Normalisation", target.EnforceNormalisation);

        EditorGUI.EndProperty();
    }

    private void DrawComponentWithLock(
        ref Rect position,
        string label,
        float value,
        bool isLocked,
        System.Action<float> setValue,
        System.Action<bool> setLock)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        Rect labelRect = new Rect(position.x, position.y, LabelWidth, lineHeight);
        Rect floatFieldRect = new Rect(position.x + LabelWidth + Spacing, position.y,
            position.width - LabelWidth - LockToggleWidth - 3 * Spacing, lineHeight);
        Rect lockToggleRect = new Rect(position.x + position.width - LockToggleWidth, position.y,
            LockToggleWidth, lineHeight);

        EditorGUI.LabelField(labelRect, label);
        float newValue = EditorGUI.FloatField(floatFieldRect, value);
        if (newValue != value)
        {
            setValue(newValue);
        }

        bool newLock = EditorGUI.Toggle(lockToggleRect, isLocked);
        if (newLock != isLocked)
        {
            setLock(newLock);
        }

        position.y += lineHeight + Spacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        string propertyKey = property.propertyPath;
        bool isExpanded = foldoutStates.TryGetValue(propertyKey, out bool expanded) && expanded;

        return isExpanded
            ? (EditorGUIUtility.singleLineHeight + Spacing) * 6 + EditorGUIUtility.singleLineHeight + Spacing
            : EditorGUIUtility.singleLineHeight + Spacing;
    }
}
