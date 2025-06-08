using System;
using RotParams;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(RotParams_EulerAngles))]
    public class EulerAngleRotationInspector : NestedPropertyDrawer
    {
        private SerializedProperty SP_outer;
        private SerializedProperty SP_middle;
        private SerializedProperty SP_inner;

        private bool isInitialised = false;
        
        private void Initialize(SerializedProperty property)
        {
            if (isInitialised)
            {
                // return; 
            }
            
            SP_outer = property.FindPropertyRelative(nameof(RotParams_EulerAngles.outer));
            SP_middle = property.FindPropertyRelative(nameof(RotParams_EulerAngles.middle)); 
            SP_inner = property.FindPropertyRelative(nameof(RotParams_EulerAngles.inner)); 
            
            isInitialised = true; 
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            InitializePropertyNesting(property);
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            RotParams_EulerAngles targetRotParamsEulerAngles = GetObject<RotParams_EulerAngles>(property);
            
            EGimbalType targetGimbalType = targetRotParamsEulerAngles!.GetGimbalType();
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, new GUIContent($"eulerAngle({Enum.GetNames(typeof(EGimbalType))[(int) targetGimbalType]})"));
            EditorGUI.EndFoldoutHeaderGroup(); //??? this is confusing, because seemingly all a BeginFoldoutHeaderGroup does is return whether it's toggled on or off, but not the actual indentation; 
            if (property.isExpanded)
            {
                EditorGUI.indentLevel+=2; 
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
                
                EditorGUI.PropertyField(position, SP_outer); 
                position.y += EditorGUI.GetPropertyHeight(SP_outer);

                EditorGUI.PropertyField(position, SP_middle); 
                position.y += EditorGUI.GetPropertyHeight(SP_middle);

                EditorGUI.PropertyField(position, SP_inner); 
                position.y += EditorGUI.GetPropertyHeight(SP_inner);
                
                EditorGUI.indentLevel-=2;
            }
            
            EditorGUI.EndProperty(); 
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            InitializePropertyNesting(property);
            
            float unfoldedHeight =
                (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2
                + EditorGUI.GetPropertyHeight(SP_outer)
                + EditorGUI.GetPropertyHeight(SP_middle)
                + EditorGUI.GetPropertyHeight(SP_inner)
                ;
            float foldedHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            
            return (property.isExpanded ? unfoldedHeight : foldedHeight); 
        }
    }
}