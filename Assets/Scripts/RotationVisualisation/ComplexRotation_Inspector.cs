using System;
using RotationTypes;
using UnityEditor;
using UnityEngine;

namespace RotationVisualisation
{
    [CustomEditor(typeof(ComplexRotation_MonoBehaviour))]
    public class ComplexRotation_Inspector : Editor
    {
        private GUIStyle _tableStyle; 
        private GUIStyle _headerColumnStyle; 
        private GUIStyle _columnStyle; 
        private GUIStyle _rowStyle; 
        private GUIStyle _rowHeaderStyle; 
        private GUIStyle _columnHeaderStyle; 
        private GUIStyle _columnLabelStyle; 
        private GUIStyle _cornerLabelStyle; 
        private GUIStyle _rowLabelStyle;

        private void OnSceneGUI()
        {
            InitGUIStyles();
        }

        public override void OnInspectorGUI()
        {
            ComplexRotation_MonoBehaviour mB = (ComplexRotation_MonoBehaviour)target;

            AngleType angleType = mB.Rotation.angleType;
            int selectedIndex = Array.IndexOf(AngleType.AngleTypes, angleType);
            if (selectedIndex < 0)
            {
                selectedIndex = 0; 
            }
            int newIndex = EditorGUILayout.Popup("AngleType: ", selectedIndex, AngleType.AngleTypeNames);
            if (newIndex >= 0 && newIndex != selectedIndex)
            {
                mB.Rotation.angleType = AngleType.AngleTypes[newIndex];
            }
            EditorGUILayout.Space(); 

            mB.Rotation.RotationAngle = EditorGUILayout.FloatField("RotationAngle", mB.Rotation.RotationAngle); 
            EditorGUILayout.Space(); 
            
            mB.Rotation.ComplexNumber = EditorGUILayout.Vector2Field("ComplexNumber", mB.Rotation.ComplexNumber);
            EditorGUILayout.Space(); 
            
            float[][] matrix; 
            if (mB.Rotation is not null && (matrix = mB.Rotation.Matrix2X2) is not null)
            {
                EditorGUILayout.LabelField("Matrix");
                EditorGUI.indentLevel++;
                
                EditorGUILayout.BeginVertical();
                for (int row = 0; row < matrix.Length; row++)
                {
                    EditorGUILayout.BeginHorizontal(); 
                    for (int column = 0; column < matrix[row].Length; column++)
                    {
                        matrix[row][column] = EditorGUILayout.FloatField(matrix[row][column]); 
                    } 
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(mB);
            }
        }

        private void InitGUIStyles()
        {
            _tableStyle = new GUIStyle("box");
            _tableStyle.padding = new RectOffset(5, 5, 5, 5);
            _tableStyle.margin = new RectOffset(2, 2, 2, 2);

            _headerColumnStyle = new GUIStyle();
            _headerColumnStyle.fixedWidth = 35;

            _columnStyle = new GUIStyle();
            _columnStyle.fixedWidth = 65;

            _rowStyle = new GUIStyle();
            _rowStyle.fixedHeight = 25;

            _rowHeaderStyle = new GUIStyle();
            _rowHeaderStyle.fixedWidth = _columnStyle.fixedWidth - 1; 
                
            _columnHeaderStyle = new GUIStyle();
            _columnHeaderStyle.fixedWidth = 30; 
            _columnHeaderStyle.fixedHeight = _rowStyle.fixedHeight - 1; 

            _columnLabelStyle = new GUIStyle ();
            _columnLabelStyle.fixedWidth = _rowHeaderStyle.fixedWidth - 6;
            _columnLabelStyle.alignment = TextAnchor.MiddleCenter;
            _columnLabelStyle.fontStyle = FontStyle.Bold;

            _cornerLabelStyle = new GUIStyle ();
            _cornerLabelStyle.fixedWidth = 42;
            _cornerLabelStyle.alignment = TextAnchor.MiddleRight;
            _cornerLabelStyle.fontStyle = FontStyle.BoldAndItalic;
            _cornerLabelStyle.fontSize = 14;
            _cornerLabelStyle.padding.top = -5;

            _rowLabelStyle = new GUIStyle ();
            _rowLabelStyle.fixedWidth = 25;
            _rowLabelStyle.alignment = TextAnchor.MiddleRight;
            _rowLabelStyle.fontStyle = FontStyle.Bold;
        }
    }
}
