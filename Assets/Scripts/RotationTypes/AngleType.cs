using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace RotationTypes
{
    [Serializable]
    public class AngleType
    {
        private AngleType(string angleTypeName, double fullCircleUnits)
        {
            this.angleTypeName = angleTypeName; 
            this.fullCircleUnits = fullCircleUnits; 
        }

        public readonly string angleTypeName; 
        public readonly double fullCircleUnits; //TODO: I wish this could be a readonly -> wherever AngleType is accessed it should just reference one of the static AngleTypes, but I couldn't get that to work with the PropertyDrawer
        
        public static float ConvertAngle(float inAngle, AngleType inAngleType, AngleType outAngleType)
        {
            return (float) (inAngle / inAngleType.fullCircleUnits * outAngleType.fullCircleUnits); 
        }
        
        public static readonly AngleType Radian = new AngleType("Radian", 2 * Math.PI);
        public static readonly AngleType Degree = new AngleType("Degree", 360);
        public static readonly AngleType CirclePart = new AngleType("CirclePart", 1); 
        public static readonly AngleType Grad = new AngleType("Grad", 400);

        public static readonly string[] AngleTypeNames = new String[] { "Radian", "Degree", "CirclePart", "Grad"};
        public static readonly AngleType[] AngleTypes = new AngleType[]{ Radian, Degree, CirclePart, Grad}; 
    }
}