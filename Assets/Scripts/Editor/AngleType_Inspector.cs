using System;
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
            positionRect = EditorGUI.PrefixLabel(positionRect, GUIUtility.GetControlID(FocusType.Passive), label);

            AngleType angleType = (AngleType) fieldInfo.GetValue(property.serializedObject.targetObject); 
            int selectedIndex = Array.IndexOf(AngleType.AngleTypes, angleType);
            if (selectedIndex < 0)
            {
                throw new Exception("Couldn't find angleType in AngleType Array"); 
            }

            int newIndex = EditorGUI.Popup(positionRect, "AngleType: ", selectedIndex, AngleType.AngleTypeNames);
            if (newIndex >= 0 && newIndex != selectedIndex)
            {
                property.managedReferenceValue = AngleType.AngleTypes[newIndex]; 
            }

            positionRect.y += EditorGUIUtility.singleLineHeight + 2; 
        }
    }
}