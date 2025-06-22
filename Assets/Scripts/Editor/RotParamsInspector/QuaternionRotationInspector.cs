using RotParams;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RotParams_Quaternion))]
public class QuaternionRotationInspector : PropertyDrawer
{
    private const float LockToggleWidth = 18f;
    private const float LabelWidth = 20f;
    private const float Spacing = 4f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect originalRect = position;
        position.height = EditorGUIUtility.singleLineHeight;

        // Get target object
        var target = fieldInfo.GetValue(property.serializedObject.targetObject) as RotParams_Quaternion;

        if (target == null)
        {
            EditorGUI.LabelField(position, "RotParams_Quaternion is null");
            return;
        }

        DrawComponentWithLock(ref position, "W", target.W, target.WLocked, 
            newVal => target.W = newVal, 
            newLock => target.WLocked = newLock);

        DrawComponentWithLock(ref position, "X", target.X, target.XLocked, 
            newVal => target.X = newVal, 
            newLock => target.XLocked = newLock);

        DrawComponentWithLock(ref position, "Y", target.Y, target.YLocked, 
            newVal => target.Y = newVal, 
            newLock => target.YLocked = newLock);

        DrawComponentWithLock(ref position, "Z", target.Z, target.ZLocked, 
            newVal => target.Z = newVal, 
            newLock => target.ZLocked = newLock);

        // Enforce Normalisation toggle
        position.y += EditorGUIUtility.singleLineHeight + Spacing;
        target.EnforceNormalisation = EditorGUI.ToggleLeft(position, "Enforce Normalisation", target.EnforceNormalisation);

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
        // 4 rows for W, X, Y, Z + 1 for EnforceNormalisation
        return (EditorGUIUtility.singleLineHeight + Spacing) * 6;
    }
}
