/*
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(ComplexRotation))]
    public class ComplexRotation_Inspector : PropertyDrawer
    {
        public override void OnGUI(Rect positionRect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(positionRect, label, property);
            EditorGUI.PrefixLabel(positionRect, GUIUtility.GetControlID(FocusType.Passive), label); 
            
            SerializedProperty angleType = property.FindPropertyRelative("_angleType");
            EditorGUILayout.PropertyField(angleType, new GUIContent("AngleType")); 
            
            SerializedProperty angle = property.FindPropertyRelative("_rotationAngle");
            angle.floatValue = EditorGUILayout.FloatField("RotationAngle", angle.floatValue);
            
            SerializedProperty complexNumber = property.FindPropertyRelative("_complexNumber");
            complexNumber.vector2Value = EditorGUILayout.Vector2Field("ComplexNumber", complexNumber.vector2Value);
            
            /*
            #region Matrix
            SerializedProperty matrixProperty = property.FindPropertyRelative("Matrix2X2");
            
            for (int row = 0; row < matrixProperty.arraySize; row++)
            {
                for (int column = 0; column < matrixProperty.GetArrayElementAtIndex(row).arraySize; column++)
                {
                    Rect matrixPositionRect = new Rect(
                        positionRect.x + row*EditorGUIUtility.singleLineHeight,
                        positionRect.y + column*EditorGUIUtility.singleLineHeight,
                        positionRect.width / 2,
                        EditorGUIUtility.singleLineHeight);
                    matrixProperty.GetArrayElementAtIndex(row).GetArrayElementAtIndex(column).floatValue 
                        = EditorGUI.FloatField(matrixPositionRect,
                        matrixProperty.GetArrayElementAtIndex(row).GetArrayElementAtIndex(column).floatValue);
                }
            }
            positionRect.y += EditorGUIUtility.singleLineHeight * matrixProperty.arraySize;
            #endregion
            *_/

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 2 * EditorGUIUtility.singleLineHeight; 
        }
    }
}
*/
