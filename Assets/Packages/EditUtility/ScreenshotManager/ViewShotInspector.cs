using UnityEditor;
using UnityEngine;
using Utility; 

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
        SerializedProperty textureProp = property.FindPropertyRelative("texture2D");
        SerializedProperty nameProp = property.FindPropertyRelative("name");
        SerializedProperty rotParamsProp = property.FindPropertyRelative("rotParams");

        // Draw background box
        GUI.Box(position, GUIContent.none);

        // Set up rects
        Rect textureRect = new Rect(position.x + 5, position.y + 5, 90, 90);
        Rect nameRect = new Rect(position.x + 100, position.y + 5, position.width - 105, 20);
        Rect rotRect = new Rect(position.x + 100, position.y + 30, position.width - 105, 20);

        // Draw Texture2D preview
        if (textureProp.objectReferenceValue != null)
        {
            Texture2D tex = textureProp.objectReferenceValue as Texture2D;
            if (tex != null)
            {
                GUI.DrawTexture(textureRect, tex, ScaleMode.ScaleToFit);
            }
            else
            {
                EditorGUI.LabelField(textureRect, "Invalid Texture2D");
            }
        }
        else
        {
            EditorGUI.LabelField(textureRect, "No Texture2D");
        }

        // Draw name
        EditorGUI.PropertyField(nameRect, nameProp, new GUIContent("Name"));

        // Draw rotParams (reference field)
        if (rotParamsProp != null)
        {
            EditorGUI.PropertyField(rotRect, rotParamsProp, new GUIContent("Rot Params"));
        }
    }
}