using System;
using System.Collections.Generic;
using Extensions.MathExtensions;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(LockableVector))]
    public class LockableVectorInspector : NestedPropertyDrawer
    {
        private const float LockToggleWidth = 18f;
        private const float LabelWidth = 20f;
        private const float Spacing = 4f; 
        
        //Foldout state per-property
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
            {
                return; 
            }
            
            EditorGUI.BeginProperty(position, label, property);

            Rect fieldPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight); 

            InitializePropertyNesting(property);
            LockableVector target = objectHierarchy[^1] as LockableVector;

            if (target == null)
            {
                EditorGUI.LabelField(fieldPosition, "LockableVector is null");
                EditorGUI.EndProperty();
                return;
            }
            
            Rect targetMagnitudeRect = new Rect(fieldPosition.x, fieldPosition.y, fieldPosition.width - LockToggleWidth - Spacing, EditorGUIUtility.singleLineHeight);
            float newTargetMagnitude = EditorGUI.FloatField(targetMagnitudeRect, new GUIContent("TargetMagnitude"), target.GetTargetMagnitude());
            if (newTargetMagnitude != target.GetTargetMagnitude())
            {
                target.SetTargetMagnitude(newTargetMagnitude);
            }
            
            Rect autoNormalizeRect = new Rect(fieldPosition.x + fieldPosition.width - LockToggleWidth, fieldPosition.y, LockToggleWidth, EditorGUIUtility.singleLineHeight);
            bool newAutoNormalize = EditorGUI.Toggle(autoNormalizeRect, target.GetAutoNormalizeToTargetMagnitude());
            if (newAutoNormalize != target.GetAutoNormalizeToTargetMagnitude())
            {
                target.SetAutoNormalizeToTargetMagnitude(newAutoNormalize);
            }
            position.y += EditorGUIUtility.singleLineHeight + Spacing;
            position.y += EditorGUIUtility.singleLineHeight + Spacing;
            
            float[] values = target.ValuesCopy;
            bool[] locks = target.LocksCopy;
            for (int i = 0; i < target.Dimensions; i++)
            {
                // DrawComponentWithLock(ref fieldPosition, "W", target.W, target.WLocked,
                //     newVal => target.W = newVal,
                //     newLock => target.WLocked = newLock);

                float lineHeight = EditorGUIUtility.singleLineHeight;
                
                Rect labelRect = new Rect(position.x, position.y, LabelWidth, lineHeight); 
                Rect floatFieldRect = new Rect(position.x + LabelWidth + Spacing, position.y, position.width - LabelWidth - LockToggleWidth - 3 * Spacing, lineHeight); 
                Rect lockToggleRect = new Rect(position.x + position.width - LockToggleWidth, position.y, LockToggleWidth, lineHeight);
                
                float newValue = EditorGUI.FloatField(floatFieldRect, new GUIContent(i.ToString()), values[i]);
                if (newValue != values[i])
                {
                    target.SetValue(i, newValue); 
                }

                bool newLock = EditorGUI.Toggle(lockToggleRect, locks[i]);
                if (newLock != locks[i])
                {
                    target.SetLock(i, newLock); 
                }
                
                position.y += lineHeight + Spacing;
            }

            Rect addDimensionRect = new Rect(position.x, position.y, position.width/2 + Spacing/2, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(addDimensionRect, new GUIContent("Add Dimension")))
            {
                target.Dimensions++; 
            }
            Rect removeDimensionRect = new Rect(position.x + position.width/2 + Spacing/2, position.y, position.width/2 + Spacing/2, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(removeDimensionRect, new GUIContent("Remove Dimension")))
            {
                target.Dimensions--;
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitializePropertyNesting(property);
            LockableVector target = objectHierarchy[^1] as LockableVector;
            
            string propertyKey = property.propertyPath;
            bool isExpanded = foldoutStates.TryGetValue(propertyKey, out bool expanded) && expanded;
            
            float foldingHeight = EditorGUIUtility.singleLineHeight + Spacing;
            
            float baseHeight = EditorGUIUtility.singleLineHeight + Spacing;
            float arrayHeight = (EditorGUIUtility.singleLineHeight + Spacing) * target.Dimensions;
            float dimensionButtons = EditorGUIUtility.singleLineHeight + Spacing;
            
            float fullHeight = baseHeight + arrayHeight + dimensionButtons;

            return isExpanded ? foldingHeight + fullHeight : foldingHeight; 
        }
        
        // Helper method for drawing this externally
        public static void DrawComponentWithLock(
            ref Rect position,
            string label,
            float value,
            bool isLocked,
            Action<float> setValue,
            Action<bool> setLock, 
            float labelWidth = 20f,
            float lockToggleWidth = 18f,
            float spacing = 4f)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            
            Rect floatFieldRect = new Rect(position.x, position.y,
                position.width - lockToggleWidth - spacing, lineHeight);
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth; 
            float newValue = EditorGUI.FloatField(floatFieldRect, new GUIContent(label), value);
            if (!Mathf.Approximately(newValue, value))
            {
                setValue(newValue);
            }
            EditorGUIUtility.labelWidth = oldLabelWidth;

            Rect lockToggleRect = new Rect(position.x + position.width - lockToggleWidth, position.y,
                lockToggleWidth, lineHeight);
            bool newLock = EditorGUI.Toggle(lockToggleRect, isLocked);
            if (newLock != isLocked)
            {
                setLock(newLock);
            }

            position.y += lineHeight + spacing;
        }
    }
}