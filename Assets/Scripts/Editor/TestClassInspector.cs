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

            /*
            SerializedProperty floatPropInfo = property.FindPropertyRelative("testFloatField"); 
            float oldValue = floatPropInfo.floatValue;
            EditorGUI.PropertyField(position, floatPropInfo);
            float newValue = floatPropInfo.floatValue;
            floatPropInfo.floatValue = oldValue; 
            if (oldValue != newValue)
            {
                PropertyInfo gsFloatPropInfo = typeof(TestClass).GetProperty("GSFloatProp", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                gsFloatPropInfo!.SetMethod.Invoke(testObject, new object[] {newValue});
            }
            position.y += EditorGUIUtility.singleLineHeight + 2;
            */
            
            PropertyInfo gsFloatPropInfo = typeof(TestClass).GetProperty("GSFloatProp", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            float newValue = EditorGUI.FloatField(position, "gsFloatProp", (float) gsFloatPropInfo!.GetValue(testObject)); 
            gsFloatPropInfo!.SetMethod.Invoke(testObject, new object[] {newValue});
            position.y += EditorGUIUtility.singleLineHeight + 2;
            

            EditorGUI.PropertyField(position, property.FindPropertyRelative("halfFloatField"));
            position.y += EditorGUIUtility.singleLineHeight + 2; 
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        { 
            return (EditorGUIUtility.singleLineHeight + 2) * 2; 
        }
    }
}
