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

            // SerializedProperty eulerRotProp = property.FindPropertyRelative("eulerAngleRotation");
            // EditorGUI.PropertyField(position, eulerRotProp);
            // position.y += EditorGUI.GetPropertyHeight(eulerRotProp); 
                
            // SerializedProperty angleWithTypeProp = property.FindPropertyRelative("angleWithType");
            // EditorGUI.PropertyField(position, angleWithTypeProp);
            // position.y += EditorGUI.GetPropertyHeight(angleWithTypeProp); 
            
            SerializedProperty angleTypeProp1 = property.FindPropertyRelative("angleTypeA");
            EditorGUI.PropertyField(position, angleTypeProp1);
            Debug.Log(angleTypeProp1.boxedValue); 
            position.y += EditorGUI.GetPropertyHeight(angleTypeProp1); 
            
            SerializedProperty angleTypeProp2 = property.FindPropertyRelative("angleTypeB");
            EditorGUI.PropertyField(position, angleTypeProp2);
            Debug.Log(angleTypeProp2.boxedValue); 
            position.y += EditorGUI.GetPropertyHeight(angleTypeProp2); 
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        { 
            return 100; 
        }
    }
}
