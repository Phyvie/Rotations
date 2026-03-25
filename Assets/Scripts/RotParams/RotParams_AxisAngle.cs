using System;
using System.Collections.Generic;
using Extensions.MathExtensions;
using MathExtensions;
using Packages.MathExtensions;
using Unity.Properties;
using UnityEngine;

//ToDoZyKa RotParams_AxisAngle: adjust the getters and setters for RotationVector so that they only have one access point
namespace RotParams
{
    [Serializable]
    public class RotParams_AxisAngle : RotParams_Base, ISerializationCallbackReceiver
    {
        #region Variables
        [SerializeField] private AngleWithType typedAngle = new AngleWithType(EAngleType.Radian, 0);
        [SerializeField] private LockableVector _axis; 
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
                _axis.SetVector(new float[]{value.x, value.y, value.z});
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public bool EnforceNormalisation
        {
            get => _axis.GetAutoNormalizeToTargetMagnitude();
            set => _axis.SetAutoNormalizeToTargetMagnitude(value);
        }

        [CreateProperty]
        public float AxisX
        {
            get => _axis[0];
            set
            {
                _axis.SetValue(0, value, ELockableValueForceSetBehaviour.Force);
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
                _axis.SetValue(1, value, ELockableValueForceSetBehaviour.Force);
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
                _axis.SetValue(2, value, ELockableValueForceSetBehaviour.Force);
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

        public EAngleType AngleType
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
                _axis.SetAutoNormalizeToTargetMagnitude(true); 
                _axis.SetTargetMagnitude(1);
                this.NormalisedAxis = rotParams.NormalisedAxis;
                this.AngleInRadian = rotParams.AngleInRadian; 
            }
            else
            {
                CopyValues(toCopy.ToAxisAngleParams());
            }
        }

        public RotParams_AxisAngle() : this(Vector3.right, 0) { }
        
        public RotParams_AxisAngle(RotParams_AxisAngle toCopy) : this(toCopy.NormalisedAxis, toCopy.AngleInRadian) { }
        
        public RotParams_AxisAngle(Vector3 inRotationVectorInRadian, bool autoNormalizeToTargetNormalisation = true, float targetMagnitude = 1) 
            : this(
                inRotationVectorInRadian, 
                inRotationVectorInRadian.magnitude
            )
        { }

        public RotParams_AxisAngle(Vector3 inAxis, float inAngleInRadian)
        {
            float[] axis;
            if (inAxis.sqrMagnitude > 0.0001)
            {
                inAxis = inAxis.normalized;
                axis = new float[]{ inAxis.x, inAxis.y, inAxis.z }; 
            }
            else
            {
                axis = new float[] { 1, 0, 0 }; 
            }
            _axis = LockableVector.SafeCreateLockableVector(axis, new bool[]{false, false, false}, 1.0f, true);
            if (_axis == null)
            {
                Debug.LogWarning("Can't create AxisAngleRotation with given axis. Axis is set to default (1,0,0).");
                _axis = LockableVector.SafeCreateLockableVector(new float[]{1, 0, 0}, new bool[]{false, false, false}, 1, true);
            }
            AngleInRadian = inAngleInRadian;
        }
        #endregion //Constructors
        
        #region Converters

        public override RotParams_Base ToSelfTypeCopy(RotParams_Base toConvert)
        {
            return toConvert.ToAxisAngleParams(); 
        }

        public override void ToSelfType(RotParams_Base toConvert)
        {
            toConvert.ToSelfType(this);
        }

        public override RotParams_EulerAngles ToEulerParams()
        {
            return ToEulerParams(new RotParams_EulerAngles());
        }

        public override RotParams_Quaternion ToQuaternionParams()
        {
            return ToQuaternionParams(new RotParams_Quaternion());
        }

        public override RotParams_Matrix ToMatrixParams()
        {
            return ToMatrixParams(new RotParams_Matrix(new float[3, 3]));
        }

