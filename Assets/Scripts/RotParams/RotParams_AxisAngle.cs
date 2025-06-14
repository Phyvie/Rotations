using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotParams
{
    [Serializable]
    public class RotParams_AxisAngle : RotParams
    {
        [SerializeField] private Vector3 axis;
        [SerializeField] private AngleWithType typedAngle = new AngleWithType(AngleType.Radian, 0); 
        
        [CreateProperty]
        public Vector3 NormalisedAxis
        {
            get => axis;
            set
            {
                axis = value;
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AngleInRadian
        {
            get => typedAngle.AngleInRadian;
            set
            {
                typedAngle.AngleInRadian = value;
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AngleInDegrees
        {
            get => typedAngle.AngleInDegree;
            set
            {
                typedAngle.AngleInDegree = value;
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AngleInCircleParts
        {
            get => typedAngle.AngleInCircleParts;
            set
            {
                typedAngle.AngleInCircleParts = value;
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AngleInCurrentUnit
        {
            get => typedAngle.AngleInCurrentUnit;
            set
            {
                typedAngle.AngleInCurrentUnit = value;
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public Vector3 RotationVectorInRadian
        {
            get => axis * AngleInRadian;
            set
            {
                axis = value.normalized;
                AngleInRadian = value.magnitude; 
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public Vector3 RotationVectorInDegrees
        {
            get => axis * AngleInDegrees;
            set
            {
                axis = value.normalized;
                AngleInRadian = value.magnitude; 
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public Vector3 RotationVectorInCircleParts
        {
            get => axis * AngleInCircleParts;
            set
            {
                axis = value.normalized;
                AngleInCircleParts = value.magnitude; 
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public Vector3 RotationVectorInCurrentUnit
        {
            get => axis * typedAngle; 
            set 
            {
                axis = value.normalized;
                typedAngle.AngleInCurrentUnit = value.magnitude;
                OnPropertyChanged();
            }
        }
        
        public RotParams_AxisAngle()
        {
            
        }
        
        #region Constructors
        public RotParams_AxisAngle(Vector3 inRotationVectorInRadian)
        {
            RotationVectorInRadian = inRotationVectorInRadian; 
        }

        public RotParams_AxisAngle(Vector3 inAxis, float inAngle)
        {
            inAxis = inAxis.normalized;
            RotationVectorInRadian = inAxis * inAngle;
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
            RotParams_Quaternion asQuat = new RotParams_Quaternion(NormalisedAxis, AngleInRadian); 
            return asQuat; 
        }

        //AxisAngleRotation.ToMatrixRotation() is the same as ToQuaternionRotation().ToMatrixRotation()
        public override RotParams_Matrix ToMatrixRotation()
        {
            //TODO: understand this formula (maybe visualise?) and correct it to be left-handed (in case it's right-handed)
            float x = NormalisedAxis.x;
            float y = NormalisedAxis.y;
            float z = NormalisedAxis.z;
        
            float cosTheta = Mathf.Cos(AngleInRadian);
            float sinTheta = Mathf.Sin(AngleInRadian);
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
            return new RotParams_AxisAngle(RotationVectorInRadian); 
        }

        public override void ResetToIdentity()
        {
            NormalisedAxis = Vector3.up;
            AngleInRadian = 0;
        }

        #endregion //Converters

        public override Vector3 RotateVector(Vector3 inVector)
        {
            //Rodrigues' rotation formula (righthand-version) TODO: Try to understand it, visualise it and make it left-handed
            Vector3 rotatedVector = 
                inVector * Mathf.Cos(AngleInRadian) + 
                Vector3.Cross(NormalisedAxis, inVector) * Mathf.Sin(AngleInRadian) + 
                NormalisedAxis * Vector3.Dot(NormalisedAxis, inVector) * (1 - Mathf.Cos(AngleInRadian));

            return rotatedVector; 
        }
    }
}
