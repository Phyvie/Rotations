using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AngleWithType))]
    public class AngleWithTypeInspector : NestedPropertyDrawer
    {
        private SerializedProperty showAngleTypeSelectorSP; 
        private SerializedProperty showAllAngleTypesSP;
        private SerializedProperty angleTypeSP;
        private SerializedProperty circlePartsSP;
        private AngleType boxedAngleType; 
        
        void Initialize(SerializedProperty property)
        {
            showAngleTypeSelectorSP = property.FindPropertyRelative("showAngleTypeSelector"); 
            showAllAngleTypesSP = property.FindPropertyRelative("showAllAngleTypes"); 
            angleTypeSP = property.FindPropertyRelative("angleType"); 
            circlePartsSP = property.FindPropertyRelative("circleParts");
            boxedAngleType = (AngleType) angleTypeSP.boxedValue; 
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //initializing
            EditorGUI.BeginProperty(position, label, property);
            InitializePropertyNesting(property);
            Initialize(property);
            position.height = EditorGUIUtility.singleLineHeight;

            float viewWidth = EditorGUIUtility.currentViewWidth;
            float offsetWidth = 30; 
            float unitWidth = 80;
            float foldoutWidth = 50; 
            
            if (true || showAngleTypeSelectorSP.boolValue && !showAllAngleTypesSP.boolValue)
            {
                Rect foldoutRect = new Rect(position);
                foldoutRect.width = foldoutWidth; 
                
                Rect unitRect = new Rect(foldoutRect);
                unitRect.width = unitWidth;
                unitRect.x += foldoutWidth + offsetWidth; 
                
                Rect selectorRect = new Rect(unitRect);
                selectorRect.width = viewWidth - unitWidth - offsetWidth;
                selectorRect.x += unitWidth + offsetWidth;

                float oldCircleParts = AngleType.ConvertAngle(circlePartsSP.floatValue, AngleType.CirclePart, boxedAngleType); 
                float newAngleValue = EditorGUI.FloatField(unitRect, new GUIContent(""), oldCircleParts);
                circlePartsSP.floatValue = AngleType.ConvertAngle(newAngleValue, boxedAngleType, AngleType.CirclePart); 
                
                EditorGUI.PropertyField(selectorRect, angleTypeSP);
                
                position.y += EditorGUI.GetPropertyHeight(angleTypeSP); 
            }
            
            /*
            //Setting up the AngleType inspector
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
            */
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            return 100; 
        }
    }
}
