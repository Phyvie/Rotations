using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public abstract class RotationParent
    {
        public abstract EulerAngleRotation ToEulerAngleRotation(); 
        public abstract QuaternionRotation ToQuaternionRotation(); 
        public abstract MatrixRotation ToMatrixRotation(); 
        public abstract AxisAngleRotation ToAxisAngleRotation();
        
        public abstract Vector3 RotateVector(Vector3 inVector); 
    }
}
