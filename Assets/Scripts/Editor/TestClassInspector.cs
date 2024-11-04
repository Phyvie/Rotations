using System;
using System.Reflection;
using System.Text;
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(TestClass))]
    public class TestClassInspector : NestedPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight; 
            
            SerializedProperty testAngleProp = property.FindPropertyRelative("testClassAngle");
            EditorGUI.PropertyField(position, testAngleProp);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            
            SerializedProperty testInt = property.FindPropertyRelative("testClassInt");
            EditorGUI.PropertyField(position, testInt);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            
            SerializedProperty testAngleTypeArray = property.FindPropertyRelative("testClassAngleTypeArray");
            EditorGUI.PropertyField(position, testAngleTypeArray);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty array = property.FindPropertyRelative("testClassAngleTypeArray"); 
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2 - EditorGUIUtility.standardVerticalSpacing + 
                   EditorGUI.GetPropertyHeight(array); 
        }
    }
}
