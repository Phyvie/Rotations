using UnityEditor;
using UnityEngine;

namespace Editor
{
    //TODO: Unity seems to not accept this as a type for a PropertyDrawer
    [CustomPropertyDrawer(typeof(Matrix4x4))]
    public class Matrix4X4_Inspector : PropertyDrawer
    {
        public override void OnGUI(Rect positionRect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(positionRect, label, property);
            EditorGUI.PrefixLabel(positionRect, GUIUtility.GetControlID(FocusType.Passive), label);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    SerializedProperty m00 = property.FindPropertyRelative("m_Data[0]");
                    m00.floatValue = EditorGUILayout.FloatField(m00.floatValue); 
                    SerializedProperty m01 = property.FindPropertyRelative("m_Data[1]");
                    m01.floatValue = EditorGUILayout.FloatField(m01.floatValue); 
                    SerializedProperty m02 = property.FindPropertyRelative("m_Data[2]");
                    m02.floatValue = EditorGUILayout.FloatField(m02.floatValue); 
                    SerializedProperty m03 = property.FindPropertyRelative("m_Data[3]");
                    m03.floatValue = EditorGUILayout.FloatField(m03.floatValue); 
                }
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.BeginVertical();
                {
                    SerializedProperty m10 = property.FindPropertyRelative("m_Data[4]");
                    m10.floatValue = EditorGUILayout.FloatField(m10.floatValue); 
                    SerializedProperty m11 = property.FindPropertyRelative("m_Data[5]");
                    m11.floatValue = EditorGUILayout.FloatField(m11.floatValue); 
                    SerializedProperty m12 = property.FindPropertyRelative("m_Data[6]");
                    m12.floatValue = EditorGUILayout.FloatField(m12.floatValue); 
                    SerializedProperty m13 = property.FindPropertyRelative("m_Data[7]");
                    m13.floatValue = EditorGUILayout.FloatField(m13.floatValue); 
                }
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndHorizontal(); 
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4 + 8; 
        }
    }
}