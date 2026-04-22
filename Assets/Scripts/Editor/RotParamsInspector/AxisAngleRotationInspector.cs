
using System.Collections.Generic;
using Editor;
using RotParams;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RotParams_AxisAngle))]
public class AxisAngleRotationInspector : NestedPropertyDrawer
{
    private const float LockToggleWidth = 18f;
    private const float LabelWidth = 20f;
    private const float Spacing = 4f;

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
        
        EditorGUIUtility.IconContent("LockIcon"); 
        
        Rect fieldPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

        // Get target object via PropertyNesting, because GetField only works if the property is at topmost level; i.e. not in an array or a property of a property
        InitializePropertyNesting(property);
        var target = objectHierarchy[^1] as RotParams_AxisAngle;

        if (target == null)
        {
            EditorGUI.LabelField(fieldPosition, "RotParams_AxisAngle is null");
            EditorGUI.EndProperty();
            return;
        }

        // Draw serialized 'typedAngle' field at the top
        SerializedProperty angleProperty = property.FindPropertyRelative("typedAngle");
        if (angleProperty != null)
        {
            EditorGUI.PropertyField(fieldPosition, angleProperty, true);
            fieldPosition.y += EditorGUI.GetPropertyHeight(angleProperty, true) + Spacing;
        }
        
        LockableVectorInspector.DrawComponentWithLock(ref fieldPosition, "X", target.AxisX, target.XLocked, 
            newValue => target.AxisX = newValue, 
            newLock => target.XLocked = newLock 
            );
        
        LockableVectorInspector.DrawComponentWithLock(ref fieldPosition, "Y", target.AxisY, target.YLocked, 
            newValue => target.AxisY = newValue, 
            newLock => target.YLocked = newLock 
        );
        
        LockableVectorInspector.DrawComponentWithLock(ref fieldPosition, "Z", target.AxisZ, target.ZLocked, 
            newValue => target.AxisZ = newValue, 
            newLock => target.ZLocked = newLock 
        );
        
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

        SerializedProperty angleProperty = property.FindPropertyRelative("typedAngle");
        float angleHeight = angleProperty != null ? EditorGUI.GetPropertyHeight(angleProperty, true) + Spacing : 0f;
        
        // Foldout (1) +  X,Y,Z (3) + TypedAngle
        return 4 * (EditorGUIUtility.singleLineHeight + Spacing) + angleHeight; 
    }
}
