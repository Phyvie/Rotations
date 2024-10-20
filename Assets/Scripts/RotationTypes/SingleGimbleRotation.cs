using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public enum ERotationAxis
    {
        Yaw, 
        Pitch, 
        Roll
    }

    [Serializable]
    public class SingleGimbleRotation
    {
        public SingleGimbleRotation(ERotationAxis inEAxis, float inAngle, AngleType inAngleType)
        {
            eAxis = inEAxis; 
            angle = inAngle;
            angleType = inAngleType; 
        }
        
        public SingleGimbleRotation Clone()
        {
            return new SingleGimbleRotation(eAxis, angle, angleType); 
        }
        
        public Vector3 GetRotationAxis()
        {
            return eAxis switch
            {
                ERotationAxis.Yaw => Vector3.up,
                ERotationAxis.Pitch => Vector3.left,
                ERotationAxis.Roll => Vector3.forward,
                _ => throw new NotImplementedException()
            }; 
        }
        
        public readonly ERotationAxis eAxis;
        public float angle;
        public AngleType angleType; 
        
        public static readonly SingleGimbleRotation Yaw = new SingleGimbleRotation(ERotationAxis.Yaw, (float) Math.PI/2, AngleType.Radian); 
        public static readonly SingleGimbleRotation Pitch = new SingleGimbleRotation(ERotationAxis.Pitch, (float) Math.PI/2, AngleType.Radian); 
        public static readonly SingleGimbleRotation Roll = new SingleGimbleRotation(ERotationAxis.Roll, (float) Math.PI/2, AngleType.Radian);

        public QuaternionRotation toQuaternionRotation() //TODO: Test this Function
        {
            return new QuaternionRotation(GetRotationAxis(), angle, angleType);
        }

        public MatrixRotation toMatrixRotation() //TODO: Test this Function
        {
            float angleInRadian = AngleType.ConvertAngle(angle, angleType, AngleType.Radian); 
            
            return eAxis switch
            {
                ERotationAxis.Yaw => new MatrixRotation(new float[3][]
                {
                    new float[3] { Mathf.Cos(angleInRadian),  0, Mathf.Sin(angleInRadian) },
                    new float[3] {          0,                1,              0           },
                    new float[3] { -Mathf.Sin(angleInRadian), 0, Mathf.Cos(angleInRadian) }
                }),
                ERotationAxis.Pitch => new MatrixRotation(new float[3][]
                {
                    new float[3] { Mathf.Cos(angleInRadian), -Mathf.Sin(angleInRadian),  0 },
                    new float[3] { Mathf.Sin(angleInRadian),  Mathf.Cos(angleInRadian),  0 },
                    new float[3] {           0,                          0,              1 }
                }),
                ERotationAxis.Roll => new MatrixRotation(new float[3][]
                {
                    new float[3] { 1,              0,                       0            },
                    new float[3] { 0, Mathf.Cos(angleInRadian), -Mathf.Sin(angleInRadian) },
                    new float[3] { 0, Mathf.Sin(angleInRadian), Mathf.Cos(angleInRadian) }
                }),
                _ => throw new NotImplementedException()
            }; 
        }

        public SingleGimbleRotation ExtractFromMatrix(MatrixRotation matrixRotation, ERotationAxis inAxis)
        {
            throw new NotImplementedException(); 
        }
    }
}