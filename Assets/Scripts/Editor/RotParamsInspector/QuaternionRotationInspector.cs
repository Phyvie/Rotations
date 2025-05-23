using RotParams;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(RotParams_Quaternion))]
    public class QuaternionRotationInspector : NestedPropertyDrawer
    {
        private SerializedProperty SP_i; 
        private SerializedProperty SP_j; 
        private SerializedProperty SP_k; 
        private SerializedProperty SP_real;
        private SerializedProperty SP_EnforceNormalisation; 

        private void Initialize(SerializedProperty property)
        {
            SP_i = property.FindPropertyRelative("_i"); 
            SP_j = property.FindPropertyRelative("_j"); 
            SP_k = property.FindPropertyRelative("_k"); 
            SP_real = property.FindPropertyRelative("_r");
            SP_EnforceNormalisation = property.FindPropertyRelative("enforceNormalisation"); 
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            InitializePropertyNesting(property);
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            RotParams_Quaternion targetRotParamsQuaternion = GetObject<RotParams_Quaternion>(property);

            if (position.width == 1)
            {
                EditorGUI.EndProperty();
                return; 
            }

            // SP_EnforceNormalisation.boolValue = EditorGUI.Toggle(position, new GUIContent("Enforce Normalisation / Rotation"), SP_EnforceNormalisation.boolValue); //-ZyKa Inspector -ZyKa Quaternion EnforceNormalisation
            // position.y += EditorGUIUtility.singleLineHeight; 
            
            float fieldWidth = position.width; 
            float labelWidth = 10;
            float valueWidth = (fieldWidth / 4) - labelWidth; 
            float spacing = 10;
            SerializedProperty[] valuesSP = new SerializedProperty[] { SP_real, SP_i, SP_j, SP_k};
            for (int i = 0; i < 4; i++)
            {
                SerializedProperty SP = valuesSP[i]; 
                Rect labelRect = new Rect(i*(labelWidth+valueWidth+spacing), position.y, labelWidth, position.height);
                EditorGUI.LabelField(labelRect, new GUIContent(SP.name.Substring(1)));
                Rect valueRect = new Rect(i*(labelWidth+valueWidth+spacing)+labelWidth, position.y, valueWidth, position.height);
                float preValue = ((LockableFloat)SP.boxedValue).value; 
                EditorGUI.PropertyField(valueRect, SP);
                float postValue = ((LockableFloat)SP.boxedValue).value;

                if (preValue != postValue)
                {
                    LockableFloat lockableFloat = targetRotParamsQuaternion.GetInternalLockableFloatByIndex(i); 
                    bool isLocked = targetRotParamsQuaternion.GetInternalLockableFloatByIndex(i).isLocked;
                    lockableFloat.isLocked = true; 
                    targetRotParamsQuaternion.NormalizeWithLocks(); //TODO: this does not work as intended, because the stored value is not yet changed, only the SerializedField is changed
                    lockableFloat.isLocked = isLocked; 
                }
            }
            
            EditorGUI.EndProperty(); 
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}