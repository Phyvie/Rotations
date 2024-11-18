using System.Reflection;
using TestScripts;
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
            InitializePropertyNesting(property);
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            
            TestClass testObject = GetObject<TestClass>(property);

            SerializedProperty floatPropInfo = property.FindPropertyRelative("testFloatField");
            float oldValue = floatPropInfo.floatValue; 
            
            /*
            EditorGUI.PropertyField(position, floatPropInfo, new GUIContent("testFloatField.PropertyField"));
            position.y += EditorGUIUtility.singleLineHeight + 2; 
            
            PropertyInfo gsFloatPropInfo = typeof(TestClass).GetProperty("GSFloatProp", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            // float newValue = EditorGUI.FloatField(position, "TrueProperty", (float) gsFloatPropInfo!.GetValue(testObject));
            float newValue = floatPropInfo.floatValue;
            if (oldValue != newValue)
            {
                gsFloatPropInfo!.SetValue(testObject, newValue);
            }
            */
            
            EditorGUI.PropertyField(position, floatPropInfo, new GUIContent("testFloatField.PropertyField"));
            position.y += EditorGUIUtility.singleLineHeight + 2; 
            
            EditorGUI.PropertyField(position, property.FindPropertyRelative("halfFloatField"));
            position.y += EditorGUIUtility.singleLineHeight + 2;
            
            bool buttonClicked = EditorGUI.Toggle(position, false);
            position.y += EditorGUIUtility.singleLineHeight + 2;

            if (buttonClicked)
            {
                Debug.Log("buttonClicked");
                MethodInfo setTestFloatMethodInfo = typeof(TestClass).GetMethod("SetTestFloat", BindingFlags.Public | BindingFlags.Instance); 
                setTestFloatMethodInfo!.Invoke(testObject, new object[]{}); 
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        { 
            return (EditorGUIUtility.singleLineHeight + 2) * 3; 
        }
    }
}
