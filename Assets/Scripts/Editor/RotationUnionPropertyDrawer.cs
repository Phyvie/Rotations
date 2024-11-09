using System;
using RotationTypes;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(RotationUnion))]
    public class RotationUnionPropertyDrawer : NestedPropertyDrawer<RotationUnion>
    {
        private bool isInitialized = false; 
        
        private SerializedProperty eRotationTypeProp;

        private SerializedProperty ActiveRotationProp(SerializedProperty property)
        {
            return eRotationTypeProp.enumValueIndex switch 
            {
                (int) ERotationType.eulerAngle => property.FindPropertyRelative("eulerAngleRotation"), 
                (int) ERotationType.quaternion => property.FindPropertyRelative("quaternionRotation"), 
                _ => throw new NotImplementedException()
            }; 
        }
        
        private void Initialize(SerializedProperty property)
        {
            if (isInitialized)
            {
                return; 
            }

            eRotationTypeProp = property.FindPropertyRelative("eRotationType"); 
            
            isInitialized = true; 
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializePropertyNesting(property);
            Initialize(property);
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty ZyKaProperty = property.FindPropertyRelative("ZyKaEnum"); 
            ERotationType newRotationType =
                (ERotationType)EditorGUI.EnumPopup(position, (ERotationType)ZyKaProperty.enumValueIndex);
            if ((int)newRotationType != ZyKaProperty.enumValueIndex)
            {
                CallPropertyMethod_Copilot(property, "SetZyKaEnum", new object[]{newRotationType});
            }
            if ((int)newRotationType != ZyKaProperty.enumValueIndex) //TODO: remove this safety check
            {
                CallPropertyMethod_Copilot(property, "SetZyKaEnum", new object[]{newRotationType});
            }
            position.y += EditorGUI.GetPropertyHeight(eRotationTypeProp); 
            
            EditorGUI.PropertyField(position, ActiveRotationProp(property));
            position.y += EditorGUI.GetPropertyHeight(ActiveRotationProp(property));
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitializePropertyNesting(property);
            Initialize(property);

            float typeSelectorHeight = EditorGUI.GetPropertyHeight(eRotationTypeProp);
            float activePropertyHeight = EditorGUI.GetPropertyHeight(ActiveRotationProp(property)); 
            
            return typeSelectorHeight + activePropertyHeight;
        }
    }
}