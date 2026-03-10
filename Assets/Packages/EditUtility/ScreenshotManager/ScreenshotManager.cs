using UnityEditor;
using UnityEngine;
using Utility;

[CustomEditor(typeof(ScreenshotManager))]
public class ScreenshotManagerEditor : UnityEditor.Editor
{
    SerializedProperty generalRotationContainer;
    SerializedProperty screenshotSettings;
    SerializedProperty interpolationSettings;
    SerializedProperty viewShots;

    UnityEditor.Editor screenshotSettingsEditor;
    UnityEditor.Editor interpolationSettingsEditor;

    // Foldout states
    private bool screenshotSettingsFoldout = true;
    private bool interpolationSettingsFoldout = true;

    private void OnEnable()
    {
        generalRotationContainer = serializedObject.FindProperty("comboRotCot");
        screenshotSettings = serializedObject.FindProperty("screenshotSettings");
        interpolationSettings = serializedObject.FindProperty("interpolationSettings");
        viewShots = serializedObject.FindProperty("viewShots");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(generalRotationContainer);

        DrawScriptableObjectWithFoldout(screenshotSettings, ref screenshotSettingsEditor, ref screenshotSettingsFoldout, "Screenshot Settings");
        DrawScriptableObjectWithFoldout(interpolationSettings, ref interpolationSettingsEditor, ref interpolationSettingsFoldout, "Interpolation Settings");

        EditorGUILayout.PropertyField(viewShots, true);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawScriptableObjectWithFoldout(SerializedProperty property, ref UnityEditor.Editor editor, ref bool foldoutState, string label)
    {
        EditorGUILayout.PropertyField(property, new GUIContent(label));
        if (property.objectReferenceValue == null)
            return;

        foldoutState = EditorGUILayout.Foldout(foldoutState, $"Edit {label}", true, EditorStyles.foldoutHeader);
        if (foldoutState)
        {
            EditorGUI.indentLevel++;
            UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
            if (editor != null)
            {
                EditorGUILayout.Space(4);
                editor.OnInspectorGUI();
            }
            EditorGUI.indentLevel--;
        }
    }
}
