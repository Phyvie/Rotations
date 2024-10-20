using System;
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
            positionRect = EditorGUI.PrefixLabel(positionRect, GUIUtility.GetControlID(FocusType.Passive), label); 
            
            #region angleType
            SerializedProperty angleType = property.FindPropertyRelative("_angleType");
            EditorGUI.PropertyField(positionRect, angleType, new GUIContent("AngleType")); 
            #endregion
            
            /*
            #region angle
            SerializedProperty angle = property.FindPropertyRelative("_rotationAngle");
            angle.floatValue = EditorGUI.FloatField(positionRect, "RotationAngle", angle.floatValue);
            positionRect.y += EditorGUIUtility.singleLineHeight + 2;
            #endregion
            
            #region complexNumber
            SerializedProperty complexNumber = property.FindPropertyRelative("_complexNumber");
            complexNumber.vector2Value = EditorGUI.Vector2Field(positionRect, "ComplexNumber", complexNumber.vector2Value);
            positionRect.y += EditorGUIUtility.singleLineHeight + 2;
            #endregion
            */
            
            /*
            #region Matrix
            SerializedProperty matrixProperty = property.FindPropertyRelative("_matrix");
            float[][] matrix = (float[][]) matrixProperty.boxedValue;

            for (int row = 0; row < matrix.Length; row++)
            {
                for (int column = 0; column < matrix[row].Length; column++)
                {
                    Rect matrixPositionRect = new Rect(
                        positionRect.x + row*EditorGUIUtility.singleLineHeight,
                        positionRect.y + column*EditorGUIUtility.singleLineHeight,
                        positionRect.width / 2,
                        EditorGUIUtility.singleLineHeight);
                    matrix[row][column] = EditorGUI.FloatField(
                        matrixPositionRect,
                        matrix[row][column]);
                }
            }
            positionRect.y += EditorGUIUtility.singleLineHeight * matrix.Length;
            #endregion
            */

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 2 * EditorGUIUtility.singleLineHeight + 4; 
        }
    }
}
