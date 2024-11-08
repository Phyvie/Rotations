using UnityEditor;
using UnityEngine;

namespace Editor
{
    /*
    [CustomPropertyDrawer(typeof(TestClass))]
    public class TestClassInspector : NestedPropertyDrawer<TestClass>
    {
        private SerializedProperty testGimbleRotationArrayProp;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(position, property.FindPropertyRelative("testName"));
            position.y += EditorGUIUtility.singleLineHeight + 2; 
            EditorGUI.PropertyField(position, property.FindPropertyRelative("child.childName")); 
            position.y += EditorGUIUtility.singleLineHeight + 2; 
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        { 
            return EditorGUIUtility.singleLineHeight*2; 
        }
    }
    */
}
