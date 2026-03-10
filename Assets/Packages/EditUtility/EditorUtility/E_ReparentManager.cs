using System.Collections.Generic;
using MathExtensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad] 
public static class E_ReparentManager
{
    private static readonly string ToggleKey = "ReparentedObjectEditorActive";
    private static bool _isActive;

    private static Dictionary<Transform, Transform> ParentsForChildren = new Dictionary<Transform, Transform>();
    
    static E_ReparentManager()
    {
        IsActive = EditorPrefs.GetBool(ToggleKey, false);

        UpdateParentDictionary();

        IsActive = IsActive; 
    }

    public static bool IsActive
    {
        get => _isActive;
        set
        {
            if (value)
            {
                EditorApplication.hierarchyChanged += OnHierarchyChanged;
                EditorSceneManager.sceneOpened += UpdateParentDictionaryOnSceneOpen;
                EditorSceneManager.sceneClosed += UpdateParentDictionaryOnSceneClose;
            }
            else
            {
                EditorApplication.hierarchyChanged -= OnHierarchyChanged;
                EditorSceneManager.sceneOpened -= UpdateParentDictionaryOnSceneOpen;
                EditorSceneManager.sceneClosed -= UpdateParentDictionaryOnSceneClose; 
            }
            
            _isActive = value;   
        }
    }
    
    // This method is called whenever the hierarchy is changed (including reparenting or transform changes)
    private static void OnHierarchyChanged()
    {
        if (IsActive)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.transform.hasChanged)
                {
                    ParentsForChildren.TryGetValue(obj.transform, out Transform previousParent);
                    if (obj.transform.parent != previousParent)
                    {
                        ResetTransformToPreviousRelativeTransform(obj.transform);
                    }
                    obj.transform.hasChanged = false;  
                }
            }
        }
    }
    
    private static void UpdateParentDictionaryOnSceneClose(Scene scene)
    {
        UpdateParentDictionary();
    }

    private static void UpdateParentDictionaryOnSceneOpen(Scene scene, OpenSceneMode mode)
    {
        UpdateParentDictionary();
    }
    
    private static void UpdateParentDictionary()
    {
        ParentsForChildren.Clear();
        foreach (Transform t in GameObject.FindObjectsOfType<Transform>())
        {
            ParentsForChildren.Add(t, t.parent);
        }
    }
    
    private static void ResetTransformToPreviousRelativeTransform(Transform t)
    {
        ParentsForChildren.TryGetValue(t, out Transform previousParent);
        if (previousParent == t.parent)
        {
            Debug.LogWarning($"Resetting relative Rotation of {t.name} even though the parent didn't get changed"); 
            return; 
        }
        Debug.Log($"Reparenting {t.name} from {previousParent?.name ?? "null"} to {t.parent?.name ?? "null"}");
        {
            Vector3 relativePosition = Vector3.zero; 
            Quaternion relativeRotation = Quaternion.identity;
            Vector3 relativeScale = Vector3.one; 
            t.GetTransformRelativeTo(previousParent, ref relativePosition, ref relativeRotation, ref relativeScale);
            t.localPosition = relativePosition;
            t.localRotation = relativeRotation;
            t.localScale = relativeScale;
        }
        ParentsForChildren.TryAdd(t, t.parent);
        ParentsForChildren[t] = t.parent;
    }
    
    // Custom menu item for toggling the functionality
    [MenuItem("Tools/Reparented Object Editor/Toggle Active %#r")]
    private static void ToggleActive()
    {
        IsActive = !IsActive;

        EditorPrefs.SetBool(ToggleKey, IsActive);

        if (IsActive)
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            Debug.Log("ReparentedObjectEditor is now active.");
        }
        else
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            Debug.Log("ReparentedObjectEditor is now inactive.");
        }
    }

    [MenuItem("Tools/Reparented Object Editor/Show State")]
    private static void ShowState()
    {
        Debug.Log($"ReparentedObjectEditor is currently {(IsActive ? "active" : "inactive")}.");
    }
}