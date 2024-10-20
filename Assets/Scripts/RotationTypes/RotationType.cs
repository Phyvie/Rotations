using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public abstract class RotationType
    {
        [SerializeField] protected AngleType _angleType = AngleType.Radian;

        public AngleType angleType
        {
            get => _angleType;
            set => SetAngleType(value);
        }

        public abstract EulerAngleRotation ToEulerAngleRotation(); 
        public abstract QuaternionRotation ToQuaternionRotation(); 
        public abstract MatrixRotation ToMatrixRotation(); 
        public abstract AxisAngleRotation ToAxisAngleRotation();

        public abstract void SetAngleType(AngleType value); 
        
        public abstract Vector3 RotateVector(Vector3 inVector); 
    }
}
