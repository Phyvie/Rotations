using RotParams;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AngleWithType))]
    public class AngleWithTypeDrawer : PropertyDrawer
    {
        private SerializedProperty showAllAngleTypesProp;
        private SerializedProperty showAngleTypeSelectorProp;
        private SerializedProperty angleTypeProp;
        private SerializedProperty angleInRadianProp;

        private bool initialized = false;

        private void Initialize(SerializedProperty property)
        {
            if (initialized) return;

            showAllAngleTypesProp = property.FindPropertyRelative("showAllAngleTypes");
            showAngleTypeSelectorProp = property.FindPropertyRelative("showAngleTypeSelector");
            angleTypeProp = property.FindPropertyRelative("angleType");
            angleInRadianProp = property.FindPropertyRelative("_angleInRadian");

            initialized = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 4f;

            // Split layout
            float labelWidth = EditorGUIUtility.labelWidth;
            float fieldWidth = (position.width - labelWidth - spacing) * 0.6f;
            float dropdownWidth = (position.width - labelWidth - spacing) * 0.4f;

            Rect labelRect = new Rect(position.x, position.y, labelWidth, lineHeight);
            Rect fieldRect = new Rect(labelRect.xMax, position.y, fieldWidth, lineHeight);
            Rect dropdownRect = new Rect(fieldRect.xMax + spacing, position.y, dropdownWidth - spacing, lineHeight);

            // Draw variable label (e.g., "angleWithType")
            EditorGUI.LabelField(labelRect, label);

            // Get current AngleType
            AngleType currentType = GetCurrentAngleType();
            float currentAngle = (float)(angleInRadianProp.floatValue * currentType.UnitMultiplier / (2 * Mathf.PI));

            // Float Field (no label)
            float newAngle = EditorGUI.FloatField(fieldRect, GUIContent.none, currentAngle);
            if (!Mathf.Approximately(newAngle, currentAngle))
            {
                angleInRadianProp.floatValue = (float)(newAngle / currentType.UnitMultiplier * 2 * Mathf.PI);
            }

            // Dropdown: show if enabled
            if (showAngleTypeSelectorProp.boolValue)
            {
                string[] dropdownLabels = System.Array.ConvertAll(AngleType.AngleTypes,
                    t => $"{t.AngleTypeName} ({t.UnitLabel})");

                int currentIndex = System.Array.IndexOf(AngleType.AngleTypes, currentType);
                int selectedIndex = EditorGUI.Popup(dropdownRect, currentIndex, dropdownLabels);

                if (selectedIndex != currentIndex)
                {
                    angleTypeProp.managedReferenceValue = AngleType.AngleTypes[selectedIndex];
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private AngleType GetCurrentAngleType()
        {
            object managedRef = angleTypeProp.managedReferenceValue;
            return managedRef as AngleType ?? AngleType.Radian;
        }
    }
}
