using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class ExtensionMethods
{
    public static void FromMatrix4x4(this float3x3 thisFloat3X3, Matrix4x4 otherMatrix)
    {
        thisFloat3X3.c0 = new float3(otherMatrix.m00, otherMatrix.m01, otherMatrix.m02); 
        thisFloat3X3.c1 = new float3(otherMatrix.m10, otherMatrix.m11, otherMatrix.m12); 
        thisFloat3X3.c2 = new float3(otherMatrix.m20, otherMatrix.m21, otherMatrix.m22);
    }
    
    public static float3x3 ToFloat3x3(this Matrix4x4 matrix)
    {
        float3x3 returnValue; 
        returnValue.c0 = new float3(matrix.m00, matrix.m01, matrix.m02); 
        returnValue.c1 = new float3(matrix.m10, matrix.m11, matrix.m12); 
        returnValue.c2 = new float3(matrix.m20, matrix.m21, matrix.m22);
        return returnValue; 
    }
}
