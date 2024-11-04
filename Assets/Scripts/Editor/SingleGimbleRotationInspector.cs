using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(SingleGimbleRotation))]
    public class SingleGimbleRotationInspector : PropertyDrawer
    {
        private SerializedProperty inheritsAngleTypeProp; 
        private SerializedProperty angleTypeProp; 
        private SerializedProperty axisProp; 
        private SerializedProperty angleProp; 
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight; 
            
            inheritsAngleTypeProp = property.FindPropertyRelative("inheritAngleTypeFromOwner");
            Debug.Log("hasOwnAngleTypeProperty: " + inheritsAngleTypeProp.boolValue);
            if (!inheritsAngleTypeProp.boolValue)
            {
                angleTypeProp = property.FindPropertyRelative("ownAngleType");
                EditorGUI.PropertyField(position, angleTypeProp);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            }
            
            axisProp = property.FindPropertyRelative("eAxis"); 
            EditorGUI.PropertyField(position, axisProp);
            position.y += EditorGUI.GetPropertyHeight(axisProp); 
            
            angleProp = property.FindPropertyRelative("angle");
            EditorGUI.PropertyField(position, angleProp);
            position.y += EditorGUI.GetPropertyHeight(angleProp); 
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            inheritsAngleTypeProp = property.FindPropertyRelative("inheritAngleTypeFromOwner");
            angleTypeProp = property.FindPropertyRelative("ownAngleType");
            axisProp = property.FindPropertyRelative("eAxis"); 
            angleProp = property.FindPropertyRelative("angle");

            float singlePropHeight =
                (inheritsAngleTypeProp.boolValue ? EditorGUI.GetPropertyHeight(angleTypeProp) : 0) +
                EditorGUI.GetPropertyHeight(axisProp) +
                EditorGUI.GetPropertyHeight(angleProp);

            return singlePropHeight; 
        }
    }
}
