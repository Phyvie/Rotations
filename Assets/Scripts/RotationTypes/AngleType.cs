using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace RotationTypes
{
    [Serializable]
    public class AngleType
    {
        private AngleType(string angleTypeName, double unitMultiplier)
        {
            this.angleTypeName = angleTypeName; 
            this.unitMultiplier = unitMultiplier; 
        }

        public readonly string angleTypeName; 
        public readonly double unitMultiplier; 
        
        public static float ConvertAngle(float inAngle, AngleType inAngleType, AngleType outAngleType)
        {
            return (float) (inAngle / inAngleType.unitMultiplier * outAngleType.unitMultiplier); 
        }
        
        public static readonly AngleType Radian = new AngleType("Radian", 2 * Math.PI);
        public static readonly AngleType Degree = new AngleType("Degree", 360);
        public static readonly AngleType CirclePart = new AngleType("CirclePart", 1); 
        public static readonly AngleType Grad = new AngleType("Grad", 400);

        public static readonly string[] AngleTypeNames = new String[] { "Radian", "Degree", "CirclePart", "Grad"};
        public static readonly AngleType[] AngleTypes = new AngleType[]{ Radian, Degree, CirclePart, Grad}; 
    }
}