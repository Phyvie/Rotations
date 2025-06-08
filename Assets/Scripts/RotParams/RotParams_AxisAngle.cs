using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotParams
{
    //-ZyKa AngleTypes
    [Serializable]
    public class RotParams_AxisAngle : RotParams
    {
        [SerializeField] private Vector3 rotationVector; 
        
        [CreateProperty]
        public Vector3 RotationVector
        {
            get => rotationVector;
            set => rotationVector = value;
        }
        
        [CreateProperty]
        public Vector3 Axis
        {
            get => RotationVector.normalized;
            set => RotationVector = value * Angle;
        }

        [CreateProperty]
        public float Angle
        {
            get => RotationVector.magnitude;
            set => RotationVector = value * Axis; 
        }

        public RotParams_AxisAngle()
        {
            
        }
        
        #region Constructors
        public RotParams_AxisAngle(Vector3 inRotationVector)
        {
            RotationVector = inRotationVector; 
        }

        public RotParams_AxisAngle(Vector3 inAxis, float inAngle)
        {
            inAxis = inAxis.normalized;
            RotationVector = inAxis * inAngle;
        }
        #endregion //Constructors
        
        #region Converters
        //AxisAngleRotation.ToEulerAngle() is the same as ToQuaternionRotation().ToEulerAngleRotation()
        public override  RotParams_EulerAngles ToEulerAngleRotation()
        {
            Debug.LogWarning("AxisAngleRotation.ToEulerAngle() is the same as ToQuaternionRotation().ToEulerAngleRotation()");
            return ToQuaternionRotation().ToEulerAngleRotation(); 
        }

        public override RotParams_Quaternion ToQuaternionRotation()
        {
            return new RotParams_Quaternion(Axis, Angle); 
        }

        //AxisAngleRotation.ToMatrixRotation() is the same as ToQuaternionRotation().ToMatrixRotation()
        public override RotParams_Matrix ToMatrixRotation()
        {
            //TODO: understand this formula (maybe visualise?) and correct it to be left-handed (in case it's right-handed)
            float x = Axis.x;
            float y = Axis.y;
            float z = Axis.z;
        
            float cosTheta = Mathf.Cos(Angle);
            float sinTheta = Mathf.Sin(Angle);
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
            return new RotParams_AxisAngle(RotationVector); 
        }
        #endregion //Converters

        public override Vector3 RotateVector(Vector3 inVector)
        {
            //Rodrigues' rotation formula (righthand-version) TODO: Try to understand it, visualise it and make it left-handed
            Vector3 rotatedVector = 
                inVector * Mathf.Cos(Angle) + 
                Vector3.Cross(Axis, inVector) * Mathf.Sin(Angle) + 
                Axis * Vector3.Dot(Axis, inVector) * (1 - Mathf.Cos(Angle));

            return rotatedVector; 
        }
    }
}
