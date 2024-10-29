using System;
using System.Reflection;
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AngleType))]
    public class AngleTypeInspector : NestedPropertyDrawer
    {
        public override void OnGUI(Rect positionRect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(positionRect, label, property);
            
            InitializePropertyNesting(property);
            FieldInfo parentFieldInfo = propertyFieldInfoHierarchy[^1];
            object parentObject = propertyObjectHierarchy[^2]; 
            AngleType fieldValue = (AngleType)parentFieldInfo.GetValue(parentObject); //TODO! this actually doesn't work for lists
            
            int currentIndex = 0;
            foreach (AngleType angleType in AngleType.AngleTypes)
            {
                if (fieldValue == angleType)
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
                fieldInfo.SetValue(parentObject, AngleType.AngleTypes[newIndex]); 
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
        }
    }
}