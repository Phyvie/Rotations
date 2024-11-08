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
        private bool isInitialized = false; 
        
        private void Initialize(SerializedProperty property)
        {
            if (isInitialized)
            {
                return; 
            }
            if (PropertyAsT is null)
            {
                Debug.LogError($"GimbleRing-PropertyAsT is null, can't Initialise Property");
                return; 
            }
            
            angleTypeProp = property.FindPropertyRelative("ownAngleType");
            axisProp = property.FindPropertyRelative("eAxis");
            angleProp = property.FindPropertyRelative("angle");

            isInitialized = true; 
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializePropertyNesting(property);
            Initialize(property);
            
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            if (PropertyAsT is null)
            {
                return; 
            }
            
            if (!PropertyAsT.bInheritedAngleType)
            {
                EditorGUI.PropertyField(position, angleTypeProp);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            EditorGUI.PropertyField(position, axisProp);
            position.y += EditorGUI.GetPropertyHeight(axisProp);

            EditorGUI.PropertyField(position, angleProp);
            position.y += EditorGUI.GetPropertyHeight(angleProp);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitializePropertyNesting(property);
            Initialize(property);
            
            if (PropertyAsT is null)
            {
                Debug.LogWarning($"GimbleRing-PropertyAsT is null, can't GetPropertyHeight");
                return 0; 
            }

            float propertyHeight =
                (PropertyAsT.bInheritedAngleType ? 0 : EditorGUI.GetPropertyHeight(angleTypeProp) + EditorGUIUtility.standardVerticalSpacing) +
                EditorGUI.GetPropertyHeight(axisProp) + EditorGUIUtility.standardVerticalSpacing + 
                EditorGUI.GetPropertyHeight(angleProp);

            return propertyHeight; 
        }
    }
}
