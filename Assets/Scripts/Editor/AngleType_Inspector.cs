using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AngleType))]
    public class AngleType_Inspector : PropertyDrawer
    {
        public override void OnGUI(Rect positionRect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(positionRect, label, property);
            
            //positionRect = EditorGUI.PrefixLabel(positionRect, GUIUtility.GetControlID(FocusType.Passive), label);
            
            //TODO: figure out whether there is some cleaner solution for this which actually sets the reference to the static AngleType-object
            /*Sadly none of these following methods worked to get the property/field due to it being nested
             * 
             * AngleType angleType = (AngleType) property.objectReferenceValue;
             *
             * var angleTypeValue = fieldInfo.GetValue(property.serializedObject.targetObject) as AngleType;
             *
             * var targetObject = property.serializedObject.targetObject; 
             * Type propertyHolderType = targetObject.GetType();
             * var field = propertyHolderType.GetField(property.propertyPath);
             * AngleType angleTypeValue = field.GetValue(targetObject) as AngleType; 
            */

            SerializedProperty fullCircleUnitsProperty = property.FindPropertyRelative("fullCircleUnits");
            
            int currentIndex = 0;
            foreach (AngleType angleType in AngleType.AngleTypes)
            {
                if (angleType.fullCircleUnits == fullCircleUnitsProperty.doubleValue)
                {
                    break; 
                }
                currentIndex++; 
            }
            
            if (currentIndex > AngleType.AngleTypes.Length)
            {
                currentIndex = 0;
                Debug.LogWarning("Couldn't find angleType in AngleType.AngleTypes");
            }

            int newIndex = EditorGUI.Popup(positionRect, currentIndex, AngleType.AngleTypeNames);
            if (newIndex >= 0 && newIndex != currentIndex)
            {
                fullCircleUnitsProperty.doubleValue = AngleType.AngleTypes[newIndex].fullCircleUnits; 
            }

            positionRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 2; 
        }
    }
}