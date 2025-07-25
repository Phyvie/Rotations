using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RotParams
{
    [Serializable]
    public abstract class RotParams_Base : INotifyPropertyChanged // -ZyKa check out Source Generators (e.g. CommunityToolkit.MVVM) for IDataSourceViewHashProvider Hash Generation in child classes
    {
        public abstract RotParams_Base ToSelfType(RotParams_Base toConvert);
        public abstract void CopyValues(RotParams_Base toCopy); 
        public abstract RotParams_EulerAngles ToEulerParams(); 
        public abstract RotParams_Quaternion ToQuaternionParams(); 
        public abstract RotParams_Matrix ToMatrixParams(); 
        public abstract RotParams_AxisAngle ToAxisAngleParams();

        public bool DoesTypeMatch(RotParams_Base otherRotation)
        {
            return otherRotation.GetType() == GetType();
        }
        
        public abstract RotParams_Base GetIdentity();
        public abstract RotParams_Base GetInverse();

        public RotParams_Base Concatenate(RotParams_Base otherRotation, bool otherFirst = false)
        {
            if (!DoesTypeMatch(otherRotation))
            {
                Debug.LogError("Cannot Concatenate Rotations of different type");
                return this; 
            }
            else
            {
                return Concatenate_Implementation(otherRotation, otherFirst); 
            }
        }

        protected abstract RotParams_Base Concatenate_Implementation(RotParams_Base otherRotation, bool otherFirst = false); 
        
        public abstract void ResetToIdentity();
        
        public abstract Vector3 RotateVector(Vector3 inVector);

        public static RotParams_Base FromUnityQuaternion(Quaternion unityQuaternion, System.Type type)
        {
            RotParams_Quaternion rotParams_Quaternion = new RotParams_Quaternion(unityQuaternion.w, unityQuaternion.x, unityQuaternion.y, unityQuaternion.z);
            switch (type)
            {
                case Type t when t == typeof(RotParams_AxisAngle): 
                    return rotParams_Quaternion.ToAxisAngleParams();
                
                case Type t when t == typeof(RotParams_Quaternion): 
                    return rotParams_Quaternion.ToQuaternionParams();
                
                case Type t when t == typeof(RotParams_EulerAngles): 
                    return rotParams_Quaternion.ToEulerParams();
                
                case Type t when t == typeof(RotParams_Matrix): 
                    return rotParams_Quaternion.ToMatrixParams();
                
                default: throw new ArgumentException("Invalid rotation type");
            }
        }
        
        public Quaternion ToUnityQuaternion()
        {
            RotParams_Quaternion asQuat = ToQuaternionParams();
            return new Quaternion(asQuat.X, asQuat.Y, asQuat.Z, asQuat.W);
        }

        public override abstract string ToString(); 
        
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
        #endregion INotifyPropertyChanged
    }
}
