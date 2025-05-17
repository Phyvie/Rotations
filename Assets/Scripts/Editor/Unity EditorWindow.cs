using UnityEditor;
using UnityEngine;

namespace Editor
{
    /* -ZyKa LaterFeature, figure out whether this feature might be useful some time later
    public class ZyKaEditorWindow : EditorWindow
    {
        private string MeshPath = "Assets/Art/Meshes/ProceduralMesh.asset"; 
        
        [MenuItem("Assets/Create/Custom/MeshWindow")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ZyKaEditorWindow)); 
        }

        private void OnGUI()
        {
            MeshPath = GUILayout.TextField("MeshPath");

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Mesh"))
            {
                Debug.Log("Creating Mesh"); 
            }
        }
    }
    */
}