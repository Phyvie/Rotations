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
    public class RotParams_Quaternion : RotParams
    {
        [SerializeField] private bool enforceNormalisation = true; //-ZyKa Quaternion enforceNormalisation
        [SerializeField] private LockableFloat _w; 
        [SerializeField] private LockableFloat _x;
        [SerializeField] private LockableFloat _y;
        [SerializeField] private LockableFloat _z;

        [SerializeField] private LockableVector lockableWXYZ; 
        
        
        #region XYZWValueAccessors
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
        
        public LockableFloat GetInternalLockableFloatByIndex(int index)
        {
            return lockableWXYZ[index]; 
        }
        #endregion XYZWValueAccessors

        private float SinHalfAngle => Mathf.Sqrt(1-W*W); 
        [CreateProperty]
        public Vector3 Axis
        {
            get => new Vector3(X, Y, Z).normalized;
            set
            {
                
                if (Mathf.Approximately((Axis - value.normalized).sqrMagnitude, 0))
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
        
        [CreateProperty]
        public float Angle
        {
            get => 2 * Mathf.Acos(W);
            set
            {
                if (Mathf.Approximately(Angle, value))
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

        public RotParams_Quaternion()
        {
            _w = new LockableFloat(1, false);
            _x = new LockableFloat(0, false);
            _y = new LockableFloat(0, false);
            _z = new LockableFloat(0, false);
            InitSetupLockableVectors();


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
            InitSetupLockableVectors();
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
            InitSetupLockableVectors();
        }

        private void InitSetupLockableVectors()
        {
            lockableWXYZ = new LockableVector(
                new List<LockableFloat>(){_w, _x, _y, _z},
                1, 
                true); 
        }
        
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

        public static RotParams_Quaternion operator *(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return new RotParams_Quaternion(
                q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z,
                q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y,
                q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X,
                q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W
            );
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

        public static RotParams_Quaternion operator +(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return new RotParams_Quaternion(
                q1.W + q2.W,
                q1.X + q2.X,
                q1.Y + q2.Y,
                q1.Z * q2.Z
            );
        }

        public override string ToString()
        {
            return $"({W}, {X}, {Y}, {Z})";
        }

        public RotParams_Quaternion Inverse()
        {
            return new RotParams_Quaternion(W, -X, -Y, -Z) / Size();
        }

        public RotParams_Quaternion Conjugate()
        {
            return new RotParams_Quaternion(W, -X, -Y, -Z);
        }

        public static RotParams_Quaternion CombineFollowingRotation(
            RotParams_Quaternion first, RotParams_Quaternion second)
        {
            return second * first;
        }

        public static RotParams_Quaternion CombineSimultaneousRotation(RotParams_Quaternion a,
            RotParams_Quaternion b)
        {
            return (a + b).Normalize();
        }

        public float SizeSquared()
        {
            return W * W + X * X + Y * Y + Z * Z;
        }

        public float Size()
        {
            return Mathf.Sqrt(W * W + X * X + Y * Y + Z * Z);
        }

        public void normalize()
        {
            lockableWXYZ.targetLength = 1;
            
        }

        public RotParams_Quaternion Normalize()
        {
            float size = this.Size();
            return new RotParams_Quaternion(
                W / size,
                X / size,
                Y / size,
                Z / size
            );
        }

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
    }
}