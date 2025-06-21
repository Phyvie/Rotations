using System;
using System.Collections.Generic;
using Extensions.MathExtensions;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotParams
{
    [Serializable]
    public class RotParams_AxisAngle : RotParams
    {
        #region Variables
        [SerializeField] private AngleWithType typedAngle = new AngleWithType(AngleType.Radian, 0);

        [SerializeField] private LockableFloat _x = new LockableFloat(0, false); 
        [SerializeField] private LockableFloat _y = new LockableFloat(0, false); 
        [SerializeField] private LockableFloat _z = new LockableFloat(0, false);
        [FormerlySerializedAs("xyzVector")] [SerializeField] private LockableVector xyzLockableVector; 
        #endregion Variables
        
        #region Properties
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
        
        public float AngleInRadian
        {
            get => typedAngle.AngleInRadian;
            set
            {
                typedAngle.AngleInRadian = value;
                OnPropertyChanged();
            }
        }

        public float AngleInDegrees
        {
            get => typedAngle.AngleInDegree;
            set
            {
                typedAngle.AngleInDegree = value;
                OnPropertyChanged();
            }
        }

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
        public Vector3 NormalisedAxis
        {
            get => new Vector3(_x, _y, _z);
            set
            {
                value = value.normalized;
                xyzLockableVector.SetVector(new List<float>(){value.x, value.y, value.z});
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisX
        {
            get => _x;
            set
            {
                xyzLockableVector.SetFloatValue(_x, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisY
        {
            get => _y;
            set
            {
                xyzLockableVector.SetFloatValue(_y, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public float AxisZ
        {
            get => _z;
            set
            {
                xyzLockableVector.SetFloatValue(_z, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float RotVecX
        {
            get => RotationVectorInCurrentUnit.x;
            set
            {
                Vector3 RotVec = RotationVectorInCurrentUnit; 
                Vector3 adjustedRotVec = new Vector3(value, RotVec.y, RotVec.z);
                RotationVectorInCurrentUnit = adjustedRotVec; 
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public float RotVecY
        {
            get => RotationVectorInCurrentUnit.y;
            set
            {
                Vector3 RotVec = RotationVectorInCurrentUnit; 
                Vector3 adjustedRotVec = new Vector3(RotVec.x, value, RotVec.z);
                RotationVectorInCurrentUnit = adjustedRotVec; 
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public float RotVecZ
        {
            get => RotationVectorInCurrentUnit.z;
            set
            {
                Vector3 RotVec = RotationVectorInCurrentUnit; 
                Vector3 adjustedRotVec = new Vector3(RotVec.x, RotVec.y, value);
                RotationVectorInCurrentUnit = adjustedRotVec; 
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public Vector3 RotationVectorInRadian
        {
            get => NormalisedAxis * AngleInRadian;
            set
            {
                NormalisedAxis = value.normalized;
                AngleInRadian = value.magnitude; 
                OnPropertyChanged();
            }
        }

        public Vector3 RotationVectorInDegrees
        {
            get => NormalisedAxis * AngleInDegrees;
            set
            {
                NormalisedAxis = value.normalized;
                AngleInRadian = value.magnitude; 
                OnPropertyChanged();
            }
        }
        
        public Vector3 RotationVectorInCircleParts
        {
            get => NormalisedAxis * AngleInCircleParts;
            set
            {
                NormalisedAxis = value.normalized;
                AngleInCircleParts = value.magnitude; 
                OnPropertyChanged();
            }
        }

        public Vector3 RotationVectorInCurrentUnit
        {
            get => NormalisedAxis * typedAngle; 
            set 
            {
                typedAngle.AngleInCurrentUnit = value.magnitude;
                NormalisedAxis = value.normalized;
                OnPropertyChanged();
            }
        }
        #endregion Properties
        
        #region Constructors
        public RotParams_AxisAngle()
        {
            xyzLockableVector = new LockableVector(new List<LockableFloat>(){_x, _y, _z}); 
        }
        
        public RotParams_AxisAngle(Vector3 inRotationVectorInRadian)
        {
            RotationVectorInRadian = inRotationVectorInRadian;
            xyzLockableVector = new LockableVector(new List<LockableFloat>(){_x, _y, _z}); 
        }

        public RotParams_AxisAngle(Vector3 inAxis, float inAngle)
        {
            inAxis = inAxis.normalized;
            RotationVectorInRadian = inAxis * inAngle;
            xyzLockableVector = new LockableVector(new List<LockableFloat>(){_x, _y, _z}); 
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
        #endregion //Converters

        #region Functions
        public override void ResetToIdentity()
        {
            NormalisedAxis = Vector3.up;
            AngleInRadian = 0;
        }
        
        public override Vector3 RotateVector(Vector3 inVector)
        {
            //Rodrigues' rotation formula (righthand-version) TODO: Try to understand it, visualise it and make it left-handed
            Vector3 rotatedVector = 
                inVector * Mathf.Cos(AngleInRadian) + 
                Vector3.Cross(NormalisedAxis, inVector) * Mathf.Sin(AngleInRadian) + 
                NormalisedAxis * Vector3.Dot(NormalisedAxis, inVector) * (1 - Mathf.Cos(AngleInRadian));

            return rotatedVector; 
        }
        
        public static RotParams_AxisAngle operator+(RotParams_AxisAngle rotParamsA, RotParams_AxisAngle rotParamsB)
        {
            return new RotParams_AxisAngle(rotParamsA.RotationVectorInRadian + rotParamsB.RotationVectorInRadian); 
        }

        public static RotParams_AxisAngle operator *(RotParams_AxisAngle rotParamsA, float alpha)
        {
            return new RotParams_AxisAngle(rotParamsA.RotationVectorInRadian * alpha);
        }

        public static RotParams_AxisAngle operator *(float alpha, RotParams_AxisAngle rotParamsA)
        {
            return rotParamsA * alpha; 
        }

        public static RotParams_AxisAngle LerpAxisAngle(RotParams_AxisAngle rotParamsA, RotParams_AxisAngle rotParamsB, float alpha)
        {
            return rotParamsA * (1 - alpha) + rotParamsB * alpha; 
        }

        public static RotParams_AxisAngle BezierCurve(RotParams_AxisAngle rotParamsA, RotParams_AxisAngle rotParamsB,
            RotParams_AxisAngle outA, RotParams_AxisAngle inB, float alpha)
        {
            float oneMinusAlpha = 1 - alpha;
            
            return 
                1*oneMinusAlpha*oneMinusAlpha*oneMinusAlpha * rotParamsA + 
                3*oneMinusAlpha*oneMinusAlpha*alpha * outA + 
                3*oneMinusAlpha*alpha*alpha * inB + 
                1*alpha*alpha*alpha * rotParamsB
                ; 
        }
        #endregion
    }
}
