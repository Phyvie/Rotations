using System;
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(RotationTypes.EulerAngleRotation))]
    public class EulerAngleRotationInspector : NestedPropertyDrawer
    {
        private SerializedProperty gimbleProperty;
        private SerializedProperty angleTypeProperty;
        private bool isFoldout; 
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            
            InitializePropertyNesting(property);
            EulerAngleRotation targetEulerAngle = GetPropertyAsT<EulerAngleRotation>();

            /*
            EditorGUI.BeginFoldoutHeaderGroup(position, isFoldout, new GUIContent("EulerAngleRotation")); 
            if (isFoldout)
            {
                
            }
            EditorGUI.EndFoldoutHeaderGroup();
            */
            
            EGimbleType targetGimbleType = targetEulerAngle!.GetGimbleType();
            GUIStyle boldStyle = new GUIStyle(EditorStyles.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14; 
            EditorGUI.LabelField(position, Enum.GetNames(typeof(EGimbleType))[(int) targetGimbleType], boldStyle); //TODO: make this text bigger and fat and red
            position.y += EditorGUIUtility.singleLineHeight +  EditorGUIUtility.standardVerticalSpacing; 
            
            SerializedProperty isIntrinsicProperty = property.FindPropertyRelative("isIntrinsic");
            isIntrinsicProperty.boolValue = EditorGUI.Toggle(position, "intrinsic", isIntrinsicProperty.boolValue);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            angleTypeProperty = property.FindPropertyRelative("_angleType");
            EditorGUI.PropertyField(position, angleTypeProperty); 
            position.y += EditorGUI.GetPropertyHeight(angleTypeProperty); 
            
            gimbleProperty = property.FindPropertyRelative("gimble");
            EditorGUI.PropertyField(position, gimbleProperty); 
            position.y += EditorGUI.GetPropertyHeight(gimbleProperty) + EditorGUIUtility.standardVerticalSpacing; 
            
            /*
            for (int i = 0; i < gimbleProperty.arraySize; i++)
            {
                SerializedProperty elementProperty = gimbleProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(position, elementProperty, GUIContent.none);
            }
            */
            
            /*
            if (GUI.Button(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), targetEulerAngle.GetAddButtonName()))
            {
                gimbleProperty.arraySize++;
                //TODO: EditorGUI.AddButton or whatever it is called
                position.y += EditorGUIUtility.singleLineHeight;
            }
            */
            
            /*
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
            angleTypeProperty = property.FindPropertyRelative("_angleType");
            gimbleProperty = property.FindPropertyRelative("gimble");
            float singlePropertyHeight =
                (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2
                + EditorGUI.GetPropertyHeight(angleTypeProperty) 
                + EditorGUI.GetPropertyHeight(gimbleProperty);
            if (property.isArray)
            {
                return property.arraySize * singlePropertyHeight; 
            }
            else
            {
                return singlePropertyHeight; 
            }
        }
    }
}