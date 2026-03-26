using System;
using System.Collections.Generic;
using Extensions.MathExtensions;
using MathExtensions;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotParams
{
    [Serializable]
    public class RotParams_Quaternion : RotParams_Base
    {
        [FormerlySerializedAs("lockableWXYZ")] [SerializeField] private LockableVector _internalVector =
            new LockableVector(4);
        
        #region Properties
        #region WXYZValueAccessors
        [CreateProperty]
        public float W
        {
            get => _internalVector[0];
            set
            {
                _internalVector.SetValue(0, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(W));
            }
        }

        public bool WLocked
        {
            get => _internalVector.GetLock(0); 
            set => _internalVector.SetLock(0, value);
        }

        [CreateProperty]
        public float X
        {
            get => _internalVector[1];
            set
            {
                _internalVector.SetValue(1, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(W));
            }
        }

        public bool XLocked
        {
            get => _internalVector.GetLock(1); 
            set => _internalVector.SetLock(1, value);
        }
        
        [CreateProperty]
        public float Y
        {
            get => _internalVector[2];
            set
            {
                _internalVector.SetValue(2, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(W));
            }
        }

        public bool YLocked
        {
            get => _internalVector.GetLock(2); 
            set => _internalVector.SetLock(2, value);
        }
        
        [CreateProperty]
        public float Z
        {
            get => _internalVector[3];
            set
            {
                _internalVector.SetValue(3, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(W));
            }
        }

        public bool ZLocked
        {
            get => _internalVector.GetLock(3); 
            set => _internalVector.SetLock(3, value);
        }
        
        public float GetInternalValueByIndex(int index)
        {
            return _internalVector[index]; 
        }
        #endregion XYZWValueAccessors

        #region otherProperties
        private float SinHalfAngle => Mathf.Sqrt(1-W*W);

        public Vector3 NonNormalizedAxis
        {
            get => new Vector3(X, Y, Z).normalized;
        }
        
        [CreateProperty]
        public Vector3 NormalizedAxis
        {
            get
            {
                Vector3 axis = new Vector3(X, Y, Z);
                if (axis.sqrMagnitude > 0.0f)
                {
                    return axis.normalized;
                }
                else
                {
                    return Vector3.forward; 
                }
            }
            set
            {
                
                if (Mathf.Approximately((NormalizedAxis - value.normalized).sqrMagnitude, 0))
                {
                    return; 
                }

                Vector3 scaledAxis = value * SinHalfAngle; 
                _internalVector.SetValue(1, scaledAxis.x, ELockableValueForceSetBehaviour.Force);
                _internalVector.SetValue(2, scaledAxis.y, ELockableValueForceSetBehaviour.Force);
                _internalVector.SetValue(3, scaledAxis.z, ELockableValueForceSetBehaviour.Force);
                
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisX
        {
            get => new Vector3(X, Y, Z).normalized.x;
            set
            {
                _internalVector.SetValue(1, value * SinHalfAngle, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public float AxisY
        {
            get => new Vector3(X, Y, Z).normalized.y;
            set
            {
                _internalVector.SetValue(2, value * SinHalfAngle, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisZ
        {
            get => new Vector3(X, Y, Z).normalized.z;
            set
            {
                _internalVector.SetValue(3, value * SinHalfAngle, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AngleInRadian
        {
            get => 2 * Mathf.Acos(Mathf.Clamp(W, -1, 1));
            set
            {
                if (Mathf.Approximately(AngleInRadian, value))
                {
                    return; 
                }
                
                bool isWLocked = _internalVector.GetLock(0);
                bool isXLocked = _internalVector.GetLock(1);
                bool isYLocked = _internalVector.GetLock(2);
                bool isZLocked = _internalVector.GetLock(3);
                
                _internalVector.SetLock(0, false);
                _internalVector.SetLock(1, false);
                _internalVector.SetLock(2, false);
                _internalVector.SetLock(3, false);
                
                _internalVector.SetValue(0, Mathf.Cos(value/2), ELockableValueForceSetBehaviour.Force);
                
                _internalVector.SetLock(0, isWLocked);
                _internalVector.SetLock(1, isXLocked);
                _internalVector.SetLock(2, isYLocked);
                _internalVector.SetLock(3, isZLocked);
                
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public string HalfAngleInPI => $"{AngleInRadian / (2 * Mathf.PI):F3} π";

        public bool EnforceNormalisation
        {
            get => _internalVector.GetAutoNormalizeToTargetMagnitude();
            set
            {
                _internalVector.SetAutoNormalizeToTargetMagnitude(value); 
                OnPropertyChanged();
            }
        }
        
        public float SizeSquared => W * W + X * X + Y * Y + Z * Z;

        public float Size => Mathf.Sqrt(W * W + X * X + Y * Y + Z * Z);
        #endregion  otherProperties
        #endregion Properties
        
        #region Constructors
        public override void CopyValues(RotParams_Base toCopy)
        {
            if (toCopy is RotParams_Quaternion rotParams)
            {
                this._internalVector = new LockableVector(rotParams._internalVector);
            }
            else
            {
                CopyValues(toCopy.ToQuaternionParams());
            }
        }
        
        public RotParams_Quaternion()
        {
        }

        public RotParams_Quaternion(RotParams_Quaternion rotParamsQuaternion) : 
            this(rotParamsQuaternion.W, rotParamsQuaternion.X, rotParamsQuaternion.Y, rotParamsQuaternion.Z)
        {
        }

        public RotParams_Quaternion(float w, float x, float y, float z, bool autoNormalizeToTargetMagnitude = true, float TargetMagnitude = 1)
        {
            _internalVector.SetVector(new float[]{w, x, y, z});
            _internalVector.SetLocks(new bool[] { false, false, false, false });
            _internalVector.SetTargetMagnitude(TargetMagnitude);
            _internalVector.SetAutoNormalizeToTargetMagnitude(autoNormalizeToTargetMagnitude);
        }

        public RotParams_Quaternion(Quaternion unityQuaternion)
        {
            _internalVector.SetVector(new float[]
            {
                unityQuaternion.w, 
                unityQuaternion.x, 
                unityQuaternion.y, 
                unityQuaternion.z
            });
        }

        public RotParams_Quaternion(Vector3 inAxis, float inAngleInRadian, bool autoNormalizeToTargetMagnitude = true, float TargetMagnitude = 1)
        {
            inAxis = inAxis.normalized;

            float halfAngle = inAngleInRadian * 0.5f;
            float cos = (float)Math.Cos(halfAngle);
            float sin = (float)Math.Sin(halfAngle);

            _internalVector.SetVector(new float[]
            {
                cos, 
                inAxis.x * sin, 
                inAxis.y * sin, 
                inAxis.z * sin
            });

            _internalVector.SetAutoNormalizeToTargetMagnitude(autoNormalizeToTargetMagnitude);
            _internalVector.SetTargetMagnitude(TargetMagnitude);
        }
        #endregion Constructors

        #region staticQuaternions
        private static readonly RotParams_Quaternion _xdentity = new RotParams_Quaternion(1, 0, 0, 0);

        public override RotParams_Base GetIdentity()
        {
            return GetQuatIdentity();
        }
        
        public static RotParams_Quaternion GetQuatIdentity()
        {
            return new RotParams_Quaternion(_xdentity);
        }

        private static readonly RotParams_Quaternion ZeroRotParamsQuaternion = new RotParams_Quaternion(0, 0, 0, 0);

        public static RotParams_Quaternion GetZeroQuaternion()
        {
            return new RotParams_Quaternion(ZeroRotParamsQuaternion);
        }
        #endregion staticQuaternions

        #region operators
        #region comparison
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            RotParams_Quaternion qr = (RotParams_Quaternion)obj;
            return this == qr;
        }

        public static bool operator ==(RotParams_Quaternion qr1, RotParams_Quaternion qr2)
        {
            if (ReferenceEquals(qr1, qr2))
            {
                return true;
            }

            if (((object)qr1 == null) || ((object)qr2 == null))
            {
                return false;
            }

            return (Math.Abs(qr1.W - qr2.W) < 0.0001f && Math.Abs(qr1.X - qr2.X) < 0.0001f &&
                   Math.Abs(qr1.Y - qr2.Y) < 0.0001f && Math.Abs(qr1.Z - qr2.Z) < 0.0001f) 
                   ||
                   (Math.Abs(qr1.W + qr2.W) < 0.0001f && Math.Abs(qr1.X + qr2.X) < 0.0001f &&
                   Math.Abs(qr1.Y + qr2.Y) < 0.0001f && Math.Abs(qr1.Z + qr2.Z) < 0.0001f);
        }

        public static bool operator !=(RotParams_Quaternion qr1, RotParams_Quaternion qr2)
        {
            return !(qr1 == qr2);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(W, X, Y, Z).GetHashCode();
        }
        #endregion comparison

        #region Arithmetic
        public static RotParams_Quaternion operator *(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return new RotParams_Quaternion(
                q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z,
                q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y,
                q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X,
                q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W, 
                false
            );
        }

        public static RotParams_Quaternion operator *(float scalar, RotParams_Quaternion q1)
        {
            return q1 * scalar; 
        }
        
        public static RotParams_Quaternion operator *(RotParams_Quaternion q1, float scalar)
        {
            return new RotParams_Quaternion(
                q1.W * scalar,
                q1.X * scalar,
                q1.Y * scalar,
                q1.Z * scalar, 
                false
            );
        }

        public static RotParams_Quaternion operator /(RotParams_Quaternion q1, float scalar)
        {
            return new RotParams_Quaternion(
                q1.W / scalar,
                q1.X / scalar,
                q1.Y / scalar,
                q1.Z / scalar, 
                false
            );
        }

        public static RotParams_Quaternion operator -(RotParams_Quaternion q)
        {
            return new RotParams_Quaternion(-q.W, -q.X, -q.Y, -q.Z, false);
        }
        
        public static RotParams_Quaternion operator +(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return new RotParams_Quaternion(
                q1.W + q2.W,
                q1.X + q2.X,
                q1.Y + q2.Y,
                q1.Z + q2.Z, 
                false
            );
        }

        public static RotParams_Quaternion operator -(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return new RotParams_Quaternion(
                q1.W - q2.W,
                q1.X - q2.X,
                q1.Y - q2.Y,
                q1.Z - q2.Z, 
                false
            ); 
        }
        
        public RotParams_Quaternion Log()
        {
            float halfAngle = AngleInRadian * 0.5f;

            if (halfAngle < 1e-6f)
                return new RotParams_Quaternion(0f, 0f, 0f, 0f, false); 

            Vector3 logVec = NormalizedAxis * halfAngle;
            return new RotParams_Quaternion(0f, logVec.x, logVec.y, logVec.z, false);
        }

        public RotParams_Quaternion Exp()
        {
            if (W != 0)
            {
                Debug.LogWarning("Quaternion exponentiation is meant to be used on pure quaternions");
            }
            
            Vector3 v = NonNormalizedAxis;
            float theta = v.magnitude;

            if (theta < 1e-6f)
            {
                return new RotParams_Quaternion(1f, 0f, 0f, 0f, false); 
            }

            Vector3 axis = v / theta;
            float w = Mathf.Cos(theta);
            Vector3 xyz = axis * Mathf.Sin(theta);

            return new RotParams_Quaternion(w, xyz.x, xyz.y, xyz.z, false);
        }

        public RotParams_Quaternion Pow(float t)
        {
            return Exp(t * Log()); 
        }

        public static RotParams_Quaternion Log(RotParams_Quaternion q)
        {
            return q.Log();
        }

        public static RotParams_Quaternion Exp(RotParams_Quaternion q)
        {
            return q.Exp();
        }

        public static RotParams_Quaternion Pow(RotParams_Quaternion q, float t)
        {
            return q.Pow(t);
        }

        
        public void Normalize()
        {
            _internalVector.SetTargetMagnitude(1);
        }
        
        public RotParams_Quaternion GetNormalized()
        {
            float size = this.Size;
            return new RotParams_Quaternion(
                W / size,
                X / size,
                Y / size,
                Z / size, 
                false
            );
        }
        
        public RotParams_Quaternion Inverse()
        {
            return new RotParams_Quaternion(
                W, -X, -Y, -Z, false) 
                   / Size;
        }

        public RotParams_Quaternion Conjugate()
        {
            return new RotParams_Quaternion(W, -X, -Y, -Z, false);
        }

        public static float Dot(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return q1.W * q2.W + q1.X + q2.X + q1.Y + q2.Y + q1.Z * q2.Z;
        }
        #endregion arithmetic
        #endregion operators
        
        #region Converters

        public override RotParams_Base ToSelfTypeCopy(RotParams_Base toConvert)
        {
            return toConvert.ToQuaternionParams(); 
        }

        public override void ToSelfType(RotParams_Base toConvert)
        {
            toConvert.ToQuaternionParams(this); 
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
            return ToAxisAngleParams(new RotParams_AxisAngle()); 
        }

        public override RotParams_EulerAngles ToEulerParams(RotParams_EulerAngles eulerParams)
        {
            if (!eulerParams.IsGimbalValid())
            {
                eulerParams.ResetToIdentity();
                return eulerParams;
            }

            RotParams_Quaternion q = this; // Assuming the current quaternion is the rotation
            float qx = q.X;
            float qy = q.Y;
            float qz = q.Z;
            float qw = q.W;

            switch (eulerParams.GetExtrinsicGimbalOrder())
            {
                case EGimbalOrder.YXZ:
                    // Yaw (Z), Pitch (X), Roll (Y) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(2.0f * (qy * qz + qw * qx), qw * qw - qx * qx - qy * qy + qz * qz);
                    eulerParams.Middle.AngleInRadian = Mathf.Asin(-2.0f * (qx * qz - qw * qy));
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(2.0f * (qx * qy + qw * qz), qw * qw + qx * qx - qy * qy - qz * qz);
                    break;

                case EGimbalOrder.XYZ:
                    // Pitch (X), Yaw (Y), Roll (Z) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(2.0f * (qz * qw - qx * qy), 1.0f - 2.0f * (qy * qy + qz * qz));
                    eulerParams.Middle.AngleInRadian = Mathf.Asin(2.0f * (qx * qz + qy * qw));
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(2.0f * (qx * qw - qy * qz), 1.0f - 2.0f * (qx * qx + qy * qy));
                    break;

                case EGimbalOrder.XZY:
                    // Pitch (X), Roll (Z), Yaw (Y) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(-2.0f * (qy * qz - qw * qx), 1.0f - 2.0f * (qx * qx + qz * qz));
                    eulerParams.Middle.AngleInRadian = Mathf.Asin(2.0f * (qx * qy + qz * qw));
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(-2.0f * (qx * qz - qw * qy), 1.0f - 2.0f * (qx * qx + qy * qy));
                    break;

                case EGimbalOrder.YZX:
                    // Yaw (Y), Roll (Z), Pitch (X) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(-2.0f * (qx * qz - qw * qy), 1.0f - 2.0f * (qy * qy + qz * qz));
                    eulerParams.Middle.AngleInRadian = Mathf.Asin(2.0f * (qx * qy + qz * qw));
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(-2.0f * (qy * qz - qw * qx), 1.0f - 2.0f * (qx * qx + qy * qy));
                    break;

                case EGimbalOrder.ZXY:
                    // Roll (Z), Pitch (X), Yaw (Y) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(2.0f * (qx * qy + qz * qw), 1.0f - 2.0f * (qx * qx + qz * qz));
                    eulerParams.Middle.AngleInRadian = Mathf.Asin(-2.0f * (qy * qz - qw * qx));
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(2.0f * (qx * qz + qy * qw), 1.0f - 2.0f * (qx * qx + qy * qy));
                    break;

                case EGimbalOrder.ZYX:
                    // Roll (Z), Yaw (Y), Pitch (X) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(2.0f * (qx * qz + qy * qw), 1.0f - 2.0f * (qy * qy + qz * qz));
                    eulerParams.Middle.AngleInRadian = Mathf.Asin(-2.0f * (qx * qy - qz * qw));
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(2.0f * (qy * qz + qx * qw), 1.0f - 2.0f * (qx * qx + qy * qy));
                    break;

                case EGimbalOrder.XYX:
                    // Pitch (X), Yaw (Y), Pitch (X) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(2.0f * (qx * qy + qz * qw), 1.0f - 2.0f * (qy * qy + qz * qz));
                    eulerParams.Middle.AngleInRadian = Mathf.Acos(2.0f * (qw * qw + qx * qx) - 1.0f);
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(2.0f * (qx * qy - qz * qw), 1.0f - 2.0f * (qx * qx + qy * qy));
                    break;

                case EGimbalOrder.XZX:
                    // Pitch (X), Roll (Z), Pitch (X) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(2.0f * (qx * qz - qy * qw), 1.0f - 2.0f * (qx * qx + qz * qz));
                    eulerParams.Middle.AngleInRadian = Mathf.Acos(2.0f * (qw * qw + qx * qx) - 1.0f);
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(2.0f * (qx * qz + qy * qw), 1.0f - 2.0f * (qx * qx + qz * qz));
                    break;

                case EGimbalOrder.YXY:
                    // Yaw (Y), Pitch (X), Yaw (Y) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(2.0f * (qx * qy - qz * qw), 1.0f - 2.0f * (qx * qx + qz * qz));
                    eulerParams.Middle.AngleInRadian = Mathf.Acos(2.0f * (qw * qw + qy * qy) - 1.0f);
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(2.0f * (qy * qz - qx * qw), 1.0f - 2.0f * (qy * qy + qz * qz));
                    break;

                case EGimbalOrder.YZY:
                    // Yaw (Y), Roll (Z), Yaw (Y) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(2.0f * (qy * qz + qx * qw), 1.0f - 2.0f * (qx * qx + qy * qy));
                    eulerParams.Middle.AngleInRadian = Mathf.Acos(2.0f * (qw * qw + qy * qy) - 1.0f);
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(2.0f * (qy * qz - qx * qw), 1.0f - 2.0f * (qy * qy + qz * qz));
                    break;

                case EGimbalOrder.ZXZ:
                    // Roll (Z), Pitch (X), Roll (Z) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(2.0f * (qx * qz + qy * qw), 1.0f - 2.0f * (qy * qy + qz * qz));
                    eulerParams.Middle.AngleInRadian = Mathf.Acos(2.0f * (qw * qw + qz * qz) - 1.0f);
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(2.0f * (qy * qz - qx * qw), 1.0f - 2.0f * (qx * qx + qz * qz));
                    break;

                case EGimbalOrder.ZYZ:
                    // Roll (Z), Yaw (Y), Roll (Z) - intrinsic
                    eulerParams.Inner.AngleInRadian = Mathf.Atan2(2.0f * (qy * qz - qx * qw), 1.0f - 2.0f * (qx * qx + qy * qy));
                    eulerParams.Middle.AngleInRadian = Mathf.Acos(2.0f * (qw * qw + qz * qz) - 1.0f);
                    eulerParams.Outer.AngleInRadian = Mathf.Atan2(2.0f * (qy * qz + qx * qw), 1.0f - 2.0f * (qx * qx + qz * qz));
                    break;

                default:
                    eulerParams.ResetToIdentity();
                    break;
            }

            return eulerParams;
        }

        public override RotParams_Quaternion ToQuaternionParams(RotParams_Quaternion quaternionParams)
        {
            quaternionParams.CopyValues(new RotParams_Quaternion(W, X, Y, Z));
            return quaternionParams;
        }

        public override RotParams_Matrix ToMatrixParams(RotParams_Matrix matrixParams)
        {
            float xx = X * X;
            float xy = X * Y;
            float xz = X * Z;
            float xw = X * W;

            float yy = Y * Y;
            float yz = Y * Z;
            float yw = Y * W;

            float zz = Z * Z;
            float zw = Z * W;

            matrixParams[0, 0] = 1 - (2 * (yy + zz));
            matrixParams[0, 1] = 2 * (xy - zw);
            matrixParams[0, 2] = 2 * (xz + yw);

            matrixParams[1, 0] = 2 * (xy + zw);
            matrixParams[1, 1] = 1 - (2 * (xx + zz));
            matrixParams[1, 2] = 2 * (yz - xw);

            matrixParams[2, 0] = 2 * (xz - yw);
            matrixParams[2, 1] = 2 * (yz + xw);
            matrixParams[2, 2] = 1 - (2 * (xx + yy));
            
            return matrixParams;
        }

        public override RotParams_AxisAngle ToAxisAngleParams(RotParams_AxisAngle axisAngleParams)
        {
            axisAngleParams.NormalisedAxis = NormalizedAxis; 
            axisAngleParams.AngleInRadian = AngleInRadian;
            return axisAngleParams; 
        }
        #endregion Converters

        #region Functions

        public override void GetValuesFromUnityQuaternion(Quaternion unityQuaternion)
        {
            W = unityQuaternion.w;
            X = unityQuaternion.x;
            Y = unityQuaternion.y;
            Z = unityQuaternion.z;
        }

        public override string ToString()
        {
            return $"({W}, {X}, {Y}, {Z})";
        }
        
        public override RotParams_Base GetInverse()
        {
            return Inverse(); 
        }
        
        protected override RotParams_Base Concatenate_Implementation(RotParams_Base otherRotation, bool otherFirst = false)
        {
            return otherFirst ? 
                this * (otherRotation as RotParams_Quaternion) :
                (otherRotation as RotParams_Quaternion) * this;
        }

        public override void ResetToIdentity()
        {
            _internalVector.SetAutoNormalizeToTargetMagnitude(false); 
            _internalVector.SetLocks(new bool[] { false, false, false, false }); 
            _internalVector.SetVector(new float[] { 1, 0, 0, 0});
            _internalVector.SetTargetMagnitude(1);
            _internalVector.SetAutoNormalizeToTargetMagnitude(true);
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            RotParams_Quaternion vectorAsRotParamsQuaternion =
                new RotParams_Quaternion(0, inVector.x, inVector.y, inVector.z);
            vectorAsRotParamsQuaternion = this * vectorAsRotParamsQuaternion * this.Inverse();
            return new Vector3(vectorAsRotParamsQuaternion.X, vectorAsRotParamsQuaternion.Y,
                vectorAsRotParamsQuaternion.Z);
        }
        
        public static RotParams_Quaternion CombineSequentialRotation(
            RotParams_Quaternion first, RotParams_Quaternion second)
        {
            return second * first;
        }
        
        public static RotParams_Quaternion CombineViaSum(RotParams_Quaternion a,
            RotParams_Quaternion b)
        {
            RotParams_Quaternion sum = a + b; 
            sum.Normalize();
            return sum;
        }

        public static RotParams_Quaternion Slerp(
            RotParams_Quaternion q0, RotParams_Quaternion q1, float alpha, 
            bool shortestPathCorrection = false)
        {
            float dot = Dot(q0, q1);
            if (dot < 0 && shortestPathCorrection)
            {
                q1 = -q1;
                dot = -dot; 
            }
            
            const float DOT_THRESHOLD = 0.9995f;
            if (dot > DOT_THRESHOLD)
            {
                return LerpNormalised(q0, q1, alpha); 
            }
            
            float theta = Mathf.Acos(dot);
            float oneMinusAlpha = 1 - alpha;
            
            float sinTheta = Mathf.Sin(theta);

            RotParams_Quaternion result = (Mathf.Sin((oneMinusAlpha * theta)) / sinTheta) * q0 +
                     (Mathf.Sin(alpha * theta) / sinTheta) * q1; 
            result.Normalize();
            return result;
        }

        public static RotParams_Quaternion Squad(
            RotParams_Quaternion q0, RotParams_Quaternion q1, 
            RotParams_Quaternion outQ1, RotParams_Quaternion inQ2, 
            float alpha)
        {
            return Slerp(Slerp(q0, q1, alpha), Slerp(outQ1, inQ2, alpha), 2*alpha*(1-alpha));
        }

        public static RotParams_Quaternion LerpNormalised(RotParams_Quaternion q0, RotParams_Quaternion q1, float alpha, bool shortestPathCorrection = false)
        {
            if (shortestPathCorrection && Dot(q0, q1) < 0)
            {
                q1 = -q1;
            }
            
            RotParams_Quaternion result = q0 + alpha * (q1 - q0); 
            result.Normalize();
            return result; 
        }

        public static RotParams_Quaternion BezierCurve(RotParams_Quaternion q0, RotParams_Quaternion q1,
            RotParams_Quaternion outQ0, RotParams_Quaternion inQ1, float alpha)
        {
            float oneMinusAlpha = 1 - alpha;
            RotParams_Quaternion result = 
                    1*oneMinusAlpha*oneMinusAlpha*oneMinusAlpha * q0 + 
                    3*oneMinusAlpha*oneMinusAlpha*alpha * outQ0 + 
                    3*oneMinusAlpha*alpha*alpha * inQ1 + 
                    1*alpha*alpha*alpha * q1
                ; 
            result.Normalize();
            return result;
        }

        public static RotParams_Quaternion LogarithmicInterpolation(RotParams_Quaternion q0, RotParams_Quaternion q1, float alpha, bool shortestPathCorrection = false)
        {
            if (shortestPathCorrection && Dot(q0, q1) < 0)
            {
                q1 = -q1;
            }
 
            RotParams_Quaternion result = q0 * (q1 * q0.Inverse()).Pow(alpha); 
            result.Normalize();
            return result; 
        }
        #endregion Functions
        
        public void OnBeforeSerialize()
        {
            if (_internalVector.Dimensions != 4)
            {
                Debug.LogWarning("AxisAngleRotation: Axis vector has invalid dimensions. Resetting to identity.");
                _internalVector = LockableVector.SafeCreateLockableVector(new float[]{1, 0, 0, 0}, new bool[]{false, false, false}, 1, true);
            }
        }

        public void OnAfterDeserialize()
        {
            if (_internalVector.Dimensions != 4)
            {
                Debug.LogWarning("AxisAngleRotation: Axis vector has invalid dimensions. Resetting to identity.");
                _internalVector = LockableVector.SafeCreateLockableVector(new float[]{1, 0, 0, 0}, new bool[]{false, false, false}, 1, true);
            }
        }
    }
}