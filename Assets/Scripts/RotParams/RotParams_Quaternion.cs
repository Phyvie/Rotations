using System;
using System.Collections.Generic;
using MathExtensions;
using Unity.Properties;
using UnityEngine;

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

        #region XYZWValueAccessors
        [CreateProperty]
        public float W
        {
            get => _w;
            set
            {
                SetWXYZValueChecked(ref _w, value);
                OnPropertyChanged(nameof(W));
            }
        }

        [CreateProperty]
        public float X
        {
            get => _x;
            set
            {
                SetWXYZValueChecked(ref _x, value);
                OnPropertyChanged(nameof(X));
            }
        }

        [CreateProperty]
        public float Y
        {
            get => _y;
            set
            {
                SetWXYZValueChecked(ref _y, value);
                OnPropertyChanged(nameof(Y));
            }
        }

        [CreateProperty]
        public float Z
        {
            get => _z;
            set
            {
                SetWXYZValueChecked(ref _z, value);
                OnPropertyChanged(nameof(Z));
            }
        }

        private void SetWXYZValueChecked(ref LockableFloat _lockableValue, float newValue)
        {
            if (Mathf.Approximately(_lockableValue.TypeValue, newValue))
            {
                return; 
            }
            
            if (EnforceNormalisation)
            {
                bool isLockedBuffer = _lockableValue.isLocked; 
                _lockableValue.isLocked = true; 
                SetValueInLockableFloatList(WXYZList, ref _lockableValue, newValue, 1);
                _lockableValue.isLocked = isLockedBuffer; 
                
                //If i, j or k are changed, while all other values are 0, then the angle is reduced
                float xyzMagnitude = (new Vector3(X, Y, Z)).magnitude; 
                W = Mathf.Sign(W) * MathFunctions.SubtractLengthPythagoreon(1, xyzMagnitude); 
            }
            else
            {
                _lockableValue.SetValue(newValue, true);
            }
        }

        private void SetXYZValueChecked(ref LockableFloat _lockableValue, float newValue)
        {
            if (EnforceNormalisation)
            {
                SetValueInLockableFloatList(XYZList, ref _lockableValue, newValue);
            }
            else
            {
                _lockableValue.SetValue(newValue, true);
            }
        }
        
        //-ZyKa this function should probably be in another class
        private void SetValueInLockableFloatList(List<LockableFloat> list, ref LockableFloat _lockableValue, float newValue, float desiredVectorLength = -1)
        {
            if (Mathf.Approximately(_lockableValue.TypeValue, newValue))
            {
                return; 
            }
            GetLockedAndUnlockedLength(list, out float lockedLength, out float unlockedLength, out int lockedCount);
            if (desiredVectorLength == -1)
            {
                desiredVectorLength = MathFunctions.AddLengthsPythagoreon(lockedLength, unlockedLength);
            }
            
            if (_lockableValue.isLocked)
            {
                lockedLength = MathFunctions.SubtractLengthPythagoreon(lockedLength, _lockableValue.TypeValue); 
                float maxAbsLength = Mathf.Sqrt(desiredVectorLength - lockedLength * lockedLength);
                _lockableValue.SetValue(Mathf.Clamp(newValue, -maxAbsLength, maxAbsLength), true);
                lockedLength = MathFunctions.AddLengthsPythagoreon(lockedLength, _lockableValue.TypeValue);
            }
            else
            {
                unlockedLength = MathFunctions.SubtractLengthPythagoreon(unlockedLength, _lockableValue.TypeValue); 
                float maxAbsLength = Mathf.Sqrt(desiredVectorLength - lockedLength * lockedLength);
                _lockableValue.SetValue(Mathf.Clamp(newValue, -maxAbsLength, maxAbsLength), true);
                unlockedLength = MathFunctions.AddLengthsPythagoreon(unlockedLength, _lockableValue.TypeValue);
            }
            ScaleLockedVectorToLength(list, lockedLength, unlockedLength, 1);
        }

        private List<LockableFloat> WXYZList => new List<LockableFloat>(){_w, _x, _y, _z}; 
        private List<LockableFloat> XYZList => new List<LockableFloat>(){_x, _y, _z};
        
        public LockableFloat GetInternalLockableFloatByIndex(int index)
        {
            return WXYZList[index]; 
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
                _x.SetValue(scaledAxis.x, true);
                _y.SetValue(scaledAxis.y, true);
                _z.SetValue(scaledAxis.z, true);
                
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisX
        {
            get => new Vector3(X, Y, Z).normalized.x;
            set
            {
                SetValueInLockableFloatList(WXYZList, ref _x, value * SinHalfAngle, 1);
                OnPropertyChanged();
            }
        }
        
        [CreateProperty]
        public float AxisY
        {
            get => new Vector3(X, Y, Z).normalized.y;
            set
            {
                SetValueInLockableFloatList(WXYZList, ref _y, value * SinHalfAngle, 1);
                OnPropertyChanged();
            }
        }

        [CreateProperty]
        public float AxisZ
        {
            get => new Vector3(X, Y, Z).normalized.z;
            set
            {
                SetValueInLockableFloatList(WXYZList, ref _z, value * SinHalfAngle, 1);
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
                
                SetValueInLockableFloatList(WXYZList, ref _w, Mathf.Cos(value/2), 1);
                
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
                enforceNormalisation = value;
                if (value)
                {
                    GetLockedAndUnlockedLength(WXYZList, out float originalLockedLength, out float unlockedLength, out int lockedCount);
                    if (originalLockedLength > 1)
                    {
                        bool[] isLockedBuffer = new bool[4];
                        for (int i = 0; i < WXYZList.Count; i++)
                        {
                            isLockedBuffer[i] = WXYZList[i].isLocked;
                            WXYZList[i].isLocked = false;
                        }
                        ScaleLockedVectorToLength(WXYZList);
                        for (int i = 0; i < WXYZList.Count; i++)
                        {
                            WXYZList[i].isLocked = isLockedBuffer[i];
                        }
                    }
                    else
                    {
                        ScaleLockedVectorToLength(WXYZList);
                    }
                }
                OnPropertyChanged();
            }
        }

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

        private bool ScaleLockedVectorToLength(List<LockableFloat> partiallyLockedVector, float desiredLength = 1)
        {
            GetLockedAndUnlockedLength(partiallyLockedVector, out float lockedVectorLength, out float unlockedVectorLength, out int lockedCount);
            return ScaleLockedVectorToLength(partiallyLockedVector, lockedVectorLength, unlockedVectorLength, desiredLength);
        }

        private bool ScaleLockedVectorToLength(List<LockableFloat> partiallyLockedVector, float lockedVectorLength, float unlockedVectorLength, float desiredLength = 1)
        {
            float ratio; 
            if (lockedVectorLength > desiredLength)
            {
                Debug.LogError($"Can't normalize Quaternion when values are locked to be at a length > 1: {ToString()}");
                return false; 
            }
            else
            {
                float UnlockedMaxLength = Mathf.Sqrt(desiredLength - lockedVectorLength * lockedVectorLength);//!ZyKa
                ratio = unlockedVectorLength != 0 ? 
                    UnlockedMaxLength / unlockedVectorLength : 
                    0;
            }
            foreach (LockableFloat value in WXYZList)
            {
                value.SetValue(value.TypeValue * ratio); 
            }

            return true; 
        }

        private void GetLockedAndUnlockedLength(List<LockableFloat> partiallyLockedVector, out float lockedVectorLength, out float unlockedVectorLength, out int lockedCount)
        {
            int _lockedCount = 0;
            float lockedLengthSquared = 0;
            float unlockedLengthSquared = 0;
            foreach (LockableFloat lockableFloat in partiallyLockedVector)
            {
                AddToLength(lockableFloat);
            }
            lockedCount = _lockedCount;
            lockedVectorLength = Mathf.Sqrt(lockedLengthSquared);
            unlockedVectorLength = Mathf.Sqrt(unlockedLengthSquared);

            void AddToLength(LockableFloat lockableFloat)
            {
                if (lockableFloat.isLocked)
                {
                    lockedLengthSquared += lockableFloat * lockableFloat;
                    _lockedCount++;
                }
                else
                {
                    unlockedLengthSquared += lockableFloat * lockableFloat;
                }
            }
        }

        public void normalize()
        {
            float sizeSquared = SizeSquared();
            _w.TypeValue = _w.TypeValue / sizeSquared;
            _x.TypeValue = _x.TypeValue / sizeSquared;
            _y.TypeValue = _y.TypeValue / sizeSquared;
            _z.TypeValue = _z.TypeValue / sizeSquared;
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
            _w.SetValue(1, true);
            _x.SetValue(0, true); 
            _y.SetValue(0, true); 
            _z.SetValue(0, true); 
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