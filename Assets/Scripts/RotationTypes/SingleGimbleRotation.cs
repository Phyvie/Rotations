using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotationTypes
{
    [Serializable]
    public enum EGimbleAxis
    {
        Yaw, 
        Pitch, 
        Roll
    }

    [Serializable]
    public class SingleGimbleRotation
    {
        public SingleGimbleRotation(EGimbleAxis inEAxis, float inAngle, AngleType ownAngleType)
        {
            eAxis = inEAxis; 
            angle = inAngle;
            ownAngleType = ownAngleType; 
        }
        
        public SingleGimbleRotation Clone()
        {
            return new SingleGimbleRotation(eAxis, angle, ownAngleType); 
        }
        
        public Vector3 GetRotationAxis()
        {
            return eAxis switch
            {
                EGimbleAxis.Yaw => Vector3.up,
                EGimbleAxis.Pitch => Vector3.left,
                EGimbleAxis.Roll => Vector3.forward,
                _ => throw new NotImplementedException()
            }; 
        }

        public AngleType angleType
        {
            get => ownAngleType;
            set
            {
                if (!hasOwnAngleType)
                {
                    ownAngleType = value; 
                }
                else
                {
                    Debug.Log("SingleGimbleRotation hasWwnAngleType = true => must change ownAngleType in order to change angleType");
                }
            }
        }

        public AngleType ownAngleType
        {
            get => _ownAngleType;
            set
            {
                angle = AngleType.ConvertAngle(angle, _ownAngleType, value); 
                _ownAngleType = value; 
            }
        }
        
        
        public bool hasOwnAngleType = false; 
        private AngleType _ownAngleType; 
        public EGimbleAxis eAxis;
        public float angle;
        
        public static readonly SingleGimbleRotation Yaw = new SingleGimbleRotation(EGimbleAxis.Yaw, (float) Math.PI/2, AngleType.Radian); 
        public static readonly SingleGimbleRotation Pitch = new SingleGimbleRotation(EGimbleAxis.Pitch, (float) Math.PI/2, AngleType.Radian); 
        public static readonly SingleGimbleRotation Roll = new SingleGimbleRotation(EGimbleAxis.Roll, (float) Math.PI/2, AngleType.Radian);

        public QuaternionRotation toQuaternionRotation() //TODO: Test this Function
        {
            return new QuaternionRotation(GetRotationAxis(), angle, ownAngleType);
        }

        public MatrixRotation toMatrixRotation() //TODO: Test this Function
        {
            float angleInRadian = AngleType.ConvertAngle(angle, ownAngleType, AngleType.Radian); 
            
            return eAxis switch
            {
                EGimbleAxis.Yaw => new MatrixRotation(new float[3,3]
                {
                    { Mathf.Cos(angleInRadian),  0, Mathf.Sin(angleInRadian) },
                    {          0,                1,              0           },
                    { -Mathf.Sin(angleInRadian), 0, Mathf.Cos(angleInRadian) }
                }),
                EGimbleAxis.Pitch => new MatrixRotation(new float[3,3]
                {
                    { Mathf.Cos(angleInRadian), -Mathf.Sin(angleInRadian),  0 },
                    { Mathf.Sin(angleInRadian),  Mathf.Cos(angleInRadian),  0 },
                    {           0,                          0,              1 }
                }),
                EGimbleAxis.Roll => new MatrixRotation(new float[3,3]
                {
                    { 1,              0,                       0            },
                    { 0, Mathf.Cos(angleInRadian), -Mathf.Sin(angleInRadian) },
                    { 0, Mathf.Sin(angleInRadian), Mathf.Cos(angleInRadian) }
                }),
                _ => throw new NotImplementedException()
            }; 
        }

        public SingleGimbleRotation ExtractFromMatrix(MatrixRotation matrixRotation, EGimbleAxis inAxis)
        {
            throw new NotImplementedException(); 
        }

        public string GetRotationName()
        {
            return eAxis switch
            {
                EGimbleAxis.Yaw => "Yaw",
                EGimbleAxis.Pitch => "Pitch",
                EGimbleAxis.Roll => "Roll",
                _ => throw new ArgumentOutOfRangeException()
            }; 
        }
    }
}