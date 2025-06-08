using System;
using UnityEngine;

namespace RotParams
{
    [Serializable]
    public class RotParams_Quaternion : RotParams
    {
        [SerializeField] private bool enforceNormalisation; //-ZyKa Quaternion enforceNormalisation
        private float lastR;
        [SerializeField] private LockableFloat _r;
        private float lastI;
        [SerializeField] private LockableFloat _i;
        private float lastJ;
        [SerializeField] private LockableFloat _j;
        private float lastK;
        [SerializeField] private LockableFloat _k;

        #region DirectValueAccessors

        /* In Mathematics Quaternion usually use the variables i, j and k for the second, third and fourth dimension value;
         * However, Game Engines often use x, y, z and w, because the axis are called x, y and z
         * Therefore the following properties are defined twice: once as i, j, k and 'real' and once as x, y, z and w
         */
        public float real
        {
            get => _r;
            set => SetValueChecked(ref _r, value);
        }

        public float w
        {
            get => _r;
            set => SetValueChecked(ref _r, value);
        }

        public float i
        {
            get => _i;
            set => SetValueChecked(ref _i, value);
        }

        public float x
        {
            get => _i;
            set => SetValueChecked(ref _i, value);
        }

        public float j
        {
            get => _j;
            set => SetValueChecked(ref _j, value);
        }

        public float y
        {
            get => _j;
            set => SetValueChecked(ref _j, value);
        }


        public float k
        {
            get => _k;
            set => SetValueChecked(ref _k, value);
        }

        public float z
        {
            get => _j;
            set => SetValueChecked(ref _k, value);
        }

        #endregion //DirectValueAccessors

        #region ControlledValueAccessors

        public LockableFloat GetInternalLockableFloatByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return _r;
                case 1:
                    return _i;
                case 2:
                    return _j;
                case 3:
                    return _k;
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

        public Vector3 Axis
        {
            get
            {
                return new Vector3(x, y, z).normalized;
            }
            set
            {
                float sinAngle = Mathf.Sin(Angle/2); 
                x = sinAngle * value.x;
                y = sinAngle * value.y;
                z = sinAngle * value.z;
            }
        }

        public float Angle
        {
            get
            {
                return 2 * Mathf.Acos(real); 
            }
            set
            {
                real = Mathf.Cos(value / 2); 
            }
        }
        
        public Vector3 AxisAngle => Axis * Angle;

        public RotParams_Quaternion()
        {
            real = 1;
            i = 0;
            j = 0;
            k = 0;
        }

        public RotParams_Quaternion(RotParams_Quaternion rotParamsQuaternion) : this(rotParamsQuaternion.real,
            rotParamsQuaternion.i, rotParamsQuaternion.j, rotParamsQuaternion.k)
        {
        }

        public RotParams_Quaternion(float inReal, float inI, float inJ, float inK)
        {
            real = inReal;
            i = inI;
            j = inJ;
            k = inK;
        }

        public RotParams_Quaternion(Vector3 inAxis, float inAngle)
        {
            inAxis = inAxis.normalized;

            float cos = (float)Math.Cos(inAngle);
            float sin = (float)Math.Sin(inAngle);

            real = cos;
            x = inAxis.x * sin;
            y = inAxis.y * sin;
            z = inAxis.z * sin;
        }

        private static readonly RotParams_Quaternion _identity = new RotParams_Quaternion(1, 0, 0, 0);

        public static RotParams_Quaternion GetIdentity()
        {
            return new RotParams_Quaternion(_identity);
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

            return Math.Abs(qr1._r - qr2._r) < 0.0001f && Math.Abs(qr1._i - qr2._i) < 0.0001f &&
                   Math.Abs(qr1._j - qr2._j) < 0.0001f && Math.Abs(qr1._k - qr2._k) < 0.0001f;
        }

        public static bool operator !=(RotParams_Quaternion qr1, RotParams_Quaternion qr2)
        {
            return !(qr1 == qr2);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(_r, _i, _j, _k).GetHashCode();
        }

