using RotationTypes;
using RotationVisualisation;
using UnityEditor; 
using UnityEditor.UIElements; 
using UnityEngine.UIElements; 

namespace Editor
{
    [CustomEditor(typeof(MB_Rotation3D), true)]
    public class Rotation3D_Inspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eulerAngleRotation"));
            /*
            EditorGUILayout.PropertyField(serializedObject.FindProperty("quaternionRotation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("axisAngleRotation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("matrixRotation"));
            */
            
            serializedObject.ApplyModifiedProperties(); 
        }
    }
}