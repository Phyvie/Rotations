using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(GimbleRingNew))]
    public class GimbleRingNewInspector : NestedPropertyDrawer<GimbleRingNew>
    {
        private bool isInitialized = false; 
        
        private SerializedProperty angleTypeProp; 
        
        private void Initialize(SerializedProperty property)
        {
            if (isInitialized)
            {
                return; 
            }
            if (PropertyAsT is null)
            {
                Debug.LogError($"PropertyAsT<GimbleRingNew> is null, can't Initialise Property");
                return; 
            }

            angleTypeProp = property.FindPropertyRelative("angleType"); 
            
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

            if (!PropertyAsT.bInheritsAngleType)
            {
                EditorGUI.PropertyField(position, angleTypeProp);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitializePropertyNesting(property);
            Initialize(property);
            
            if (PropertyAsT is null)
            {
                Debug.LogWarning($"PropertyAsT<GimbleRingNew> is null, can't GetPropertyHeight");
                return 0; 
            }

            float propertyHeight =
                (PropertyAsT.bInheritsAngleType
                    ? 0
                    : EditorGUI.GetPropertyHeight(angleTypeProp) + EditorGUIUtility.standardVerticalSpacing) 
                ; 

            return propertyHeight; 
        }
    }
}
