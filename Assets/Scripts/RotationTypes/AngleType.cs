using System;
using System.Collections.Generic;

namespace RotationTypes
{
    [Serializable]
    public class AngleType
    {
        public AngleType(AngleType angleType)
        {
            this.fullCircleUnits = angleType.fullCircleUnits; 
        }
        
        private AngleType(double fullCircleUnits)
        {
            this.fullCircleUnits = fullCircleUnits; 
        }

        public double fullCircleUnits; //TODO: I wish this could be a readonly -> wherever AngleType is accessed it should just reference one of the static AngleTypes, but I couldn't get that to work with the PropertyDrawer
        
        public static float ConvertAngle(float inAngle, AngleType inAngleType, AngleType outAngleType)
        {
            return (float) (inAngle / inAngleType.fullCircleUnits * outAngleType.fullCircleUnits); 
        }
        
        public static readonly AngleType Radian = new AngleType(2 * Math.PI);
        public static readonly AngleType Degree = new AngleType(360);
        public static readonly AngleType CirclePart = new AngleType(1); 
        public static readonly AngleType Grad = new AngleType(400);

        public static readonly string[] AngleTypeNames = new String[] { "Radian", "Degree", "CirclePart", "Grad"};
        public static readonly AngleType[] AngleTypes = new AngleType[]{ Radian, Degree, CirclePart, Grad}; 
    }
}