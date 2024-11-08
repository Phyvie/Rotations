using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(GimbleRing))]
    public class GimbleRingInspector : NestedPropertyDrawer<GimbleRing>
    {
        private SerializedProperty inheritsAngleTypeProp; 
        private SerializedProperty angleTypeProp; 
        private SerializedProperty axisProp; 
        private SerializedProperty angleProp; 
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            InitializePropertyNesting(property);
            position.height = EditorGUIUtility.singleLineHeight;

            if (PropertyAsT is null)
            {
                return; 
            }
            
            if (!PropertyAsT.bInheritedAngleType)
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
            if (PropertyAsT is null)
            {
                Debug.LogWarning($"GimbleRing-PropertyAsT is null, can't GetPropertyHeight");
                return 0; 
            }
            
            angleTypeProp = property.FindPropertyRelative("ownAngleType");
            axisProp = property.FindPropertyRelative("eAxis"); 
            angleProp = property.FindPropertyRelative("angle");

            float propertyHeight =
                (PropertyAsT.bInheritedAngleType ? 0 : EditorGUI.GetPropertyHeight(angleTypeProp) + EditorGUIUtility.standardVerticalSpacing) +
                EditorGUI.GetPropertyHeight(axisProp) + EditorGUIUtility.standardVerticalSpacing + 
                EditorGUI.GetPropertyHeight(angleProp);

            return propertyHeight; 
        }
    }
}
