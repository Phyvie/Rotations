using RotParams;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(RotParams_Matrix))]
    public class MatrixRotationInspector : NestedPropertyDrawer
    {
        private SerializedProperty internalMatrixProp;
        private readonly string primaryAxisIndexName = "primaryAxisIndex";
        private SerializedProperty SP_primaryAxisIndex; 
        private readonly string secondaryAxisIndexName = "secondaryAxisIndex";
        private SerializedProperty SP_secondaryAxisIndex;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializePropertyNesting(property);
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            RotParams_Matrix rotParamsMatrix = GetObject<RotParams_Matrix>(property);

            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, new GUIContent("Matrix" + (rotParamsMatrix.isRotationMatrix ? " (Rotation)" : " (NotRotation)")));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
            if (property.isExpanded)
            {
                EditorGUI.indentLevel += 2;

                internalMatrixProp = property.FindPropertyRelative("InternalMatrix");
                EditorGUI.PropertyField(position, internalMatrixProp);
                position.y += EditorGUI.GetPropertyHeight(internalMatrixProp); 
                
                EditorGUI.indentLevel -= 2;

                SP_primaryAxisIndex = property.FindPropertyRelative(primaryAxisIndexName);
                EditorGUI.PropertyField(position, SP_primaryAxisIndex); 
                position.y += EditorGUI.GetPropertyHeight(SP_primaryAxisIndex); 
                
                SP_secondaryAxisIndex = property.FindPropertyRelative(secondaryAxisIndexName);
                EditorGUI.PropertyField(position, SP_secondaryAxisIndex); 
                position.y += EditorGUI.GetPropertyHeight(SP_secondaryAxisIndex); 
            }
            EditorGUI.EndFoldoutHeaderGroup();
        
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            internalMatrixProp = property.FindPropertyRelative("InternalMatrix");
            SP_primaryAxisIndex = property.FindPropertyRelative(primaryAxisIndexName);
            SP_secondaryAxisIndex = property.FindPropertyRelative(secondaryAxisIndexName);
            
            float unexpandedHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float expandedHeight = 
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
                + EditorGUI.GetPropertyHeight(internalMatrixProp) + EditorGUIUtility.standardVerticalSpacing
                + EditorGUI.GetPropertyHeight(SP_primaryAxisIndex) + EditorGUIUtility.standardVerticalSpacing
                + EditorGUI.GetPropertyHeight(SP_secondaryAxisIndex) + EditorGUIUtility.standardVerticalSpacing;
            
            return property.isExpanded ? expandedHeight : unexpandedHeight; 
        }
    }
}