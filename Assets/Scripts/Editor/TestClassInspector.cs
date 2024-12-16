using System.Reflection;
using RotationTypes;
using TestScripts;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(TestClass))]
    public class TestClassInspector : NestedPropertyDrawer
    {
        private SerializedProperty testGimbleRotationArrayProp;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Initialising
            InitializePropertyNesting(property);
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            TestClass testObject = GetObject<TestClass>(property);
            
            
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        { 
            return 100; 
        }
    }
}
