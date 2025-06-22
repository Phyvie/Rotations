using RotContainers;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Editor.UtilityInspector
{
    [CustomPropertyDrawer(typeof(ScreenshotManager.ViewShot))]
    public class ViewShotDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 100f; // Adjust height for preview
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Access properties
            SerializedProperty textureProp = property.FindPropertyRelative("renderTexture");
            SerializedProperty nameProp = property.FindPropertyRelative("name");
            SerializedProperty rotParamsProp = property.FindPropertyRelative("rotParams");

            // Draw background box
            GUI.Box(position, GUIContent.none);

            // Set up rects
            Rect textureRect = new Rect(position.x + 5, position.y + 5, 90, 90);
            Rect nameRect = new Rect(position.x + 100, position.y + 5, position.width - 105, 20);
            Rect rotRect = new Rect(position.x + 100, position.y + 30, position.width - 105, 20);

            // Draw RenderTexture preview
            if (textureProp.objectReferenceValue != null)
            {
                RenderTexture rt = textureProp.objectReferenceValue as RenderTexture;
                if (rt != null)
                {
                    Texture2D preview = GetTextureFromRenderTexture(rt);
                    if (preview != null)
                        GUI.DrawTexture(textureRect, preview, ScaleMode.ScaleToFit);
                }
            }
            else
            {
                EditorGUI.LabelField(textureRect, "No RenderTexture");
            }

            // Draw name
            EditorGUI.PropertyField(nameRect, nameProp, new GUIContent("Name"));

            // Draw rotParams if needed (just show reference field)
            if (rotParamsProp != null)
            {
                EditorGUI.PropertyField(rotRect, rotParamsProp, new GUIContent("Rot Params"));
            }
        }

        private Texture2D GetTextureFromRenderTexture(RenderTexture rt)
        {
            // Convert RenderTexture to Texture2D
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = rt;

            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            RenderTexture.active = currentRT;
            return tex;
        }
    }
}
