using UnityEditor;
using UnityEngine;

namespace MeshGenerator
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TorusGenerator : MonoBehaviour
    {
        [Range(3, 64)] public int segments = 24;
        [Range(3, 64)] public int rings = 12;
        public float radius = 1f;
        public float tubeRadius = 0.3f;

        [ContextMenu("GenerateTorus")]
        void GenerateTorus()
        {
            Mesh torusMesh = new Mesh();
            Undo.RecordObject(torusMesh, "Generated Torus");
            GetComponent<MeshFilter>().mesh = torusMesh;
            
            Vector3[] vertices = new Vector3[(segments + 1) * (rings + 1)];
            int[] triangles = new int[segments * rings * 6];
            Vector2[] uv = new Vector2[vertices.Length];
            float segmentAngle = 2 * Mathf.PI / segments;
            float ringAngle = 2 * Mathf.PI / rings;
            int vert = 0;
            int tris = 0;
            for (int i = 0; i <= segments; i++)
            {
                for (int j = 0; j <= rings; j++)
                {
                    float theta = i * segmentAngle;
                    float phi = j * ringAngle;
                    float x = (radius + tubeRadius * Mathf.Cos(phi)) * Mathf.Cos(theta);
                    float y = (radius + tubeRadius * Mathf.Cos(phi)) * Mathf.Sin(theta);
                    float z = tubeRadius * Mathf.Sin(phi);
                    vertices[vert] = new Vector3(x, y, z);
                    uv[vert] = new Vector2((float)i / segments, (float)j / rings);
                    if (i < segments && j < rings)
                    {
                        triangles[tris + 0] = vert;
                        triangles[tris + 1] = vert + rings + 1;
                        triangles[tris + 2] = vert + rings + 2;
                        triangles[tris + 3] = vert;
                        triangles[tris + 4] = vert + rings + 2;
                        triangles[tris + 5] = vert + 1;
                        tris += 6;
                    }

                    vert++;
                }
            }

            torusMesh.vertices = vertices;
            torusMesh.triangles = triangles;
            torusMesh.uv = uv;
            torusMesh.RecalculateNormals();
            
            Undo.FlushUndoRecordObjects();
        }
    }
}
