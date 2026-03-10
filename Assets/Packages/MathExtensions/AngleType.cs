using System;
using System.Collections.Generic;
using UnityEngine;

namespace RotParams
{
    [System.Serializable]
    public class AngleType 
    {
        #region publicstatic
        public static readonly AngleType Radian = new AngleType("Radian", 2 * Mathf.PI, "2PI");
        public static readonly AngleType Degree = new AngleType("Degree", 360, "360°");
        public static readonly AngleType CirclePart = new AngleType("CirclePart", 1, "Circle(s)");

        public static readonly List<string> AngleTypeNames = new List<string>() { "Radian", "Degree", "Circles" }; //this is a List instead of Array, because it's easier to use for UI Toolkit
        public static readonly AngleType[] AngleTypes = new AngleType[] { Radian, Degree, CirclePart };
        #endregion publicstatic
        
        #region variables
        [SerializeField] private string angleTypeName;
        [SerializeField] private double unitMultiplier;
        [SerializeField] private string unitLabel;

        public string AngleTypeName
        {
            get => angleTypeName;
            private set => angleTypeName = value;
        }
        
        public double UnitMultiplier
        {
            get => unitMultiplier;
            private set => unitMultiplier = value;
        }

        public string UnitLabel
        {
            get => unitLabel;
            private set => unitLabel = value;
        }
        #endregion variables

        private AngleType(string angleTypeName, double unitMultiplier, string unitLabel)
        {
            this.AngleTypeName = angleTypeName;
            this.UnitMultiplier = unitMultiplier;
            this.UnitLabel = unitLabel; 
        }

        public static float ConvertAngle(float inAngle, AngleType inAngleType, AngleType outAngleType)
        {
            return (float)(inAngle / inAngleType.UnitMultiplier * outAngleType.UnitMultiplier);
        }

        public override string ToString()
        {
            return AngleTypeName + $"({UnitMultiplier})({UnitLabel})"; 
        }
    }
}