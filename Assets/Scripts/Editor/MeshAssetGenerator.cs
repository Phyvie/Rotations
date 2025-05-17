using UnityEditor;
using UnityEngine;

public class MeshAssetGenerator
{
    public static Mesh mesh;
    public static string savePath; 
    
    [MenuItem("Assets/Create/Custom/Generate Mesh")]
    public static void CreateMeshAsset()
    {
        // Define mesh data (simple triangle)
        Vector3[] vertices = GenerateVertices();
        int[] triangles = GenerateTriangles(); 

        mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles
        };

        mesh.normals = GenerateNormals(); 
        
        SaveMesh();
    }

    public static Vector3[] GenerateVertices()
    {
        return new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0)
        };
    }

    public static int[] GenerateTriangles()
    {
        return new int[]
        {
            0, 1, 2
        };
    }

    public static Vector3[] GenerateNormals()
    {
        mesh.RecalculateNormals();
        return mesh.normals;
    }

    public static void SaveMesh()
    {
        string path = !string.IsNullOrEmpty(savePath) ? savePath : "Assets/TriangleMesh.asset";
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = mesh;

        Debug.Log("Mesh asset created at " + path);
    }
}
