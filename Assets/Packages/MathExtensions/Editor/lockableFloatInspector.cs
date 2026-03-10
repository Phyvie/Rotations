using System.Globalization;
using Extensions.MathExtensions;
using RotParams;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(LockableFloat))]
    public class LockableFloatDrawer : PropertyDrawer
    {
        private SerializedProperty typeValueProp;
        private SerializedProperty isLockedProp;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            typeValueProp = property.FindPropertyRelative("typeValue");
            isLockedProp = property.FindPropertyRelative("isLocked");
         
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float lockWidth = 20f;
            float spacing = 4f;

            // Layout calculation
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, lineHeight);
            Rect fieldRect = new Rect(labelRect.xMax, position.y, position.width - labelRect.width - lockWidth - spacing, lineHeight);
            Rect lockRect = new Rect(fieldRect.xMax + spacing, position.y, lockWidth, lineHeight);

            // Draw label
            EditorGUI.LabelField(labelRect, label);

            // Draw value (disabled if locked)
            EditorGUI.BeginDisabledGroup(isLockedProp.boolValue);
            EditorGUI.PropertyField(fieldRect, typeValueProp, GUIContent.none);
            EditorGUI.EndDisabledGroup();

            // Draw lock toggle as icon
            isLockedProp.boolValue = GUI.Toggle(
                lockRect,
                isLockedProp.boolValue,
                EditorGUIUtility.IconContent("LockIcon-On", "Toggle Lock"),
                GUIStyle.none
            );

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}