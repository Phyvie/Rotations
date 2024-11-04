using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TestMonoBehaviour))]
    public class TestMonoBehaviourInspector : UnityEditor.Editor
    {
        private TestStruct structObject;
        private Person classObject = new Person();
        private Person secondClassObject = new Person(); 
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("testAngleTypeArray"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("testClass")); 
            EditorGUILayout.PropertyField(serializedObject.FindProperty("testClassArray"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("testInt"));
            
            serializedObject.ApplyModifiedProperties();
            
            secondClassObject.SetAge();
            int ageCopy = secondClassObject.age; 
        }   
    }

    struct TestStruct
    {
        private String firstName;
        private String lastName;
        private int Age;
        
        public bool isProtected;
        
        public String GetFullName()
        {
            return firstName + lastName; 
        }
    }

    class Person
    {
        public String firstName;
        public String lastName;
        private int age;

        public int GetAge()
        {
            return age; 
        }
        
        public void SetAge(int newAge)
        {
            if (newAge > 130)
            {
                Debug.LogError("Age > 130 is not possible");
                return; 
            }
            
            age = newAge; 
        }
        
        public String GetFullName()
        {
            return firstName + lastName; 
        }
    }
}