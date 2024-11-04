using System;
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

            EditorGUILayout.PropertyField(serializedObject.FindProperty("mBTestClass"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("eulerAngleInMB"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("MBTestGimbleRotationArray"));
            
            serializedObject.ApplyModifiedProperties();
        }   
    }
}