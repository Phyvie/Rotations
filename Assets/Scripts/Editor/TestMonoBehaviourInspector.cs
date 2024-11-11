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

            // EditorGUILayout.PropertyField(serializedObject.FindProperty("testObject"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("testObjectArray"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("ZyKa"));
            
            serializedObject.ApplyModifiedProperties();
        }   
    }
}