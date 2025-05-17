using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class RotParams_AxisAngle : RotationParameterisation
    {
        [SerializeField] protected AngleType _angleType = AngleType.Radian;
        public AngleType angleType
        {
            get => _angleType;
            set => SetAngleType(value);
        }
        
        [SerializeField] private Vector3 axisAngle;
        
        private Vector3 axis => axisAngle.normalized; 
        private float angle => axisAngle.magnitude;
        private float angleInRadian => AngleType.ConvertAngle(angle, angleType, AngleType.Radian); 

        public RotParams_AxisAngle()
        {
            
        }
        
        public RotParams_AxisAngle(Vector3 inAxisAngle, AngleType inAngleType)
        {
            angleType = inAngleType; 
            axisAngle = inAxisAngle; 
        }

        public RotParams_AxisAngle(Vector3 inAxis, float inAngle, AngleType inAngleType)
        {
            angleType = inAngleType; 
            inAxis = inAxis.normalized;
            axisAngle = inAxis * inAngle;
        }
        
        //AxisAngleRotation.ToEulerAngle() is the same as ToQuaternionRotation().ToEulerAngleRotation()
        public override  RotParams_EulerAngle ToEulerAngleRotation()
        {
            Debug.LogWarning("AxisAngleRotation.ToEulerAngle() is the same as ToQuaternionRotation().ToEulerAngleRotation()");
            return ToQuaternionRotation().ToEulerAngleRotation(); 
        }

        public override RotParams_Quaternion ToQuaternionRotation()
        {
            return new RotParams_Quaternion(axis, angle); 
        }

        //AxisAngleRotation.ToMatrixRotation() is the same as ToQuaternionRotation().ToMatrixRotation()
        public override RotParams_Matrix ToMatrixRotation()
        {
            //TODO: understand this formula (maybe visualise?) and correct it to be left-handed (in case it's righthanded)
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
        
            float cosTheta = Mathf.Cos(angle);
            float sinTheta = Mathf.Sin(angle);
            float oneMinusCosTheta = 1 - cosTheta;

            RotParams_Matrix rotParamsMatrix = new RotParams_Matrix(new float[3, 3]); 
        
            rotParamsMatrix[0, 0] = cosTheta + x * x * oneMinusCosTheta;
            rotParamsMatrix[0, 1] = x * y * oneMinusCosTheta - z * sinTheta;
            rotParamsMatrix[0, 2] = x * z * oneMinusCosTheta + y * sinTheta;

            rotParamsMatrix[1, 0] = y * x * oneMinusCosTheta + z * sinTheta;
            rotParamsMatrix[1, 1] = cosTheta + y * y * oneMinusCosTheta;
            rotParamsMatrix[1, 2] = y * z * oneMinusCosTheta - x * sinTheta;

            rotParamsMatrix[2, 0] = z * x * oneMinusCosTheta - y * sinTheta;
            rotParamsMatrix[2, 1] = z * y * oneMinusCosTheta + x * sinTheta;
            rotParamsMatrix[2, 2] = cosTheta + z * z * oneMinusCosTheta;

            return rotParamsMatrix; 
        }

        public override RotParams_AxisAngle ToAxisAngleRotation()
        {
            return new RotParams_AxisAngle(axisAngle, angleType); 
        }

        public void SetAngleType(AngleType value)
        {
            axisAngle = axisAngle * (float) (value.UnitMultiplier / angleType.UnitMultiplier);
            _angleType = value; 
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            //Rodrigues' rotation formula (righthand-version) TODO: Try to understand it, visualise it and make it left-handed
            float _angleInRadian = angleInRadian; 
            Vector3 rotatedVector = 
                inVector * Mathf.Cos(_angleInRadian) + 
                Vector3.Cross(axis, inVector) * Mathf.Sin(_angleInRadian) + 
                axis * Vector3.Dot(axis, inVector) * (1 - Mathf.Cos(_angleInRadian));

            return rotatedVector; 
        }
    }
}
