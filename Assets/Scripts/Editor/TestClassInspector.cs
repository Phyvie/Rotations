using System;
using System.Linq;
using System.Reflection;
using System.Text;
using RotationTypes;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(TestClass))]
    public class TestClassInspector : NestedPropertyDrawer<TestClass>
    {
        private SerializedProperty testGimbleRotationArrayProp;
        private SerializedProperty testIntProp; 
        private SerializedProperty testFloatProp; 
        private SerializedProperty testStringProp; 
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, new GUIContent("TestClassObject"));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
            
                testIntProp = property.FindPropertyRelative("testInt");
                EditorGUI.PropertyField(position, testIntProp);
                position.y += EditorGUI.GetPropertyHeight(testIntProp);
            
                testFloatProp = property.FindPropertyRelative("testFloat");
                EditorGUI.PropertyField(position, testFloatProp);
                position.y += EditorGUI.GetPropertyHeight(testFloatProp);
                
                testStringProp = property.FindPropertyRelative("testString");
                EditorGUI.PropertyField(position, testStringProp);
                position.y += EditorGUI.GetPropertyHeight(testStringProp);
                
                EditorGUI.indentLevel--; 
            } 
            EditorGUI.EndFoldoutHeaderGroup();
            
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        { 
            float testIntPropHeight = EditorGUI.GetPropertyHeight(testIntProp = property.FindPropertyRelative("testInt"));
            float testFloatPropHeight = EditorGUI.GetPropertyHeight(testFloatProp = property.FindPropertyRelative("testFloat"));
            float testStringPropHeight = EditorGUI.GetPropertyHeight(testStringProp = property.FindPropertyRelative("testString"));
            // testGimbleRotationArrayProp = property.FindPropertyRelative("testGimbleRotationArray");

            float foldedPropHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float unfoldPropertyHeight = 
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 
                testIntPropHeight + 
                testFloatPropHeight + 
                testStringPropHeight; 
            
            return property.isExpanded ? unfoldPropertyHeight : foldedPropHeight; 
        }
    }
}
