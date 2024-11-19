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

            EditorGUILayout.PropertyField(serializedObject.FindProperty("angleTypeA"));
            Debug.Log(serializedObject.FindProperty("angleTypeA").boxedValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("angleTypeB"));
            Debug.Log(serializedObject.FindProperty("angleTypeB").boxedValue);
            
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("testObject"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("testObjectArray"));
            
            serializedObject.ApplyModifiedProperties();
        }   
    }
}