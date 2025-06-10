using System;
using Unity.Properties;
using UnityEngine;

namespace RotParams
{
    [Serializable]
    public class RotParams_Quaternion : RotParams
    {
        [SerializeField] private bool enforceNormalisation = false; //-ZyKa Quaternion enforceNormalisation
        private float lastR;
        [SerializeField] private LockableFloat _w; //-ZyKa figure out whether you need a LockableFloat here or not
        private float lastI;
        [SerializeField] private LockableFloat _x;
        private float lastJ;
        [SerializeField] private LockableFloat _y;
        private float lastK;
        [SerializeField] private LockableFloat _z;

        #region DirectValueAccessors
        public float W
        {
            get => _w;
            set => SetValueChecked(ref _w, value);
        }

        public float X
        {
            get => _x;
            set => SetValueChecked(ref _x, value);
        }

        public float Y
        {
            get => _y;
            set => SetValueChecked(ref _y, value);
        }


        public float Z
        {
            get => _z;
            set => SetValueChecked(ref _z, value);
        }
        #endregion //DirectValueAccessors

        #region ControlledValueAccessors

        public LockableFloat GetInternalLockableFloatByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return _w;
                case 1:
                    return _x;
                case 2:
                    return _y;
                case 3:
                    return _z;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        private void SetValueChecked(ref LockableFloat _value, float newValue)
        {
            _value.Value = newValue;
            if (enforceNormalisation)
            {
                GetLockedAndUnlockedLength(out float lockedLength, out float unlockedLength, out int lockedCount);
                Math.Clamp(_value.Value, lockedLength - 1, 1 - lockedLength);
                bool isRLocked = _value.isLocked;
                _value.isLocked = true;
                NormalizeWithLocks();
                _value.isLocked = isRLocked;
            }
        }

        #endregion //ControlledValueAccessors

        [CreateProperty]
        public Vector3 Axis
        {
            get => new Vector3(X, Y, Z).normalized;
            set
            {
                float sinAngle = Mathf.Sin(Angle/2); 
                X = sinAngle * value.x;
                Y = sinAngle * value.y;
                Z = sinAngle * value.z;
            }
        }

        [CreateProperty]
        public float Angle
        {
            get
            {
                return 2 * Mathf.Acos(W); 
            }
            set
            {
                W = Mathf.Cos(value / 2); 
            }
        }

        [CreateProperty]
        public Vector4 Vector4
        {
            get => new Vector4(W, X, Y, Z); 
            set
            {
                W = value.w; 
                X = value.x;
                Y = value.y;
                Z = value.z;
            }
        }
        
        public RotParams_Quaternion()
        {
            _w = 1;
            _x = 0;
            _y = 0;
            _z = 0;
        }

        public RotParams_Quaternion(RotParams_Quaternion rotParamsQuaternion) : this(rotParamsQuaternion.W,
            rotParamsQuaternion.X, rotParamsQuaternion.Y, rotParamsQuaternion.Z)
        {
        }

        public RotParams_Quaternion(float inReal, float inI, float inJ, float inK)
        {
            _w = inReal;
            _x = inI;
            _y = inJ;
            _z = inK;
        }

        public RotParams_Quaternion(Vector3 inAxis, float inAngle)
        {
            inAxis = inAxis.normalized;

            float halfAngle = inAngle * 0.5f;
            float cos = (float)Math.Cos(halfAngle);
            float sin = (float)Math.Sin(halfAngle);

            _w = cos;
            _x = inAxis.x * sin;
            _y = inAxis.y * sin;
            _z = inAxis.z * sin;
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

        public void NormalizeWithLocks()
        {
            GetLockedAndUnlockedLength(out float lockedLength, out float unlockedLength, out int lockedCount);
            if (lockedCount == 4)
            {
                Debug.LogError("Can't normalise Quaternion if all 4 values are locked");
                return;
            }

            if (lockedLength > 1)
            {
                Debug.LogError($"Can't normalise Quaternion if lockedLength = {lockedLength} > 1");
                return;
            }

            float ratio = 1 - lockedLength / unlockedLength;
            _w.value *= _w.isLocked ? 1 : ratio;
            _x.value *= _x.isLocked ? 1 : ratio;
            _y.value *= _y.isLocked ? 1 : ratio;
            _z.value *= _z.isLocked ? 1 : ratio;
        }

        private void GetLockedAndUnlockedLength(out float lockedLength, out float unlockedLength, out int lockedCount)
        {
            int _lockedCount = 0;
            float lockedLengthSquared = 0;
            float unlockedLengthSquared = 0;
            AddTo_Un_Locked_Value(ref _w.value, ref _w.isLocked);
            AddTo_Un_Locked_Value(ref _x.value, ref _x.isLocked);
            AddTo_Un_Locked_Value(ref _y.value, ref _y.isLocked);
            AddTo_Un_Locked_Value(ref _z.value, ref _z.isLocked);
            lockedCount = _lockedCount;
            lockedLength = Mathf.Sqrt(lockedLengthSquared);
            unlockedLength = Mathf.Sqrt(unlockedLengthSquared);

            void AddTo_Un_Locked_Value(ref float value, ref bool locked)
            {
                if (locked)
                {
                    lockedLengthSquared += value * value;
                    _lockedCount++;
                }
                else
                {
                    unlockedLengthSquared += value * value;
                }
            }
        }

        public void normalize()
        {
            float sizeSquared = SizeSquared();
            _w.Value = _w.Value / sizeSquared;
            _x.Value = _x.Value / sizeSquared;
            _y.Value = _y.Value / sizeSquared;
            _z.Value = _z.Value / sizeSquared;
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
            _w = 1;
            _x = 0;
            _y = 0;
            _z = 0; 
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