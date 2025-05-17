using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(RotParams_Matrix))]
    public class MatrixRotationInspector : NestedPropertyDrawer
    {
        private SerializedProperty internalMatrixProp; 
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight; 

            RotParams_Matrix rotParamsMatrix = objectHierarchy[~1] as RotParams_Matrix; 
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, new GUIContent("Matrix" + (rotParamsMatrix.isRotationMatrix ? " (Rotation)" : " (NotRotation)")));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            EditorGUI.EndFoldoutHeaderGroup();
            if (property.isExpanded)
            {
                EditorGUI.indentLevel += 2;

                internalMatrixProp = property.FindPropertyRelative("InternalMatrix");
                EditorGUI.PropertyField(position, internalMatrixProp);
                position.y += EditorGUI.GetPropertyHeight(internalMatrixProp); 
                
                EditorGUI.indentLevel -= 2; 
            }
        
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            internalMatrixProp = property.FindPropertyRelative("InternalMatrix");
            float internalMatrixPropHeight = EditorGUI.GetPropertyHeight(internalMatrixProp); 
            
            float unexpandedHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float expandedHeight = 
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
                + internalMatrixPropHeight; 
            
            return property.isExpanded ? expandedHeight : unexpandedHeight; 
        }
    }
}