using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotationTypes
{
    [Serializable]
    public class AngleType

    {
    private AngleType(string angleTypeName, double unitMultiplier, string unitLabel)
    {
        this.angleTypeName = angleTypeName;
        this.unitMultiplier = unitMultiplier;
    }

    public readonly string angleTypeName;
    public readonly double unitMultiplier;
    public readonly string unitLabel;
    [SerializeField] private bool thisEnforcedSerialization = true; 
    
    public static float ConvertAngle(float inAngle, AngleType inAngleType, AngleType outAngleType)
    {
        return (float)(inAngle / inAngleType.unitMultiplier * outAngleType.unitMultiplier);
    }

    public override string ToString()
    {
        return angleTypeName + $"({unitMultiplier})({unitLabel})"; 
    }

    public static readonly AngleType Radian = new AngleType("Radian", 2 * Math.PI, "2PI");
    public static readonly AngleType Degree = new AngleType("Degree", 360, "360°");
    public static readonly AngleType CirclePart = new AngleType("CirclePart", 1, "Circle(s)");

    public static readonly string[] AngleTypeNames = new String[] { "Radian", "Degree", "CirclePart" };
    public static readonly AngleType[] AngleTypes = new AngleType[] { Radian, Degree, CirclePart };
    }
}