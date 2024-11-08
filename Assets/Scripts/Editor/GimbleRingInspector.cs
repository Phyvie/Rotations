using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(GimbleRing))]
    public class GimbleRingInspector : NestedPropertyDrawer<GimbleRing>
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
            if (PropertyAsT is null)
            {
                Debug.LogError($"GimbleRing-PropertyAsT is null, can't Initialise Property");
                return; 
            }
            
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
                EditorGUI.GetPropertyHeight(axisProp) + EditorGUIUtility.standardVerticalSpacing + 
                EditorGUI.GetPropertyHeight(angleProp);

            return propertyHeight; 
        }
    }
}
