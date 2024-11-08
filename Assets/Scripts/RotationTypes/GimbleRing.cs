using System;
using Unity.VisualScripting;
using UnityEngine;

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
    public class GimbleRing
    {
        private GimbleRing()
        {
            eAxis = EGimbleAxis.Yaw;
            angle = 0;
        }
        
        public GimbleRing(EGimbleAxis inEAxis, float inAngle, AngleType ownAngleType, EulerAngleRotation parentEulerAngle)
        {
            eAxis = inEAxis; 
            angle = inAngle;
        }
        
        public GimbleRing(GimbleRing gimbleRing)
        {
            eAxis = gimbleRing.eAxis;
            angle = gimbleRing.angle;
        }
        
        public Vector3 GetLocalRotationAxis()
        {
            return eAxis switch
            {
                EGimbleAxis.Yaw => Vector3.up,
                EGimbleAxis.Pitch => Vector3.left,
                EGimbleAxis.Roll => Vector3.forward,
                _ => throw new UnexpectedEnumValueException<EGimbleAxis>(eAxis)
            }; 
        }
        
        public EGimbleAxis eAxis;
        public float angle;

        public static GimbleRing Yaw(EulerAngleRotation parent)
        {
            return new GimbleRing(EGimbleAxis.Yaw, (float)Math.PI / 2, AngleType.Radian, parent); 
        }
        public static GimbleRing Pitch(EulerAngleRotation parent)
        {
            return new GimbleRing(EGimbleAxis.Pitch, (float)Math.PI / 2, AngleType.Radian, parent); 
        }
        public static GimbleRing Roll(EulerAngleRotation parent)
        {
            return new GimbleRing(EGimbleAxis.Roll, (float)Math.PI / 2, AngleType.Radian, parent); 
        }
        
        
        public MatrixRotation toMatrixRotation(AngleType angleType)
        {
            float angleInRadian = AngleType.ConvertAngle(angle, angleType, AngleType.Radian); 
            
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
                _ => throw new UnexpectedEnumValueException<EGimbleAxis>(eAxis)
            }; 
        }

        public void ExtractValueFromMatrix(MatrixRotation m)
        {
            switch (eAxis)
            {
                case EGimbleAxis.Yaw:
                    angle = Mathf.Atan2(m[2, 0], m[0, 0]); 
                    break; 
                case EGimbleAxis.Pitch:
                    angle = Mathf.Atan2(m[0, 1], m[0, 0]); 
                    break; 
                case EGimbleAxis.Roll:
                    angle = Mathf.Atan2(m[2, 1], m[1, 1]); 
                    break; 
            }
        }

        public QuaternionRotation toQuaternionRotation(AngleType angleType) //TODO: Test this Function
        {
            return new QuaternionRotation(GetLocalRotationAxis(), angle);
        }
        
        public void ExtractValueFromQuaternion(QuaternionRotation q)
        {
            //TODO: Understand this
            switch (eAxis)
            {
                case EGimbleAxis.Yaw:
                    angle = Mathf.Atan2(2.0f * (q.w * q.z + q.x * q.y), 1.0f - 2.0f * (q.y * q.y + q.z * q.z));
                    break; 
                case EGimbleAxis.Pitch:
                    angle = Mathf.Atan2(2.0f * (q.w * q.y - q.z * q.x), 1.0f - 2.0f * (q.y * q.y + q.z * q.z));
                    break; 
                case EGimbleAxis.Roll:
                    angle = Mathf.Atan2(2.0f * (q.w * q.x + q.y*q.z), 1.0f - 2.0f*(q.x * q.x + q.y * q.y)); 
                    break; 
            }
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