using System;
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(EulerAngleRotation))]
    public class EulerAngleRotationInspector : NestedPropertyDrawer<EulerAngleRotation>
    {
        private SerializedProperty firstGimbleRing;
        private SerializedProperty secondGimbleRing;
        private SerializedProperty thirdGimbleRing;
        private SerializedProperty angleTypeProperty;
        private SerializedProperty isIntrinsicProperty; 

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
            angleTypeProperty = property.FindPropertyRelative("angleType"); 
            isIntrinsicProperty = property.FindPropertyRelative("isIntrinsic");
            
            isInitialised = true; 
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            InitializePropertyNesting(property);
            
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            
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
                
                isIntrinsicProperty.boolValue = EditorGUI.Toggle(position, "intrinsic", isIntrinsicProperty.boolValue);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.PropertyField(position, angleTypeProperty); 
                position.y += EditorGUI.GetPropertyHeight(angleTypeProperty);
                
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
                + EditorGUI.GetPropertyHeight(angleTypeProperty) 
                + EditorGUI.GetPropertyHeight(firstGimbleRing)
                + EditorGUI.GetPropertyHeight(secondGimbleRing)
                + EditorGUI.GetPropertyHeight(thirdGimbleRing)
                ;
            float foldedHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            
            return (property.isExpanded ? unfoldedHeight : foldedHeight); 
        }
    }
}