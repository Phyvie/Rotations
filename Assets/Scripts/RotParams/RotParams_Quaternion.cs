using System;
using System.Collections.Generic;
using Extensions.MathExtensions;
using MathExtensions;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace RotParams
{
    [Serializable]
    public class RotParams_Quaternion : RotParams_Base
    {
        [SerializeField] private bool enforceNormalisation = true; //-ZyKa Quaternion enforceNormalisation
        [SerializeField] private LockableFloat _w; 
        [SerializeField] private LockableFloat _x;
        [SerializeField] private LockableFloat _y;
        [SerializeField] private LockableFloat _z;

        private LockableVector lockableWXYZ => new LockableVector(new List<LockableFloat>(){_w, _x, _y, _z}, 1.0f, true); 
        
        #region Properties
        #region WXYZValueAccessors
        [CreateProperty]
        public float W
        {
            get => _w;
            set
            {
                lockableWXYZ.SetFloatValue(_w, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(W));
            }
        }

        public bool WLocked
        {
            get => _w.isLocked; 
            set => _w.isLocked = value;
        }

        [CreateProperty]
        public float X
        {
            get => _x;
            set
            {
                lockableWXYZ.SetFloatValue(_x, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(X));
            }
        }

        public bool XLocked
        {
            get => _x.isLocked; 
            set => _x.isLocked = value;
        }
        
        [CreateProperty]
        public float Y
        {
            get => _y;
            set
            {
                lockableWXYZ.SetFloatValue(_y, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(Y));
            }
        }

        public bool YLocked
        {
            get => _y.isLocked; 
            set => _y.isLocked = value;
        }
        
        [CreateProperty]
        public float Z
        {
            get => _z;
            set
            {
                lockableWXYZ.SetFloatValue(_z, value, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged(nameof(Z));
            }
        }
        
        public bool ZLocked
        {
            get => _z.isLocked; 
            set => _z.isLocked = value;
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
            get => new Vector3(X, Y, Z).normalized;
            set
            {
                
                if (Mathf.Approximately((NormalizedAxis - value.normalized).sqrMagnitude, 0))
                {
                    return; 
                }

                Vector3 scaledAxis = value * SinHalfAngle; 
                _x.SetValue(scaledAxis.x, ELockableValueForceSetBehaviour.Force);
                _y.SetValue(scaledAxis.y, ELockableValueForceSetBehaviour.Force);
                _z.SetValue(scaledAxis.z, ELockableValueForceSetBehaviour.Force);
                
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisX
        {
            get => new Vector3(X, Y, Z).normalized.x;
            set
            {
                lockableWXYZ.SetFloatValue(_x, value * SinHalfAngle, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public float AxisY
        {
            get => new Vector3(X, Y, Z).normalized.y;
            set
            {
                lockableWXYZ.SetFloatValue(_y, value * SinHalfAngle, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisZ
        {
            get => new Vector3(X, Y, Z).normalized.z;
            set
            {
                lockableWXYZ.SetFloatValue(_z, value * SinHalfAngle, ELockableValueForceSetBehaviour.Force);
                OnPropertyChanged();
            }
        }
        
        public float SignedAngle
        {
            get => 2 * Mathf.Acos(W) * Mathf.Sign(W);
            set
            {
                if (Mathf.Approximately(SignedAngle, value))
                {
                    return; 
                }
                
                bool isXLocked = _x.isLocked;
                bool isYLocked = _y.isLocked;
                bool isZLocked = _z.isLocked;
                bool isWLocked = _w.isLocked;
                
                _w.isLocked = false;
                _x.isLocked = false;
                _y.isLocked = false;
                _z.isLocked = false;
                
                lockableWXYZ.SetFloatValue(_w, Mathf.Cos(value/2), ELockableValueForceSetBehaviour.Force);
                
                _w.isLocked = isWLocked;
                _x.isLocked = isXLocked;
                _y.isLocked = isYLocked;
                _z.isLocked = isZLocked;
                
                OnPropertyChanged();
            }
        }

        public bool EnforceNormalisation
        {
            get => enforceNormalisation;
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
        public RotParams_Quaternion()
        {
            _w = new LockableFloat(1, false);
            _x = new LockableFloat(0, false);
            _y = new LockableFloat(0, false);
            _z = new LockableFloat(0, false);
        }

        public RotParams_Quaternion(RotParams_Quaternion rotParamsQuaternion) : this(rotParamsQuaternion.W,
            rotParamsQuaternion.X, rotParamsQuaternion.Y, rotParamsQuaternion.Z)
        {
        }

        public RotParams_Quaternion(float w, float x, float y, float z)
        {
            _w = new LockableFloat(w, false);
            _x = new LockableFloat(x, false);
            _y = new LockableFloat(y, false);
            _z = new LockableFloat(z, false);
        }

        public RotParams_Quaternion(Vector3 inAxis, float inAngle)
        {
            inAxis = inAxis.normalized;

            float halfAngle = inAngle * 0.5f;
            float cos = (float)Math.Cos(halfAngle);
            float sin = (float)Math.Sin(halfAngle);

            _w = new LockableFloat(cos, false);
            _x = new LockableFloat(inAxis.x * sin, false);
            _y = new LockableFloat(inAxis.y * sin, false);
            _z = new LockableFloat(inAxis.z * sin, false);
        }
        #endregion Constructors

        #region staticQuaternions
        private static readonly RotParams_Quaternion _xdentity = new RotParams_Quaternion(1, 0, 0, 0);

        public static RotParams_Quaternion GetIdentity()
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

            return Math.Abs(qr1._w - qr2._w) < 0.0001f && Math.Abs(qr1._x - qr2._x) < 0.0001f &&
                   Math.Abs(qr1._y - qr2._y) < 0.0001f && Math.Abs(qr1._z - qr2._z) < 0.0001f;
        }

        public static bool operator !=(RotParams_Quaternion qr1, RotParams_Quaternion qr2)
        {
            return !(qr1 == qr2);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(_w, _x, _y, _z).GetHashCode();
        }
        #endregion comparison

        #region Arithmetic
        public static RotParams_Quaternion operator *(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return new RotParams_Quaternion(
                q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z,
                q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y,
                q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X,
                q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W
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
                q1.Z * scalar
            );
        }

        public static RotParams_Quaternion operator /(RotParams_Quaternion q1, float scalar)
        {
            return new RotParams_Quaternion(
                q1.W / scalar,
                q1.X / scalar,
                q1.Y / scalar,
                q1.Z / scalar
            );
        }

        public static RotParams_Quaternion operator -(RotParams_Quaternion q)
        {
            return new RotParams_Quaternion(-q.W, -q.X, -q.Y, -q.Z);
        }
        
        public static RotParams_Quaternion operator +(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return new RotParams_Quaternion(
                q1.W + q2.W,
                q1.X + q2.X,
                q1.Y + q2.Y,
                q1.Z * q2.Z
            );
        }

        public static RotParams_Quaternion operator -(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return new RotParams_Quaternion(
                q1.W - q2.W,
                q1.X - q2.X,
                q1.Y - q2.Y,
                q1.Z - q2.Z
            ); 
        }
        
        public RotParams_Quaternion Log()
        {
            float halfAngle = SignedAngle * 0.5f;

            if (halfAngle < 1e-6f)
                return new RotParams_Quaternion(0f, 0f, 0f, 0f); 

            Vector3 logVec = NormalizedAxis * halfAngle;
            return new RotParams_Quaternion(0f, logVec.x, logVec.y, logVec.z);
        }

        public RotParams_Quaternion Exp()
        {
            if (_w != 0)
            {
                Debug.LogWarning("Quaternion exponentiation is meant to be used on pure quaternions");
            }
            
            Vector3 v = NonNormalizedAxis;
            float theta = v.magnitude;

            if (theta < 1e-6f)
            {
                return new RotParams_Quaternion(1f, 0f, 0f, 0f); 
            }

            Vector3 axis = v / theta;
            float w = Mathf.Cos(theta);
            Vector3 xyz = axis * Mathf.Sin(theta);

            return new RotParams_Quaternion(w, xyz.x, xyz.y, xyz.z);
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
                Z / size
            );
        }
        
        public RotParams_Quaternion Inverse()
        {
            return new RotParams_Quaternion(W, -X, -Y, -Z) / Size;
        }

        public RotParams_Quaternion Conjugate()
        {
            return new RotParams_Quaternion(W, -X, -Y, -Z);
        }

        public static float Dot(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return q1.W * q2.W + q1.X + q2.X + q1.Y + q2.Y + q1.Z * q2.Z;
        }
        #endregion arithmetic
        #endregion operators
        
        #region Converters
        public override RotParams_EulerAngles ToEulerAngleRotation()
        {
            RotParams_EulerAngles newRotParamsEulerAngles = new RotParams_EulerAngles(0, 0, 0);
            newRotParamsEulerAngles.GetValuesFromQuaternion(this);
            return newRotParamsEulerAngles;
        }

        public override RotParams_Quaternion ToQuaternionRotation()
        {
            return new RotParams_Quaternion(W, X, Y, Z);
        }

        public override RotParams_Matrix ToMatrixRotation()
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

        public override RotParams_AxisAngle ToAxisAngleRotation()
        {
            return new RotParams_AxisAngle(new Vector3(X, Y, Z), (float)Math.Acos(W));
        }
        #endregion Converters

        #region Functions
        public override string ToString()
        {
            return $"({W}, {X}, {Y}, {Z})";
        }
        
        public override void ResetToIdentity()
        {
            _w.SetValue(1, ELockableValueForceSetBehaviour.Force);
            _x.SetValue(0, ELockableValueForceSetBehaviour.Force); 
            _y.SetValue(0, ELockableValueForceSetBehaviour.Force); 
            _z.SetValue(0, ELockableValueForceSetBehaviour.Force); 
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

        public static RotParams_Quaternion Slerp(RotParams_Quaternion q0, RotParams_Quaternion q1, float alpha, bool shortestPathCorrection = false)
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

        public static RotParams_Quaternion Squad(RotParams_Quaternion q0, RotParams_Quaternion q1, RotParams_Quaternion outQ1, RotParams_Quaternion inQ2, float alpha)
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
        
        #region UISupport
        [InitializeOnLoadMethod]
        public static void RegisterConverters()
        {
            ConverterGroup boolGroup = new ConverterGroup("bool");

            boolGroup.AddConverter((ref bool v) => !v);
            
            ConverterGroups.RegisterConverterGroup(boolGroup);
        }
        #endregion UISupport
    }
}