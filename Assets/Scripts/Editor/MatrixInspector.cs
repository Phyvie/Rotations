using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(RotationTypes.Matrix))]
    public class MatrixInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Find the matrix field
            SerializedProperty arrayProperty = property.FindPropertyRelative("_propertyDrawerMatrix");
            SerializedProperty heightProperty = property.FindPropertyRelative("height");
            SerializedProperty widthProperty = property.FindPropertyRelative("width");

            // Display the label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int height = heightProperty.intValue;
            int width = widthProperty.intValue;
            float fieldWidth = position.width / width;

            position.height = EditorGUIUtility.singleLineHeight;

            // Draw the matrix elements
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Rect fieldPosition = new Rect(position.x + col * fieldWidth, position.y + row * position.height, fieldWidth, position.height);
                    arrayProperty.GetArrayElementAtIndex(row * width + col).floatValue = EditorGUI.FloatField(fieldPosition, arrayProperty.GetArrayElementAtIndex(row * width + col).floatValue);
                }
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty heightProperty = property.FindPropertyRelative("height");
            return (heightProperty.intValue + 1) * EditorGUIUtility.singleLineHeight; // +1 for the label height
        }
    }
}