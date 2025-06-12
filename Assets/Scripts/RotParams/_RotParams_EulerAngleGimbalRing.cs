using System;
using System.ComponentModel;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace RotParams
{
    [Serializable]
    public enum EGimbleAxis
    {
        Yaw, 
        Pitch, 
        Roll
    }

    [Serializable]
    public class _RotParams_EulerAngleGimbalRing
    {
        #region Variables
        public EGimbleAxis eAxis;
        public Vector3 RotationAxis => eAxis switch
        {
            EGimbleAxis.Yaw => Vector3.up,
            EGimbleAxis.Pitch => Vector3.right,
            EGimbleAxis.Roll => Vector3.forward,
            _ => Vector3.zero
        };

        [SerializeField] private AngleWithType typedAngle; 
        public static string NameOfAngle => nameof(typedAngle); //for PropertyDrawer

        public float AngleInRadian
        {
            get => typedAngle.AngleInRadian; 
            set => typedAngle.AngleInRadian = value;
        }
        
        #endregion
        
        #region Constructors
        private _RotParams_EulerAngleGimbalRing()
        {
            eAxis = EGimbleAxis.Yaw;
            typedAngle = new AngleWithType(AngleType.Degree, 0);
        }
        
        public _RotParams_EulerAngleGimbalRing(EGimbleAxis inEAxis, float angleInRadian) : this(inEAxis, AngleType.Radian, angleInRadian)
        {
        }

        public _RotParams_EulerAngleGimbalRing(EGimbleAxis inEAxis, AngleType angleType, float angle) : this(inEAxis, new AngleWithType(angleType, angle))
        {
        }

        public _RotParams_EulerAngleGimbalRing(EGimbleAxis inEAxis, AngleWithType typedAngle)
        {
            eAxis = inEAxis; 
            this.typedAngle = typedAngle;
        }
        
        public _RotParams_EulerAngleGimbalRing(_RotParams_EulerAngleGimbalRing rotParamsEulerAngleGimbalRing)
        {
            eAxis = rotParamsEulerAngleGimbalRing.eAxis;
            typedAngle = new AngleWithType(rotParamsEulerAngleGimbalRing.typedAngle.angleType, rotParamsEulerAngleGimbalRing.typedAngle.AngleInCurrentUnit);
        }
        
        public static _RotParams_EulerAngleGimbalRing Yaw()
        {
            return new _RotParams_EulerAngleGimbalRing(EGimbleAxis.Yaw, AngleType.Degree, 90); 
        }
        public static _RotParams_EulerAngleGimbalRing Pitch(RotParams_EulerAngles parent)
        {
            return new _RotParams_EulerAngleGimbalRing(EGimbleAxis.Pitch, AngleType.Degree, 90); 
        }
        public static _RotParams_EulerAngleGimbalRing Roll(RotParams_EulerAngles parent)
        {
            return new _RotParams_EulerAngleGimbalRing(EGimbleAxis.Roll, AngleType.Degree, 90); 
        }
        #endregion //Constructors
        
        #region Converters
        public RotParams_Matrix toMatrixRotation()
        {
            return eAxis switch
            {
                EGimbleAxis.Yaw => new RotParams_Matrix(new float[3,3]
                {
                    { Mathf.Cos(AngleInRadian),  0, Mathf.Sin(AngleInRadian) },
                    {          0,                1,              0           },
                    { -Mathf.Sin(AngleInRadian), 0, Mathf.Cos(AngleInRadian) }
                }),
                EGimbleAxis.Pitch => new RotParams_Matrix(new float[3,3]
                {
                    { Mathf.Cos(AngleInRadian), -Mathf.Sin(AngleInRadian),  0 },
                    { Mathf.Sin(AngleInRadian),  Mathf.Cos(AngleInRadian),  0 },
                    {           0,                          0,              1 }
                }),
                EGimbleAxis.Roll => new RotParams_Matrix(new float[3,3]
                {
                    { 1,              0,                       0            },
                    { 0, Mathf.Cos(AngleInRadian), -Mathf.Sin(AngleInRadian) },
                    { 0, Mathf.Sin(AngleInRadian), Mathf.Cos(AngleInRadian) }
                }),
                _ => throw new InvalidEnumArgumentException()
            }; 
        }

        public void ExtractValueFromMatrix(RotParams_Matrix m)
        {
            switch (eAxis)
            {
                case EGimbleAxis.Yaw:
                    AngleInRadian = Mathf.Atan2(m[2, 0], m[0, 0]); 
                    break; 
                case EGimbleAxis.Pitch:
                    AngleInRadian = Mathf.Atan2(m[0, 1], m[0, 0]); 
                    break; 
                case EGimbleAxis.Roll:
                    AngleInRadian = Mathf.Atan2(m[2, 1], m[1, 1]); 
                    break; 
            }
        }

        public RotParams_Quaternion toQuaternionRotation() 
        {
            return new RotParams_Quaternion(RotationAxis, AngleInRadian);
        }
        
        public void ExtractValueFromQuaternion(RotParams_Quaternion q)
        {
            //TODO: Understand this
            switch (eAxis)
            {
                case EGimbleAxis.Yaw:
                    AngleInRadian = Mathf.Atan2(2.0f * (q.W * q.Z + q.X * q.Y), 1.0f - 2.0f * (q.Y * q.Y + q.Z * q.Z));
                    break; 
                case EGimbleAxis.Pitch:
                    AngleInRadian = Mathf.Atan2(2.0f * (q.W * q.Y - q.Z * q.X), 1.0f - 2.0f * (q.Y * q.Y + q.Z * q.Z));
                    break; 
                case EGimbleAxis.Roll:
                    AngleInRadian = Mathf.Atan2(2.0f * (q.W * q.X + q.Y*q.Z), 1.0f - 2.0f*(q.X * q.X + q.Y * q.Y)); 
                    break; 
            }
        }
        #endregion //Converters

        public string GetRotationName()
        {
            return GetRotationAxisName(eAxis); 
        }

        public static string GetRotationAxisName(EGimbleAxis eAxis)
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