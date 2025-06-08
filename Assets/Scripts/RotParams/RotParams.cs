using System;
using UnityEngine;

namespace RotParams
{
    [Serializable]
    public abstract class RotParams // -ZyKa check out Source Generators (e.g. CommunityToolkit.MVVM) for IDataSourceViewHashProvider Hash Generation in child classes
    {
        public abstract RotParams_EulerAngles ToEulerAngleRotation(); 
        public abstract RotParams_Quaternion ToQuaternionRotation(); 
        public abstract RotParams_Matrix ToMatrixRotation(); 
        public abstract RotParams_AxisAngle ToAxisAngleRotation();
        
        public abstract Vector3 RotateVector(Vector3 inVector);
    }
}
