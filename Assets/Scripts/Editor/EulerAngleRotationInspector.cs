using System;
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(EulerAngleRotation))]
    public class EulerAngleRotationInspector : NestedPropertyDrawer
    {
        private SerializedProperty gimbleProperty;
        private SerializedProperty angleTypeProperty;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            
            InitializePropertyNesting(property);
            EulerAngleRotation targetEulerAngle = GetPropertyAsT<EulerAngleRotation>();
            if (targetEulerAngle is null)
            {
                Debug.LogWarning("EulerAngleInspector: targetEulerAngle is null; this should only happen during initialisation or array-size change");
                return; 
            }
            
            EGimbleType targetGimbleType = targetEulerAngle!.GetGimbleType();
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, new GUIContent($"eulerAngle({Enum.GetNames(typeof(EGimbleType))[(int) targetGimbleType]})"));
            EditorGUI.EndFoldoutHeaderGroup(); //??? this is confusing, because seemingly all a BeginFoldoutHeaderGroup does is return whether it's toggled on or off, but not the actual indentation; 
            if (property.isExpanded)
            {
                EditorGUI.indentLevel+=2; 
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
                
                SerializedProperty isIntrinsicProperty = property.FindPropertyRelative("isIntrinsic");
                isIntrinsicProperty.boolValue = EditorGUI.Toggle(position, "intrinsic", isIntrinsicProperty.boolValue);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                angleTypeProperty = property.FindPropertyRelative("_angleType");
                EditorGUI.PropertyField(position, angleTypeProperty); 
                position.y += EditorGUI.GetPropertyHeight(angleTypeProperty);
                
                gimbleProperty = property.FindPropertyRelative("gimble");
                EditorGUI.PropertyField(position, gimbleProperty); 
                position.y += EditorGUI.GetPropertyHeight(gimbleProperty) + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.indentLevel-=2; 
            }
            
            EditorGUI.EndProperty(); 
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            angleTypeProperty = property.FindPropertyRelative("_angleType");
            gimbleProperty = property.FindPropertyRelative("gimble");
            float unfoldedHeight =
                (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2
                + EditorGUI.GetPropertyHeight(angleTypeProperty) 
                + EditorGUI.GetPropertyHeight(gimbleProperty);
            float foldedHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            
            return (property.isExpanded ? unfoldedHeight : foldedHeight); 
        }
    }
}