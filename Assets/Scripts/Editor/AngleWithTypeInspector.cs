using System;
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AngleWithType))]
    public class AngleWithTypeInspector : NestedPropertyDrawer
    {
        private SerializedProperty showAllAngleTypesSP;
        private SerializedProperty angleTypeSP;
        private AngleType boxedAngleType; 
        private SerializedProperty circlePartsSP;

        void Initialize(SerializedProperty property)
        {
            showAllAngleTypesSP = property.FindPropertyRelative("showAllAngleTypes"); 
            angleTypeSP = property.FindPropertyRelative("angleType"); 
            boxedAngleType = (AngleType)angleTypeSP.boxedValue; 
            circlePartsSP = property.FindPropertyRelative("circleParts"); 
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //initializing
            EditorGUI.BeginProperty(position, label, property);
            InitializePropertyNesting(property);
            Initialize(property);
            position.height = EditorGUIUtility.singleLineHeight;
            //Setting up the AngleType inspector
            
            float unitWidth = 100;
            float angleTypeWidth = 500;
            float valueWidth = EditorGUIUtility.currentViewWidth - unitWidth - angleTypeWidth; 
            if (!showAllAngleTypesSP.boolValue)
            {
                Rect valueRect = new Rect(position);
                valueRect.width = valueWidth;
                
                Rect unitRect = new Rect(valueRect);
                unitRect.width = unitWidth;
                unitRect.x += valueWidth + 10; 
                
                Rect angleTypeRect = new Rect(unitRect);
                angleTypeRect.width = angleTypeWidth;
                angleTypeRect.x += unitWidth + 10; 
                
                float oldAngleValue = circlePartsSP.floatValue; 
                float newAngleValue = EditorGUI.FloatField(valueRect, new GUIContent("angle"), circlePartsSP.floatValue);
                EditorGUI.LabelField(unitRect, new GUIContent(boxedAngleType.unitLabel));
                
                #region AngleTypeDropdown
                int currentIndex = Array.IndexOf(AngleType.AngleTypes, boxedAngleType);
                int newIndex = EditorGUI.Popup(angleTypeRect, currentIndex, AngleType.AngleTypeNames);
                if (currentIndex >= AngleType.AngleTypes.Length || currentIndex < 0)
                {
                    newIndex = 0;
                    Debug.LogWarning($"Couldn't find angleType in AngleType.AngleTypes for object: {((MonoBehaviour)(property.serializedObject.targetObject))}");
                }
                if (newIndex >= 0 && newIndex != currentIndex)
                {
                    boxedAngleType = AngleType.AngleTypes[newIndex]; 
                }
                #endregion //AngleTypeDropdown
            }
            else
            {
                position.width = EditorGUIUtility.currentViewWidth / 3 - 10;
                float offsetPerNumber = position.width + 5;
                
                float unitsInFullCircle = EditorGUI.FloatField(position, circlePartsSP.floatValue * 1);
                position.x += offsetPerNumber; 
                float unitsInRadian = EditorGUI.FloatField(position, circlePartsSP.floatValue * 1);
                position.x += offsetPerNumber; 
                float unitsInDegree = EditorGUI.FloatField(position, circlePartsSP.floatValue * 1);

                position.width = EditorGUIUtility.currentViewWidth; 
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            return 100; 
        }
    }
}
