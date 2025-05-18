using System;
using System.ComponentModel;
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
    public class _RotParams_EulerAngleGimbleRing
    {
        public EGimbleAxis eAxis;
        public Vector3 RotationAxis => eAxis switch
        {
            EGimbleAxis.Yaw => Vector3.up,
            EGimbleAxis.Pitch => Vector3.right,
            EGimbleAxis.Roll => Vector3.forward,
            _ => Vector3.zero
        };

        private float _angle;
        public float Angle
        {
            get => _angle;
            set => _angle = value;
        }
        
        #region Constructors
        private _RotParams_EulerAngleGimbleRing()
        {
            eAxis = EGimbleAxis.Yaw;
            Angle = 0;
        }
        
        public _RotParams_EulerAngleGimbleRing(EGimbleAxis inEAxis, float inAngle)
        {
            eAxis = inEAxis; 
            Angle = inAngle;
        }
        
        public _RotParams_EulerAngleGimbleRing(_RotParams_EulerAngleGimbleRing rotParamsEulerAngleGimbleRing)
        {
            eAxis = rotParamsEulerAngleGimbleRing.eAxis;
            Angle = rotParamsEulerAngleGimbleRing.Angle;
        }
        
        public static _RotParams_EulerAngleGimbleRing Yaw()
        {
            return new _RotParams_EulerAngleGimbleRing(EGimbleAxis.Yaw, (float)Math.PI / 2); 
        }
        public static _RotParams_EulerAngleGimbleRing Pitch(RotParams_EulerAngles parent)
        {
            return new _RotParams_EulerAngleGimbleRing(EGimbleAxis.Pitch, (float)Math.PI / 2); 
        }
        public static _RotParams_EulerAngleGimbleRing Roll(RotParams_EulerAngles parent)
        {
            return new _RotParams_EulerAngleGimbleRing(EGimbleAxis.Roll, (float)Math.PI / 2); 
        }
        #endregion //Constructors
        
        public Vector3 GetLocalRotationAxis() //+ZyKa is this actually the local axis or is it the gloabl axis?
        {
            return eAxis switch
            {
                EGimbleAxis.Yaw => Vector3.up,
                EGimbleAxis.Pitch => Vector3.left,
                EGimbleAxis.Roll => Vector3.forward,
                _ => throw new InvalidEnumArgumentException()
            }; 
        }
        
        #region Converters
        public RotParams_Matrix toMatrixRotation()
        {
            return eAxis switch
            {
                EGimbleAxis.Yaw => new RotParams_Matrix(new float[3,3]
                {
                    { Mathf.Cos(Angle),  0, Mathf.Sin(Angle) },
                    {          0,                1,              0           },
                    { -Mathf.Sin(Angle), 0, Mathf.Cos(Angle) }
                }),
                EGimbleAxis.Pitch => new RotParams_Matrix(new float[3,3]
                {
                    { Mathf.Cos(Angle), -Mathf.Sin(Angle),  0 },
                    { Mathf.Sin(Angle),  Mathf.Cos(Angle),  0 },
                    {           0,                          0,              1 }
                }),
                EGimbleAxis.Roll => new RotParams_Matrix(new float[3,3]
                {
                    { 1,              0,                       0            },
                    { 0, Mathf.Cos(Angle), -Mathf.Sin(Angle) },
                    { 0, Mathf.Sin(Angle), Mathf.Cos(Angle) }
                }),
                _ => throw new InvalidEnumArgumentException()
            }; 
        }

        public void ExtractValueFromMatrix(RotParams_Matrix m)
        {
            switch (eAxis)
            {
                case EGimbleAxis.Yaw:
                    Angle = Mathf.Atan2(m[2, 0], m[0, 0]); 
                    break; 
                case EGimbleAxis.Pitch:
                    Angle = Mathf.Atan2(m[0, 1], m[0, 0]); 
                    break; 
                case EGimbleAxis.Roll:
                    Angle = Mathf.Atan2(m[2, 1], m[1, 1]); 
                    break; 
            }
        }

        public RotParams_Quaternion toQuaternionRotation() 
        {
            return new RotParams_Quaternion(GetLocalRotationAxis(), Angle);
        }
        
        public void ExtractValueFromQuaternion(RotParams_Quaternion q)
        {
            //TODO: Understand this
            switch (eAxis)
            {
                case EGimbleAxis.Yaw:
                    Angle = Mathf.Atan2(2.0f * (q.w * q.z + q.x * q.y), 1.0f - 2.0f * (q.y * q.y + q.z * q.z));
                    break; 
                case EGimbleAxis.Pitch:
                    Angle = Mathf.Atan2(2.0f * (q.w * q.y - q.z * q.x), 1.0f - 2.0f * (q.y * q.y + q.z * q.z));
                    break; 
                case EGimbleAxis.Roll:
                    Angle = Mathf.Atan2(2.0f * (q.w * q.x + q.y*q.z), 1.0f - 2.0f*(q.x * q.x + q.y * q.y)); 
                    break; 
            }
        }
        #endregion //Converters

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