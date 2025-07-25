using System;
using System.Collections.Generic;
using Extensions.MathExtensions;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

//-ZyKa adjust the getters and setters for RotationVector so that they only have one access point
namespace RotParams
{
    [Serializable]
    public class RotParams_AxisAngle : RotParams_Base
    {
        #region Variables
        [SerializeField] private AngleWithType typedAngle = new AngleWithType(AngleType.Radian, 0);

        [SerializeField] private LockableVector _axis = new LockableVector(new List<LockableFloat>()
        {
            new LockableFloat(0, false), 
            new LockableFloat(0, false), 
            new LockableFloat(0, false)
        }); 
        
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
            get => new Vector3(AxisX, AxisY, AxisZ);
            set
            {
                value = value.normalized;
                _axis.SetVector(new List<float>(){value.x, value.y, value.z});
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisX
        {
            get => _axis[0];
            set
            {
                _axis.SetFloatValue(0, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public bool XLocked
        {
            get => _axis.GetLock(0); 
            set => _axis.SetLock(0, value);
        }

        [CreateProperty]
        public float AxisY
        {
            get => _axis[1];
            set
            {
                _axis.SetFloatValue(1, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public bool YLocked
        {
            
            get => _axis.GetLock(1); 
            set => _axis.SetLock(1, value);
        }

        [CreateProperty]
        public float AxisZ
        {
            get => _axis[2];
            set
            {
                _axis.SetFloatValue(2, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public bool ZLocked
        {
            
            get => _axis.GetLock(2); 
            set => _axis.SetLock(2, value);
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
                if (value.sqrMagnitude > 0)
                {
                    NormalisedAxis = value.normalized;
                    AngleInRadian = value.magnitude; 
                }
                else
                {
                    NormalisedAxis = Vector3.zero;
                    AngleInRadian = 0; 
                }
                OnPropertyChanged();
            }
        }

        public Vector3 RotationVectorInCurrentUnit
        {
            get => NormalisedAxis * typedAngle; 
            set 
            {
                if ((value - RotationVectorInCurrentUnit).sqrMagnitude < 0.001f)
                {
                    return; 
                }
                if (value.sqrMagnitude > 0)
                {
                    typedAngle.AngleInCurrentUnit = value.magnitude;
                    NormalisedAxis = value.normalized;
                }
                else
                {
                    typedAngle.AngleInCurrentUnit = 0;
                    NormalisedAxis = Vector3.zero;
                }
                OnPropertyChanged();
            }
        }

        public AngleType AngleType
        {
            get => typedAngle.angleType;
            set => typedAngle.angleType = value;
        }
        #endregion Properties
        
        #region Constructors
        public override void CopyValues(RotParams_Base toCopy)
        {
            if (toCopy is RotParams_AxisAngle rotParams)
            {
                this.NormalisedAxis = rotParams.NormalisedAxis;
                this.AngleInRadian = rotParams.AngleInRadian; 
            }
            else
            {
                CopyValues(toCopy.ToAxisAngleParams());
            }
        }

        public RotParams_AxisAngle(RotParams_AxisAngle toCopy) : this(toCopy.NormalisedAxis, toCopy.AngleInRadian)
        {
        }
        
        public RotParams_AxisAngle(bool enforceNormalisation = true, float targetLength = 1)
        {
            _axis.EnforceLength = enforceNormalisation;
            _axis.TargetLength = targetLength;
        }
        
        public RotParams_AxisAngle(Vector3 inRotationVectorInRadian, bool enforceNormalisation = true, float targetLength = 1) : 
            this(
            inRotationVectorInRadian, 
            inRotationVectorInRadian.magnitude, 
            enforceNormalisation, 
            targetLength
            )
        {
        }

        public RotParams_AxisAngle(Vector3 inAxis, float inAngleInRadian, bool enforceNormalisation = true, float targetLength = 1)
        {
            List<float> axis;
            if (inAxis.sqrMagnitude > 0.0001)
            {
                inAxis = inAxis.normalized;
                axis = new List<float>(){ inAxis.x, inAxis.y, inAxis.z }; 
            }
            else
            {
                axis = new List<float>() { 1, 0, 0 }; 
            }
            _axis.SetVector(axis);
            AngleInRadian = inAngleInRadian;
            
            _axis.EnforceLength = enforceNormalisation;
            _axis.TargetLength = targetLength;
        }
        #endregion //Constructors
        
        #region Converters
        //AxisAngleRotation.ToEulerAngle() is the same as ToQuaternionRotation().ToEulerAngleRotation()
        public override RotParams_Base ToSelfType(RotParams_Base toConvert)
        {
            return toConvert.ToAxisAngleParams(); 
        }

        public override  RotParams_EulerAngles ToEulerParams()
        {
            Debug.LogWarning("AxisAngleRotation.ToEulerAngle() is the same as ToQuaternionRotation().ToEulerAngleRotation()");
            return ToQuaternionParams().ToEulerParams(); 
        }

        public override RotParams_Quaternion ToQuaternionParams()
        {
            RotParams_Quaternion asQuat = new RotParams_Quaternion(NormalisedAxis, AngleInRadian); 
            return asQuat; 
        }

        //AxisAngleRotation.ToMatrixRotation() is the same as ToQuaternionRotation().ToMatrixRotation()
        public override RotParams_Matrix ToMatrixParams()
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

        public override RotParams_AxisAngle ToAxisAngleParams()
        {
            return new RotParams_AxisAngle(RotationVectorInRadian); 
        }
        #endregion //Converters

        #region operators
        public static RotParams_AxisAngle operator+(
            RotParams_AxisAngle rotParamsA, RotParams_AxisAngle rotParamsB)
        {
            return new RotParams_AxisAngle(
                rotParamsA.RotationVectorInRadian + rotParamsB.RotationVectorInRadian, 
                false); 
        }

        public static RotParams_AxisAngle operator *(RotParams_AxisAngle rotParamsA, float alpha)
        {
            return new RotParams_AxisAngle(rotParamsA.RotationVectorInRadian * alpha, false);
        }

        public static RotParams_AxisAngle operator *(float alpha, RotParams_AxisAngle rotParamsA)
        {
            return rotParamsA * alpha; 
        }
        #endregion operators
        
        #region Functions
        public override string ToString()
        {
            return $"({_axis}, {AngleInDegrees})";
        }
        
        public override void ResetToIdentity()
        {
            NormalisedAxis = Vector3.up;
            AngleInRadian = 0;
        }
        
        public override RotParams_Base GetIdentity()
        {
            return new RotParams_AxisAngle();
        }

        public override RotParams_Base GetInverse()
        {
            return new RotParams_AxisAngle(-RotationVectorInRadian); 
        }

        protected override RotParams_Base Concatenate_Implementation(RotParams_Base otherRotation, bool otherFirst)
        {
            Debug.LogWarning("AxisAngle cannot be arithmetically concatenated; converting to Quaternion for concatenation calculation"); 
            
            RotParams_Quaternion thisQuat = ToQuaternionParams();
            RotParams_Quaternion otherQuat = otherRotation.ToQuaternionParams();

            return thisQuat.Concatenate(otherQuat, otherFirst).ToAxisAngleParams();
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
