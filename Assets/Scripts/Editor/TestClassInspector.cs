using System.Reflection;
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
            //Finding the Properties
            SerializedProperty multiplierSerialized = property.FindPropertyRelative("unitMultiplierField");
            float oldValue = multiplierSerialized.floatValue;
            SerializedProperty currentUnitsSerialized = property.FindPropertyRelative("currentUnitsField"); 
            
            /*
            //Version 1: 
            EditorGUI.PropertyField(position, multiplierSerialized, new GUIContent("unitMultiplierField.PropertyField"));
            position.y += EditorGUIUtility.singleLineHeight + 2;
            
            PropertyInfo gsFloatPropInfo = typeof(TestClass).GetProperty("GSUnitMultiplierField", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            float newValue = multiplierSerialized.floatValue;
            gsFloatPropInfo!.SetValue(testObject, newValue);
            Debug.Log("currentUnitsSerialized: " + currentUnitsSerialized.floatValue); 
            EditorGUI.PropertyField(position, currentUnitsSerialized);
            position.y += EditorGUIUtility.singleLineHeight + 2;
            */
            
            //Version 2: 
            multiplierSerialized.floatValue = EditorGUI.FloatField(position, "unitMultiplierField.PropertyField", multiplierSerialized.floatValue);
            position.y += EditorGUIUtility.singleLineHeight + 2;
            
            PropertyInfo gsFloatPropInfo = typeof(TestClass).GetProperty("GSUnitMultiplierField", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            float newValue = multiplierSerialized.floatValue;
            gsFloatPropInfo!.SetValue(testObject, newValue);
            Debug.Log("currentUnitsSerialized: " + currentUnitsSerialized.floatValue); 
            currentUnitsSerialized.floatValue = EditorGUI.FloatField(position, "unitMultiplierField.PropertyField", currentUnitsSerialized.floatValue);
            position.y += EditorGUIUtility.singleLineHeight + 2;
            
            
            
            // EditorGUI.BeginChangeCheck(); 
            // EditorGUI.EndChanceCheck(); 
            
            // EditorGUI.ApplyModifiedProperties(); 
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        { 
            return (EditorGUIUtility.singleLineHeight + 2) * 3; 
        }
    }
}
