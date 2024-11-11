using System.Collections;
using System.Reflection;
using Codice.CM.SEIDInfo;
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
            
            PropertyInfo gsFloatPropInfo = typeof(TestClass).GetProperty("GSFloatProp", BindingFlags.NonPublic | BindingFlags.Instance);
            gsFloatPropInfo!.GetValue(testObject);
            float newFloatValue = EditorGUI.FloatField(position, "gsFloatProp", testObject.GSFloatProp); //property.FindPropertyRelative("testFloatField").floatValue
            //gsFloatPropInfo.SetMethod.Invoke(testObject, new object[] {newFloatValue});
            position.y += EditorGUIUtility.singleLineHeight + 2; 
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        { 
            return EditorGUIUtility.singleLineHeight + 2; 
        }
    }
}
