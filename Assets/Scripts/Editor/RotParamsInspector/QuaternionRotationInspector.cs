using System.Collections.Generic;
using Editor;
using RotParams;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RotParams_Quaternion))]
public class QuaternionRotationInspector : NestedPropertyDrawer
{
    private const float LockToggleWidth = 18f;
    private const float LabelWidth = 20f;
    private const float Spacing = 4f;

    // Foldout state per-property
    private static readonly Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string propertyKey = property.propertyPath;

        if (!foldoutStates.ContainsKey(propertyKey))
        {
            foldoutStates[propertyKey] = false;
        }

        foldoutStates[propertyKey] = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            foldoutStates[propertyKey], label, true);

        if (!foldoutStates[propertyKey])
            return;

        EditorGUI.BeginProperty(position, label, property);

        Rect fieldPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

        // Get target object via PropertyNesting, because GetField only works if the property is at topmost level; i.e. not in an array or a property of a property
        InitializePropertyNesting(property);
        var target = objectHierarchy[^1] as RotParams_Quaternion;

        if (target == null)
        {
            EditorGUI.LabelField(fieldPosition, "RotParams_Quaternion is null");
            EditorGUI.EndProperty();
            return;
        }
        
        LockableVectorInspector.DrawComponentWithLock(ref fieldPosition, "W", target.W, target.WLocked,
            newVal => target.W = newVal,
            newLock => target.WLocked = newLock);

        LockableVectorInspector.DrawComponentWithLock(ref fieldPosition, "X", target.X, target.XLocked,
            newVal => target.X = newVal,
            newLock => target.XLocked = newLock);

        LockableVectorInspector.DrawComponentWithLock(ref fieldPosition, "Y", target.Y, target.YLocked,
            newVal => target.Y = newVal,
            newLock => target.YLocked = newLock);

        LockableVectorInspector.DrawComponentWithLock(ref fieldPosition, "Z", target.Z, target.ZLocked,
            newVal => target.Z = newVal,
            newLock => target.ZLocked = newLock);

        fieldPosition.y += EditorGUIUtility.singleLineHeight + Spacing;
        target.EnforceNormalisation = EditorGUI.ToggleLeft(fieldPosition, "Enforce Normalisation", target.EnforceNormalisation);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        string propertyKey = property.propertyPath;
        bool isExpanded = foldoutStates.TryGetValue(propertyKey, out bool expanded) && expanded;
        if (!isExpanded)
        {
            return EditorGUIUtility.singleLineHeight + Spacing;
        }
        
        // Foldout (1) + W,X,Y,Z (4) + Enforce Normalisation (1)
        return 6 * (EditorGUIUtility.singleLineHeight + Spacing); 
    }
}
