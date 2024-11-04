using System;
using System.Linq;
using System.Reflection;
using System.Text;
using RotationTypes;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(TestClass))]
    public class TestClassInspector : NestedPropertyDrawer
    {
        private SerializedProperty testGimbleRotationArrayProp;
        private SerializedProperty testIntProp; 
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            
            // if (!property.isArray)
            {
                property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, new GUIContent("Foldout"));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;
                
                    testIntProp = property.FindPropertyRelative("testInt");
                    EditorGUI.PropertyField(position, testIntProp);
                    position.y += EditorGUI.GetPropertyHeight(testIntProp);
                
                    EditorGUI.indentLevel--; 
                } 
                EditorGUI.EndFoldoutHeaderGroup();
            }
            /*
            else //property.isArray = true; 
            {
                if (isFoldoutArray.Length != property.arraySize)
                {
                    bool[] newFoldoutArray = new bool[property.arraySize]; 
                    Array.Copy(isFoldoutArray, newFoldoutArray, Math.Min(isFoldoutArray.Length, property.arraySize));
                    isFoldoutArray = newFoldoutArray; 
                }

                for (int i = 0; i < isFoldoutArray.Length; i++)
                {
                    isFoldoutArray[i] = EditorGUI.BeginFoldoutHeaderGroup(position, isFoldout, new GUIContent("Foldout"));
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; 
                    if (isFoldoutArray[i])
                    {
                        EditorGUI.indentLevel++;
                        
                        string arrayElementPropPath = "testInt.Array.data[" + i + "]"; 
                        testIntProp = property.FindPropertyRelative(arrayElementPropPath);
                        EditorGUI.PropertyField(position, testIntProp);
                        position.y += EditorGUI.GetPropertyHeight(testIntProp);
                
                        EditorGUI.indentLevel--; 
                    } 
                    EditorGUI.EndFoldoutHeaderGroup();
                }
            }
            */
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        { 
            float testIntPropHeight = EditorGUI.GetPropertyHeight(testIntProp = property.FindPropertyRelative("testInt"));
            // testGimbleRotationArrayProp = property.FindPropertyRelative("testGimbleRotationArray");

            float foldedPropHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float unfoldPropertyHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                                         testIntPropHeight; 
            
            /* if (property.isArray)
            {
                return (foldOutCount * unfoldPropertyHeight) +
                    ((isFoldoutArray.Length - foldOutCount) * unfoldPropertyHeight); 
            } 
            else */
            {
                return property.isExpanded ? unfoldPropertyHeight : foldedPropHeight; 
            }
        }
    }
}
