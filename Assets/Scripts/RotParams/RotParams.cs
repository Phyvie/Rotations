using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RotParams
{
    [Serializable]
    public abstract class RotParams : INotifyPropertyChanged // -ZyKa check out Source Generators (e.g. CommunityToolkit.MVVM) for IDataSourceViewHashProvider Hash Generation in child classes
    {
        public abstract RotParams_EulerAngles ToEulerAngleRotation(); 
        public abstract RotParams_Quaternion ToQuaternionRotation(); 
        public abstract RotParams_Matrix ToMatrixRotation(); 
        public abstract RotParams_AxisAngle ToAxisAngleRotation();
        public abstract void ResetToIdentity(); 
        
        public abstract Vector3 RotateVector(Vector3 inVector);

        public Quaternion ToUnityQuaternion()
        {
            RotParams_Quaternion asQuat = ToQuaternionRotation();
            return new Quaternion(asQuat.X, asQuat.Y, asQuat.Z, asQuat.W);
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
        #endregion INotifyPropertyChanged
    }
}
