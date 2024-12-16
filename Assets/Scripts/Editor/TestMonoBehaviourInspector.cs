using System;
using TestScripts;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TestMonoBehaviour))]
    public class TestMonoBehaviourInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("quaternionRotation"));
            
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("angleTypeA"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("angleTypeB"));
            
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("angleWithTypeA"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("angleWithTypeB"));
            
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("testObject"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("testObjectArray"));
            
            serializedObject.ApplyModifiedProperties();
        }   
    }
}