using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public abstract class RotationParameterisation
    {
        public abstract RotParams_EulerAngles ToEulerAngleRotation(); 
        public abstract RotParams_Quaternion ToQuaternionRotation(); 
        public abstract RotParams_Matrix ToMatrixRotation(); 
        public abstract RotParams_AxisAngle ToAxisAngleRotation();
        
        public abstract Vector3 RotateVector(Vector3 inVector);
    }
}
