using System;
using System.Collections.Generic;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class AxisAngleRotation : RotationParent
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

        public AxisAngleRotation()
        {
            
        }
        
        public AxisAngleRotation(Vector3 inAxisAngle, AngleType inAngleType)
        {
            angleType = inAngleType; 
            axisAngle = inAxisAngle; 
        }

        public AxisAngleRotation(Vector3 inAxis, float inAngle, AngleType inAngleType)
        {
            angleType = inAngleType; 
            inAxis = inAxis.normalized;
            axisAngle = inAxis * inAngle;
        }
        
        //AxisAngleRotation.ToEulerAngle() is the same as ToQuaternionRotation().ToEulerAngleRotation()
        public override  EulerAngleRotation ToEulerAngleRotation()
        {
            Debug.LogWarning("AxisAngleRotation.ToEulerAngle() is the same as ToQuaternionRotation().ToEulerAngleRotation()");
            return ToQuaternionRotation().ToEulerAngleRotation(); 
        }

        public override QuaternionRotation ToQuaternionRotation()
        {
            return new QuaternionRotation(axis, angle); 
        }

        //AxisAngleRotation.ToMatrixRotation() is the same as ToQuaternionRotation().ToMatrixRotation()
        public override MatrixRotation ToMatrixRotation()
        {
            //TODO: understand this formula (maybe visualise?) and correct it to be left-handed (in case it's righthanded)
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
        
            float cosTheta = Mathf.Cos(angle);
            float sinTheta = Mathf.Sin(angle);
            float oneMinusCosTheta = 1 - cosTheta;

            MatrixRotation matrixRotation = new MatrixRotation(new float[3, 3]); 
        
            matrixRotation[0, 0] = cosTheta + x * x * oneMinusCosTheta;
            matrixRotation[0, 1] = x * y * oneMinusCosTheta - z * sinTheta;
            matrixRotation[0, 2] = x * z * oneMinusCosTheta + y * sinTheta;

            matrixRotation[1, 0] = y * x * oneMinusCosTheta + z * sinTheta;
            matrixRotation[1, 1] = cosTheta + y * y * oneMinusCosTheta;
            matrixRotation[1, 2] = y * z * oneMinusCosTheta - x * sinTheta;

            matrixRotation[2, 0] = z * x * oneMinusCosTheta - y * sinTheta;
            matrixRotation[2, 1] = z * y * oneMinusCosTheta + x * sinTheta;
            matrixRotation[2, 2] = cosTheta + z * z * oneMinusCosTheta;

            return matrixRotation; 
        }

        public override AxisAngleRotation ToAxisAngleRotation()
        {
            return new AxisAngleRotation(axisAngle, angleType); 
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
