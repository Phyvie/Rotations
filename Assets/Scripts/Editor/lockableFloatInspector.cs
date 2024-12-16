using System.Globalization;
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(LockableFloat))]
    public class lockableFloatInspector : PropertyDrawer
    {
        private SerializedProperty SP_value;
        private SerializedProperty SP_isLocked;

        private void Initialize(SerializedProperty property)
        {
            SP_value = property.FindPropertyRelative("value");
            SP_isLocked = property.FindPropertyRelative("isLocked"); 
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            float fieldWidth = position.width;
            if (fieldWidth == 1)
            {
                EditorGUI.EndProperty();
                return; 
            }

            float lockWidth = 30; 
            
            Rect valueRect = new Rect(position);
            valueRect.width = fieldWidth - lockWidth - 10;
            valueRect.x = position.x; 
            
            Rect lockRect = new Rect(position);
            lockRect.width = lockWidth;
            lockRect.x = position.x + fieldWidth - lockWidth;
            
            if (!SP_isLocked.boolValue)
            {
                SP_value.floatValue = EditorGUI.FloatField(valueRect, new GUIContent(""), SP_value.floatValue); 
            }
            else
            {
                EditorGUI.LabelField(valueRect, new GUIContent(SP_value.floatValue.ToString(CultureInfo.InvariantCulture)));
            }
            SP_isLocked.boolValue = EditorGUI.Foldout(lockRect, SP_isLocked.boolValue, new GUIContent("")); 
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight; 
        }
    }
}