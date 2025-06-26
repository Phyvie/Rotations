using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace RotParams
{
    [Serializable]
    public enum EGimbalAxis
    {
        Yaw, 
        Pitch, 
        Roll
    }

    [Serializable]
    public class _RotParams_EulerAngleGimbalRing : INotifyPropertyChanged
    {
        #region Variables
        [SerializeField] private EGimbalAxis eAxis;

        public EGimbalAxis EAxis
        {
            get => eAxis;
            set
            {
                eAxis = value;
                OnPropertyChanged(nameof(EAxis));
            }
        }
        
        public Vector3 RotationAxis => eAxis switch
        {
            EGimbalAxis.Yaw => Vector3.up,
            EGimbalAxis.Pitch => Vector3.right,
            EGimbalAxis.Roll => Vector3.forward,
            _ => Vector3.zero
        };

        [SerializeField] private AngleWithType typedAngle;

        [CreateProperty]
        public AngleWithType TypedAngle
        {
            get => typedAngle;
            set
            {
                typedAngle = value;
                OnPropertyChanged(nameof(TypedAngle));
                
                
            }
        }
        
        public static string NameOfAngle => nameof(typedAngle); //for PropertyDrawer

        public float AngleInRadian
        {
            get => typedAngle.AngleInRadian;
            set
            {
                typedAngle.AngleInRadian = value;
                OnPropertyChanged(nameof(AngleInRadian));
            }
        }

        #endregion
        
        #region Constructors
        private _RotParams_EulerAngleGimbalRing()
        {
            eAxis = EGimbalAxis.Yaw;
            typedAngle = new AngleWithType(AngleType.Degree, 0);
        }
        
        public _RotParams_EulerAngleGimbalRing(EGimbalAxis inEAxis, float angleInRadian) : this(inEAxis, AngleType.Radian, angleInRadian)
        {
        }

        public _RotParams_EulerAngleGimbalRing(EGimbalAxis inEAxis, AngleType angleType, float angle) : this(inEAxis, new AngleWithType(angleType, angle))
        {
        }

        public _RotParams_EulerAngleGimbalRing(EGimbalAxis inEAxis, AngleWithType typedAngle)
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
            return new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Yaw, AngleType.Degree, 90); 
        }
        public static _RotParams_EulerAngleGimbalRing Pitch(RotParams_EulerAngles parent)
        {
            return new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Pitch, AngleType.Degree, 90); 
        }
        public static _RotParams_EulerAngleGimbalRing Roll(RotParams_EulerAngles parent)
        {
            return new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Roll, AngleType.Degree, 90); 
        }
        #endregion //Constructors
        
        #region Converters
        public RotParams_Matrix toMatrixRotation()
        {
            return eAxis switch
            {
                EGimbalAxis.Yaw => new RotParams_Matrix(new float[3,3]
                {
                    { Mathf.Cos(AngleInRadian),  0, Mathf.Sin(AngleInRadian) },
                    {          0,                1,              0           },
                    { -Mathf.Sin(AngleInRadian), 0, Mathf.Cos(AngleInRadian) }
                }),
                EGimbalAxis.Pitch => new RotParams_Matrix(new float[3,3]
                {
                    { Mathf.Cos(AngleInRadian), -Mathf.Sin(AngleInRadian),  0 },
                    { Mathf.Sin(AngleInRadian),  Mathf.Cos(AngleInRadian),  0 },
                    {           0,                          0,              1 }
                }),
                EGimbalAxis.Roll => new RotParams_Matrix(new float[3,3]
                {
                    { 1,              0,                       0            },
                    { 0, Mathf.Cos(AngleInRadian), -Mathf.Sin(AngleInRadian) },
                    { 0, Mathf.Sin(AngleInRadian), Mathf.Cos(AngleInRadian) }
                }),
                _ => throw new InvalidEnumArgumentException()
            }; 
        }

        /*
        public void ExtractValueFromMatrix(RotParams_Matrix m) 
        {
            switch (eAxis)
            {
                case EGimbalAxis.Yaw:
                    AngleInRadian = Mathf.Atan2(m[2, 0], m[0, 0]); 
                    break; 
                case EGimbalAxis.Pitch:
                    AngleInRadian = Mathf.Atan2(m[0, 1], m[0, 0]); 
                    break; 
                case EGimbalAxis.Roll:
                    AngleInRadian = Mathf.Atan2(m[2, 1], m[1, 1]); 
                    break; 
            }
        }
        */

        public RotParams_Quaternion toQuaternionRotation() 
        {
            return new RotParams_Quaternion(RotationAxis, AngleInRadian);
        }
        
        /*
        public void ExtractValueFromQuaternion(RotParams_Quaternion q)
        {
            //TODO: Understand this
            switch (eAxis)
            {
                case EGimbalAxis.Yaw:
                    AngleInRadian = Mathf.Atan2(2.0f * (q.W * q.Z + q.X * q.Y), 1.0f - 2.0f * (q.Y * q.Y + q.Z * q.Z));
                    break; 
                case EGimbalAxis.Pitch:
                    AngleInRadian = Mathf.Atan2(2.0f * (q.W * q.Y - q.Z * q.X), 1.0f - 2.0f * (q.Y * q.Y + q.Z * q.Z));
                    break; 
                case EGimbalAxis.Roll:
                    AngleInRadian = Mathf.Atan2(2.0f * (q.W * q.X + q.Y*q.Z), 1.0f - 2.0f*(q.X * q.X + q.Y * q.Y)); 
                    break; 
            }
        }
        */
        #endregion //Converters

        public string GetRotationName()
        {
            return GetRotationAxisName(eAxis); 
        }

        public static string GetRotationAxisName(EGimbalAxis eAxis)
        {
            return eAxis switch
            {
                EGimbalAxis.Yaw => "Yaw",
                EGimbalAxis.Pitch => "Pitch",
                EGimbalAxis.Roll => "Roll",
                _ => throw new ArgumentOutOfRangeException()
            }; 
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}