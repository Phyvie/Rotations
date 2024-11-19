using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AngleType))]
    public class AngleTypeInspector : NestedPropertyDrawer
    {
        public override void OnGUI(Rect positionRect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(positionRect, label, property);
            
            InitializePropertyNesting(property);
            
            positionRect.height = EditorGUIUtility.singleLineHeight; 
            
            int currentIndex = 0;
            foreach (AngleType angleType in AngleType.AngleTypes)
            {
                if (objectHierarchy[^1] as AngleType == angleType)
                {
                    break; 
                }
                currentIndex++; 
            }

            int newIndex = EditorGUI.Popup(positionRect, currentIndex, AngleType.AngleTypeNames);
            if (currentIndex >= AngleType.AngleTypes.Length || currentIndex < 0)
            {
                newIndex = 0;
                Debug.LogWarning($"Couldn't find angleType in AngleType.AngleTypes for object: {((MonoBehaviour)(property.serializedObject.targetObject))}");
            }
            
            if (newIndex >= 0 && newIndex != currentIndex)
            {
                SetFieldValue(property, AngleType.AngleTypes[newIndex]); //TODO: test
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight; 
        }
    }
}


