using System;
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(RotParams_EulerAngles))]
    public class EulerAngleRotationInspector : NestedPropertyDrawer
    {
        private SerializedProperty firstGimbleRing;
        private SerializedProperty secondGimbleRing;
        private SerializedProperty thirdGimbleRing;

        private bool isInitialised = false;
        
        private void Initialize(SerializedProperty property)
        {
            if (isInitialised)
            {
                // return; 
            }
            
            firstGimbleRing = property.FindPropertyRelative("firstGimbleRing");
            secondGimbleRing = property.FindPropertyRelative("secondGimbleRing"); 
            thirdGimbleRing = property.FindPropertyRelative("thirdGimbleRing"); 
            
            isInitialised = true; 
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            InitializePropertyNesting(property);
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            RotParams_EulerAngles targetRotParamsEulerAngles = GetObject<RotParams_EulerAngles>(property);
            
            EGimbleType targetGimbleType = targetRotParamsEulerAngles!.GetGimbleType();
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, new GUIContent($"eulerAngle({Enum.GetNames(typeof(EGimbleType))[(int) targetGimbleType]})"));
            EditorGUI.EndFoldoutHeaderGroup(); //??? this is confusing, because seemingly all a BeginFoldoutHeaderGroup does is return whether it's toggled on or off, but not the actual indentation; 
            if (property.isExpanded)
            {
                EditorGUI.indentLevel+=2; 
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
                
                EditorGUI.PropertyField(position, firstGimbleRing); 
                position.y += EditorGUI.GetPropertyHeight(firstGimbleRing);

                EditorGUI.PropertyField(position, secondGimbleRing); 
                position.y += EditorGUI.GetPropertyHeight(secondGimbleRing);

                EditorGUI.PropertyField(position, thirdGimbleRing); 
                position.y += EditorGUI.GetPropertyHeight(thirdGimbleRing);
                
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
                + EditorGUI.GetPropertyHeight(firstGimbleRing)
                + EditorGUI.GetPropertyHeight(secondGimbleRing)
                + EditorGUI.GetPropertyHeight(thirdGimbleRing)
                ;
            float foldedHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            
            return (property.isExpanded ? unfoldedHeight : foldedHeight); 
        }
    }
}