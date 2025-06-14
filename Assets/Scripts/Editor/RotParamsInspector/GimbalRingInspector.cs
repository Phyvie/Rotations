using Editor;
using RotParams;
using UnityEditor;
using UnityEngine;

namespace RotationTypes
{
    [CustomPropertyDrawer(typeof(_RotParams_EulerAngleGimbalRing))]
    public class GimbalRingInspector : NestedPropertyDrawer
    {
        private SerializedProperty axisProp; 
        private SerializedProperty angleProp;
        private bool isInitialized = false; 
        
        private void Initialize(SerializedProperty property)
        {
            if (isInitialized)
            {
                return; 
            }
            axisProp = property.FindPropertyRelative("eAxis");
            angleProp = property.FindPropertyRelative(_RotParams_EulerAngleGimbalRing.NameOfAngle);

            isInitialized = true; 
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            
            EditorGUI.PropertyField(position, axisProp);
            position.y += EditorGUI.GetPropertyHeight(axisProp);

            EditorGUI.PropertyField(position, angleProp);
            position.y += EditorGUI.GetPropertyHeight(angleProp);
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            
            float propertyHeight =
                EditorGUI.GetPropertyHeight(axisProp) + 
                EditorGUIUtility.standardVerticalSpacing + 
                EditorGUI.GetPropertyHeight(angleProp);

            return propertyHeight; 
        }
    }
}
