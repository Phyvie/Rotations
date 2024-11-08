using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public abstract class RotationType
    {
        public abstract EulerAngleRotationDeprecated ToEulerAngleRotation(); 
        public abstract QuaternionRotation ToQuaternionRotation(); 
        public abstract MatrixRotation ToMatrixRotation(); 
        public abstract AxisAngleRotation ToAxisAngleRotation();
        
        public abstract Vector3 RotateVector(Vector3 inVector); 
    }
}
