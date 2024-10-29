using System;
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(RotationTypes.EulerAngleRotation))]
    public class EulerAngleRotationInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight; 
            
            EulerAngleRotation targetProperty = fieldInfo.GetValue(property.serializedObject.targetObject) as EulerAngleRotation;

            EGimbleType targetGimbleType = targetProperty!.GetGimbleType();
            GUIStyle boldStyle = new GUIStyle(EditorStyles.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14; 
            EditorGUI.LabelField(position, Enum.GetNames(typeof(EGimbleType))[(int) targetGimbleType], boldStyle); //TODO: make this text bigger and fat and red
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            
            SerializedProperty isIntrinsicProperty = property.FindPropertyRelative("isIntrinsic");
            isIntrinsicProperty.boolValue = EditorGUI.Toggle(position, "intrinsic", isIntrinsicProperty.boolValue);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty angleTypeProperty = property.FindPropertyRelative("_angleType");
            EditorGUI.PropertyField(position, angleTypeProperty); 
            position.y += EditorGUI.GetPropertyHeight(angleTypeProperty); 
            
            SerializedProperty gimbleProperty = property.FindPropertyRelative("gimble");
            /*
            for (int i = 0; i < gimbleProperty.arraySize; i++)
            {
                SerializedProperty elementProperty = gimbleProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(position, elementProperty, GUIContent.none);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            */
            
            /*
            if (GUI.Button(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), targetProperty.GetAddButtonName()))
            {
                gimbleProperty.arraySize++;
                position.y += EditorGUIUtility.singleLineHeight;
            }

            position.y += EditorGUIUtility.singleLineHeight;

            if (gimbleProperty.arraySize > 0 && GUI.Button(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), targetProperty.GetRemoveButtonName()))
            {
                gimbleProperty.arraySize--;
                position.y += EditorGUIUtility.singleLineHeight;
            }
            */

            SerializedProperty ZyKaProperty = property.FindPropertyRelative("ZyKa");
            for (int i = 0; i < ZyKaProperty.arraySize; i++)
            {
                SerializedProperty arrayElementProperty = ZyKaProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(position, arrayElementProperty, GUIContent.none);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            
            EditorGUI.EndProperty(); 
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // SerializedProperty listProperty = property.FindPropertyRelative("gimble");
            // return EditorGUIUtility.singleLineHeight * (listProperty.arraySize + 5); 
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3; 
        }
    }
}