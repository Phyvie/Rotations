using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class MatrixRotationInspector
    {
        [CustomPropertyDrawer(typeof(MatrixRotation))]
        public class MatrixInspector : NestedPropertyDrawer
        {
            private SerializedProperty internalMatrixProp; 
            
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);
                position.height = EditorGUIUtility.singleLineHeight; 
                
                InitializePropertyNesting(property);

                MatrixRotation matrixRotation = GetPropertyAsT<MatrixRotation>(); 
                property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, new GUIContent("Matrix" + (matrixRotation.isRotationMatrix ? " (Rotation)" : " (NotRotation)")));
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
}