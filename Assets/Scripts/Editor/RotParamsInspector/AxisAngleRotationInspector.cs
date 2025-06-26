using RotParams;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RotParams_AxisAngle))]
public class RotParamsAxisAngleInspector : PropertyDrawer
{
    private const float LockToggleWidth = 18f;
    private const float LabelWidth = 40f;
    private const float Spacing = 4f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded,
            label,
            true // toggle when label is clicked
        );

        // Start drawing below the foldout line
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            var target = fieldInfo.GetValue(property.serializedObject.targetObject) as RotParams_AxisAngle;

            if (target == null)
            {
                EditorGUI.LabelField(position, "RotParams_AxisAngle is null");
                EditorGUI.indentLevel--;
                EditorGUI.EndProperty();
                return;
            }

            // Draw serialized 'typedAngle' field
            SerializedProperty angleProperty = property.FindPropertyRelative("typedAngle");
            if (angleProperty != null)
            {
                EditorGUI.PropertyField(position, angleProperty, true);
                position.y += EditorGUI.GetPropertyHeight(angleProperty, true) + Spacing;
            }

            // AxisX + lock
            DrawComponentWithLock(ref position, "X", target.AxisX, target.XLocked,
                newVal => target.AxisX = newVal,
                newLock => target.XLocked = newLock);

            // AxisY + lock
            DrawComponentWithLock(ref position, "Y", target.AxisY, target.YLocked,
                newVal => target.AxisY = newVal,
                newLock => target.YLocked = newLock);

            // AxisZ + lock
            DrawComponentWithLock(ref position, "Z", target.AxisZ, target.ZLocked,
                newVal => target.AxisZ = newVal,
                newLock => target.ZLocked = newLock);

            EditorGUI.indentLevel--;
        }

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
        Rect floatRect = new Rect(position.x + LabelWidth + Spacing, position.y,
            position.width - LabelWidth - LockToggleWidth - 3 * Spacing, lineHeight);
        Rect lockRect = new Rect(position.x + position.width - LockToggleWidth, position.y,
            LockToggleWidth, lineHeight);

        EditorGUI.LabelField(labelRect, label);
        float newValue = EditorGUI.FloatField(floatRect, value);
        if (newValue != value)
            setValue(newValue);

        bool newLock = EditorGUI.Toggle(lockRect, isLocked);
        if (newLock != isLocked)
            setLock(newLock);

        position.y += lineHeight + Spacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight; // Always show foldout line

        if (property.isExpanded)
        {
            SerializedProperty angleProperty = property.FindPropertyRelative("typedAngle");
            float angleHeight = angleProperty != null ? EditorGUI.GetPropertyHeight(angleProperty, true) + Spacing : 0f;

            float linesHeight = 3 * (EditorGUIUtility.singleLineHeight + Spacing); // For X, Y, Z

            height += angleHeight + linesHeight;
        }

        return height;
    }

}
