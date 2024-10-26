using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(RotationTypes.SingleGimbleRotation))]
    public class SingleGimbleRotationInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            SerializedProperty hasOwnAngleTypeProperty = property.FindPropertyRelative("hasOwnAngleType");
            Debug.Log("hasOwnAngleTypeProperty: " + hasOwnAngleTypeProperty.boolValue);
            if (hasOwnAngleTypeProperty.boolValue)
            {
                SerializedProperty angleTypeProperty = property.FindPropertyRelative("ownAngleType");
                EditorGUILayout.PropertyField(angleTypeProperty); 
            }
            
            EditorGUILayout.BeginVertical();
            SerializedProperty axisProperty = property.FindPropertyRelative("eAxis"); 
            EditorGUILayout.PropertyField(axisProperty);
            
            SerializedProperty angle = property.FindPropertyRelative("angle");
            EditorGUILayout.PropertyField(angle);
            EditorGUILayout.EndVertical();
            
            EditorGUI.EndProperty();
        }
    }
}