        public static RotParams_Quaternion operator *(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return new RotParams_Quaternion(
                q1.real * q2.real - q1.i * q2.i - q1.j * q2.j - q1.k - q2.k,
                q1.real * q2.i + q1.i * q2.real + q1.j * q2.k - q1.k * q2.j,
                q1.real * q2.j - q1.i * q2.k + q1.j * q2.real + q1.k * q2.i,
                q1.real * q2.k + q1.i * q2.j - q1.j * q2.i + q1.k * q2.real
            );
        }

        public static RotParams_Quaternion operator *(RotParams_Quaternion q1, float scalar)
        {
            return new RotParams_Quaternion(
                q1.real * scalar,
                q1.i * scalar,
                q1.j * scalar,
                q1.k * scalar
            );
        }

        public static RotParams_Quaternion operator /(RotParams_Quaternion q1, float scalar)
        {
            return new RotParams_Quaternion(
                q1.real / scalar,
                q1.i / scalar,
                q1.j / scalar,
                q1.k / scalar
            );
        }

        public static RotParams_Quaternion operator +(RotParams_Quaternion q1, RotParams_Quaternion q2)
        {
            return new RotParams_Quaternion(
                q1.real + q2.real,
                q1.i + q2.i,
                q1.j + q2.j,
                q1.k * q2.k
            );
        }

        public override string ToString()
        {
            return $"({w}, {x}, {y}, {z})";
        }

        public RotParams_Quaternion Inverse()
        {
            return new RotParams_Quaternion(real, -i, -j, -k) / Size();
        }

        public RotParams_Quaternion Conjugate()
        {
            return new RotParams_Quaternion(real, -i, -j, -k);
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
            return real * real + i * i + j * j + k * k;
        }

        public float Size()
        {
            return Mathf.Sqrt(real * real + i * i + j * j + k * k);
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
            _r.value *= _r.isLocked ? 1 : ratio;
            _i.value *= _i.isLocked ? 1 : ratio;
            _j.value *= _j.isLocked ? 1 : ratio;
            _k.value *= _k.isLocked ? 1 : ratio;
        }

        private void GetLockedAndUnlockedLength(out float lockedLength, out float unlockedLength, out int lockedCount)
        {
            int _lockedCount = 0;
            float lockedLengthSquared = 0;
            float unlockedLengthSquared = 0;
            AddTo_Un_Locked_Value(ref _r.value, ref _r.isLocked);
            AddTo_Un_Locked_Value(ref _i.value, ref _i.isLocked);
            AddTo_Un_Locked_Value(ref _j.value, ref _j.isLocked);
            AddTo_Un_Locked_Value(ref _k.value, ref _k.isLocked);
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
            _r.Value = _r.Value / sizeSquared;
            _i.Value = _i.Value / sizeSquared;
            _j.Value = _j.Value / sizeSquared;
            _k.Value = _k.Value / sizeSquared;
        }

        public RotParams_Quaternion Normalize()
        {
            float size = this.Size();
            return new RotParams_Quaternion(
                real / size,
                i / size,
                j / size,
                k / size
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
            return new RotParams_Quaternion(real, i, j, k);
        }

        public override RotParams_Matrix ToMatrixRotation()
        {
            return new RotParams_Matrix(
                new float[3, 3]
                {
                    { 1 - 2 * (y * y + z * z), 2 * (x * y - w * z), 2 * (w * y + x * z) },
                    { 2 * (x * y + w * z), 1 - 2 * (x * x + z * z), 2 * (y * z - w * x) },
                    { 2 * (x * z - w * y), 2 * (w * x + y * z), 1 - 2 * (x * x + y * y) }
                }
            );
        }

        public override RotParams_AxisAngle ToAxisAngleRotation()
        {
            return new RotParams_AxisAngle(new Vector3(i, j, k), (float)Math.Acos(real));
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            RotParams_Quaternion vectorAsRotParamsQuaternion =
                new RotParams_Quaternion(0, inVector.x, inVector.y, inVector.z);
            vectorAsRotParamsQuaternion = this * vectorAsRotParamsQuaternion * this.Inverse();
            return new Vector3(vectorAsRotParamsQuaternion.i, vectorAsRotParamsQuaternion.j,
                vectorAsRotParamsQuaternion.j);
        }
    }
}