        public override RotParams_AxisAngle ToAxisAngleParams()
        {
            return ToAxisAngleParams(new RotParams_AxisAngle(Vector3.right, 0));
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_EulerAngles ToEulerParams(RotParams_EulerAngles eulerParams)
        {
            ToMatrixParams().ToEulerParams(eulerParams);
            return eulerParams;
        }

        public override RotParams_Quaternion ToQuaternionParams(RotParams_Quaternion quaternionParams)
        {
            float halfAngle = AngleInRadian * 0.5f;
            float s = Mathf.Sin(halfAngle);
            float c = Mathf.Cos(halfAngle);
            quaternionParams.W = c;
            quaternionParams.X = NormalisedAxis.x * s;
            quaternionParams.Y = NormalisedAxis.y * s;
            quaternionParams.Z = NormalisedAxis.z * s;
            return quaternionParams;
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_Matrix ToMatrixParams(RotParams_Matrix matrixParams)
        {
            float x = NormalisedAxis.x;
            float y = NormalisedAxis.y;
            float z = NormalisedAxis.z;

            float cosTheta = Mathf.Cos(AngleInRadian);
            float sinTheta = Mathf.Sin(AngleInRadian);
            float oneMinusCosTheta = 1 - cosTheta;

            matrixParams[0, 0] = cosTheta + x * x * oneMinusCosTheta;
            matrixParams[1, 0] = x * y * oneMinusCosTheta + z * sinTheta;
            matrixParams[2, 0] = x * z * oneMinusCosTheta - y * sinTheta;

            matrixParams[0, 1] = x * y * oneMinusCosTheta - z * sinTheta;
            matrixParams[1, 1] = cosTheta + y * y * oneMinusCosTheta;
            matrixParams[2, 1] = y * z * oneMinusCosTheta + x * sinTheta;

            matrixParams[0, 2] = x * z * oneMinusCosTheta + y * sinTheta;
            matrixParams[1, 2] = y * z * oneMinusCosTheta - x * sinTheta;
            matrixParams[2, 2] = cosTheta + z * z * oneMinusCosTheta;

            return matrixParams;
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_AxisAngle ToAxisAngleParams(RotParams_AxisAngle axisAngleParams)
        {
            axisAngleParams.CopyValues(new RotParams_AxisAngle(RotationVectorInRadian));
            return axisAngleParams;
        }
        #endregion //Converters

        #region operators
        #region comparison
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            RotParams_AxisAngle other = (RotParams_AxisAngle)obj;
            return this == other;
        }

        public static bool operator ==(RotParams_AxisAngle a, RotParams_AxisAngle b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (((object)a == null) || ((object)b == null)) return false;

            return Mathf.Abs(a.AngleInRadian - b.AngleInRadian) < 0.0001f && 
                   Vector3.Distance(a.NormalisedAxis, b.NormalisedAxis) < 0.0001f;
        }

        public static bool operator !=(RotParams_AxisAngle a, RotParams_AxisAngle b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (NormalisedAxis, AngleInRadian).GetHashCode();
        }
        #endregion comparison
        
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
        #endregion operators
        
        #region Functions

        public override void GetValuesFromUnityQuaternion(Quaternion unityQuaternion)
        {
            float w = Mathf.Clamp(unityQuaternion.w, -1.0f, 1.0f);
            AngleInRadian = 2.0f * Mathf.Acos(w);
            float s = Mathf.Sqrt(1.0f - w * w);

            if (s < 0.0001f)
            {
                NormalisedAxis = Vector3.right;
            }
            else
            {
                NormalisedAxis = new Vector3(unityQuaternion.x / s, unityQuaternion.y / s, unityQuaternion.z / s);
            }
        }

        public override string ToString()
        {
            return $"({_axis}, {AngleInDegrees})";
        }
        
        public override void ResetToIdentity()
        {
            NormalisedAxis = Vector3.right;
            AngleInRadian = 0;
            _axis.SetTargetMagnitude(1.0f); 
            EnforceNormalisation = true;
        }
        
        public override RotParams_Base GetIdentity()
        {
            return new RotParams_AxisAngle(Vector3.right, 0);
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

        public void OnBeforeSerialize()
        {
            if (_axis.Dimensions != 3)
            {
                Debug.LogWarning("AxisAngleRotation: Axis vector has invalid dimensions. Resetting to identity.");
                _axis = LockableVector.SafeCreateLockableVector(new float[]{1, 0, 0}, new bool[]{false, false, false}, 1, true);
            }
        }

        public void OnAfterDeserialize()
        {
            if (_axis.Dimensions != 3)
            {
                Debug.LogWarning("AxisAngleRotation: Axis vector has invalid dimensions. Resetting to identity.");
                _axis = LockableVector.SafeCreateLockableVector(new float[]{1, 0, 0}, new bool[]{false, false, false}, 1, true);
            }
        }
    }
}
