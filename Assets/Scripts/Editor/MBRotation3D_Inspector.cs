using RotationVisualisation;
using UnityEditor; 

namespace Editor
{
    [CustomEditor(typeof(MB_Rotation3D), true)]
    public class MBRotation3D_Inspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eulerAngleRotation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("quaternionRotation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("axisAngleRotation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("matrixRotation"));
            
            serializedObject.ApplyModifiedProperties(); 
        }
    }
}