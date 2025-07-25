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
        [SerializeField] private LockableVector lockableWXYZ =
            new LockableVector(
                new List<LockableFloat>()
                {
                    new LockableFloat(1, false),
                    new LockableFloat(0, false),
                    new LockableFloat(0, false),
                    new LockableFloat(0, false)
                }); 
        
        #region Properties
        #region WXYZValueAccessors
        [CreateProperty]
        public float W
        {
            get => lockableWXYZ[0];
            set
            {
                lockableWXYZ.SetFloatValue(0, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(W));
            }
        }

        public bool WLocked
        {
            get => lockableWXYZ.GetLockableFloatAtIndex(0).isLocked; 
            set => lockableWXYZ.GetLockableFloatAtIndex(0).isLocked = value;
        }

        [CreateProperty]
        public float X
        {
            get => lockableWXYZ[1];
            set
            {
                lockableWXYZ.SetFloatValue(1, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(W));
            }
        }

        public bool XLocked
        {
            get => lockableWXYZ.GetLockableFloatAtIndex(1).isLocked; 
            set => lockableWXYZ.GetLockableFloatAtIndex(1).isLocked = value;
        }
        
        [CreateProperty]
        public float Y
        {
            get => lockableWXYZ[2];
            set
            {
                lockableWXYZ.SetFloatValue(2, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(W));
            }
        }

        public bool YLocked
        {
            get => lockableWXYZ.GetLockableFloatAtIndex(2).isLocked; 
            set => lockableWXYZ.GetLockableFloatAtIndex(2).isLocked = value;
        }
        
        [CreateProperty]
        public float Z
        {
            get => lockableWXYZ[3];
            set
            {
                lockableWXYZ.SetFloatValue(3, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(W));
            }
        }

        public bool ZLocked
        {
            get => lockableWXYZ.GetLockableFloatAtIndex(3).isLocked; 
            set => lockableWXYZ.GetLockableFloatAtIndex(3).isLocked = value;
        }
        
        public LockableFloat GetInternalLockableFloatByIndex(int index)
        {
            return lockableWXYZ.GetLockableFloatAtIndex(index); 
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
                if (axis.sqrMagnitude > 0.0001)
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
                lockableWXYZ.SetFloatValue(1, scaledAxis.x, ELockableValueForceSetBehaviour.Force);
                lockableWXYZ.SetFloatValue(2, scaledAxis.y, ELockableValueForceSetBehaviour.Force);
                lockableWXYZ.SetFloatValue(3, scaledAxis.z, ELockableValueForceSetBehaviour.Force);
                
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisX
        {
            get => new Vector3(X, Y, Z).normalized.x;
            set
            {
                lockableWXYZ.SetFloatValue(1, value * SinHalfAngle, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public float AxisY
        {
            get => new Vector3(X, Y, Z).normalized.y;
            set
            {
                lockableWXYZ.SetFloatValue(2, value * SinHalfAngle, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisZ
        {
            get => new Vector3(X, Y, Z).normalized.z;
            set
            {
                lockableWXYZ.SetFloatValue(3, value * SinHalfAngle, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float Angle
        {
            get => 2 * Mathf.Acos(Mathf.Clamp(W, 0, 1));
            set
            {
                if (Mathf.Approximately(Angle, value))
                {
                    return; 
                }
                
                bool isXLocked = lockableWXYZ.GetLockableFloatAtIndex(0).isLocked;
                bool isYLocked = lockableWXYZ.GetLockableFloatAtIndex(1).isLocked;
                bool isZLocked = lockableWXYZ.GetLockableFloatAtIndex(2).isLocked;
                bool isWLocked = lockableWXYZ.GetLockableFloatAtIndex(3).isLocked;
                
                lockableWXYZ.GetLockableFloatAtIndex(0).isLocked = false;
                lockableWXYZ.GetLockableFloatAtIndex(1).isLocked = false;
                lockableWXYZ.GetLockableFloatAtIndex(2).isLocked = false;
                lockableWXYZ.GetLockableFloatAtIndex(3).isLocked = false;
                
                lockableWXYZ.SetFloatValue(0, Mathf.Cos(value/2), ELockableValueForceSetBehaviour.Force);
                
                lockableWXYZ.GetLockableFloatAtIndex(0).isLocked = isWLocked;
                lockableWXYZ.GetLockableFloatAtIndex(1).isLocked = isXLocked;
                lockableWXYZ.GetLockableFloatAtIndex(2).isLocked = isYLocked;
                lockableWXYZ.GetLockableFloatAtIndex(3).isLocked = isZLocked;
                
                OnPropertyChanged();
            }
        }

        public bool EnforceNormalisation
        {
            get => lockableWXYZ.EnforceLength;
            set
            {
                lockableWXYZ.EnforceLength = value; 
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
                this.lockableWXYZ = new LockableVector(new List<LockableFloat>()
                {
                    new LockableFloat(W, WLocked),
                    new LockableFloat(X, XLocked), 
                    new LockableFloat(Y, YLocked),
                    new LockableFloat(Z, ZLocked)
                }); 
            }
            else
            {
                CopyValues(toCopy.ToAxisAngleParams());
            }
        }
        
        public RotParams_Quaternion()
        {
        }

        public RotParams_Quaternion(RotParams_Quaternion rotParamsQuaternion) : 
            this(rotParamsQuaternion.W, rotParamsQuaternion.X, rotParamsQuaternion.Y, rotParamsQuaternion.Z)
        {
        }

        public RotParams_Quaternion(float w, float x, float y, float z, bool enforceLength = true, float targetLength = 1)
        {
            lockableWXYZ.SetVector(new List<float>(){w, x, y, z});
            lockableWXYZ.EnforceLength = enforceLength;
            lockableWXYZ.TargetLength = targetLength;
        }

        public RotParams_Quaternion(Quaternion unityQuaternion)
        {
            lockableWXYZ.SetVector(new List<float>()
            {
                unityQuaternion.w, 
                unityQuaternion.x, 
                unityQuaternion.y, 
                unityQuaternion.z
            });
        }

        public RotParams_Quaternion(Vector3 inAxis, float inAngle, bool enforceLength = true, float targetLength = 1)
        {
            inAxis = inAxis.normalized;

            float halfAngle = inAngle * 0.5f;
            float cos = (float)Math.Cos(halfAngle);
            float sin = (float)Math.Sin(halfAngle);

            lockableWXYZ.SetVector(new List<float>()
            {
                cos, 
                inAxis.x * sin, 
                inAxis.y * sin, 
                inAxis.z * sin
            });

            lockableWXYZ.EnforceLength = enforceLength;
            lockableWXYZ.TargetLength = targetLength;
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

            return Math.Abs(qr1.W - qr2.W) < 0.0001f && Math.Abs(qr1.X - qr2.X) < 0.0001f &&
                   Math.Abs(qr1.Y - qr2.Y) < 0.0001f && Math.Abs(qr1.Z - qr2.Z) < 0.0001f;
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
            float halfAngle = Angle * 0.5f;

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
            lockableWXYZ.TargetLength = 1;
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

        public override RotParams_Base ToSelfType(RotParams_Base toConvert)
        {
            return toConvert.ToQuaternionParams(); 
        }

        public override RotParams_EulerAngles ToEulerParams()
        {
            return ToMatrixParams().ToEulerParams();
        }

        public override RotParams_Quaternion ToQuaternionParams()
        {
            return new RotParams_Quaternion(W, X, Y, Z);
        }

        public override RotParams_Matrix ToMatrixParams()
        {
            return new RotParams_Matrix(
                new float[3, 3]
                {
                    { 1 - 2 * (Y * Y + Z * Z), 2 * (X * Y - W * Z), 2 * (W * Y + X * Z) },
                    { 2 * (X * Y + W * Z), 1 - 2 * (X * X + Z * Z), 2 * (Y * Z - W * X) },
                    { 2 * (X * Z - W * Y), 2 * (W * X + Y * Z), 1 - 2 * (X * X + Y * Y) }
                }
            );
        }

        public override RotParams_AxisAngle ToAxisAngleParams()
        {
            return new RotParams_AxisAngle(NormalizedAxis, Angle);
        }
        #endregion Converters

        #region Functions
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
            lockableWXYZ.SetVector(new List<float>(new float[] { 1, 0, 0, 0}));
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            RotParams_Quaternion vectorAsRotParamsQuaternion =
                new RotParams_Quaternion(0, inVector.x, inVector.y, inVector.z);
            vectorAsRotParamsQuaternion = this * vectorAsRotParamsQuaternion * this.Inverse();
            return new Vector3(vectorAsRotParamsQuaternion.X, vectorAsRotParamsQuaternion.Y,
                vectorAsRotParamsQuaternion.Y);
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
    }
}