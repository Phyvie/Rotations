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

            EulerAngleRotation targetProperty =
                fieldInfo.GetValue(property.serializedObject.targetObject) as EulerAngleRotation;
            EGimbleType targetGimbleType = targetProperty!.GetGimbleType(); 
            EditorGUI.LabelField(position, Enum.GetNames(typeof(EGimbleType))[(int) targetGimbleType]);
            position.y += 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            
            SerializedProperty isIntrinsicProperty = property.FindPropertyRelative("isIntrinsic");
            isIntrinsicProperty.boolValue = EditorGUI.Toggle(position, "intrinsic", isIntrinsicProperty.boolValue);
            // position.y += EditorGUIUtility.singleLineHeight + 2; 
            /*
            SerializedProperty gimbleProperty = property.FindPropertyRelative("gimble");
            for (int i = 0; i < gimbleProperty.arraySize; i++)
            {
                SerializedProperty elementProperty = gimbleProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(position, elementProperty, GUIContent.none);
                position.y += EditorGUIUtility.singleLineHeight;
            }

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

            EditorGUI.EndProperty(); 
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty listProperty = property.FindPropertyRelative("gimble");
            // return EditorGUIUtility.singleLineHeight * (listProperty.arraySize + 5); 
            return (EditorGUIUtility.singleLineHeight+EditorGUIUtility.standardVerticalSpacing) * 3; 
        }
    }
}