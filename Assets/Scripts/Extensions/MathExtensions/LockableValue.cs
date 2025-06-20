using System.Collections.Generic;
using MathExtensions;
using NUnit.Framework;
using UnityEngine;

namespace RotParams
{
    [System.Serializable] 
    public class LockableValue<Type> 
    { 
        [SerializeField] private Type typeValue; 
        [SerializeField] public bool isLocked; 
        
        public LockableValue(Type newTypeValue, bool isLocked)
        {
            this.typeValue = newTypeValue; 
            this.isLocked = isLocked;
        } 
        
        public Type TypeValue
        {
            get => typeValue;
            set
            {
                if (!isLocked)
                {
                    this.typeValue = value;
                }
                else
                {
                    Debug.LogWarning("Value is locked and cannot be changed.");
                }
            }
        } 
        
        public static implicit operator Type(LockableValue<Type> lockableValue)
        {
            return lockableValue.TypeValue;
        }
        
        public static implicit operator LockableValue<Type>(Type value) 
        { 
            LockableValue<Type> lockableValue = new LockableValue<Type>(value, false); 
            return lockableValue; 
        }
        
        public void SetValue(Type value, bool forceSet = false)
        {
            if (!isLocked || forceSet)
            {
                typeValue = value;
            }
            else
            {
                Debug.Log("Value is locked and cannot be changed.");
            }
        }
        
        #region UXMLSupport
        
        #endregion UXMLSupport
    }

    [System.Serializable]
    public class LockableFloat : LockableValue<float>
    {
        public LockableFloat(float newTypeValue, bool isLocked) : base(newTypeValue, isLocked)
        {
        }
    }

    [System.Serializable]
    public class LockableVector3
    {
        public float targetLength;
        public List<LockableFloat> values;
        public bool enforceNormalisation; //0ZyKa implement
        
        public float this[int index]
        {
            get => values[index];
            set => SetValue(index, value); 
        }
        
        public LockableVector3(int dimensions)
        {
            values = new List<LockableFloat>();
        }

        public void SetValue(int index, float newValue, bool forceSet = false)
        {
            SetValue(values[index], newValue);
        }
        
        public void SetValue(LockableFloat _lockableValue, float newValue, bool forceSet = false, float newTargetLength = -1)
        {
            if (!values.Contains(_lockableValue))
            {
                Debug.Log("Can't set value that is not in vector");
                return; 
            }
            
            if (_lockableValue.isLocked && !forceSet)
            {
                Debug.LogWarning("Can't set locked value");
                return; 
            }
            
            if (Mathf.Approximately(_lockableValue.TypeValue, newValue))
            {
                return; 
            }

            if (newTargetLength != 0)
            {
                targetLength = newTargetLength;
            }
            
            if (enforceNormalisation)
            {
                bool isLockedBuffer = _lockableValue.isLocked; 
                _lockableValue.isLocked = true; 
                
                GetLockedAndUnlockedLength(out float lockedLength, out float unlockedLength, out int lockedCount);
                
                if (_lockableValue.isLocked)
                {
                    lockedLength = MathFunctions.SubtractLengthPythagoreon(lockedLength, _lockableValue.TypeValue); 
                    float maxAbsLength = Mathf.Sqrt(targetLength - lockedLength * lockedLength);
                    _lockableValue.SetValue(Mathf.Clamp(newValue, -maxAbsLength, maxAbsLength), true);
                    lockedLength = MathFunctions.AddLengthsPythagoreon(lockedLength, _lockableValue.TypeValue);
                }
                else
                {
                    unlockedLength = MathFunctions.SubtractLengthPythagoreon(unlockedLength, _lockableValue.TypeValue); 
                    float maxAbsLength = Mathf.Sqrt(targetLength - lockedLength * lockedLength);
                    _lockableValue.SetValue(Mathf.Clamp(newValue, -maxAbsLength, maxAbsLength), true);
                    unlockedLength = MathFunctions.AddLengthsPythagoreon(unlockedLength, _lockableValue.TypeValue);
                }
                ScaleLockedVectorToLength(lockedLength, unlockedLength, 1);
                
                _lockableValue.isLocked = isLockedBuffer; 
                
                //!ZyKa ensure length is not smaller than targetLength
                // float xyzMagnitude = (new Vector3(X, Y, Z)).magnitude; 
                // values[0].TypeValue = Mathf.Sign(values[0]) * MathFunctions.SubtractLengthPythagoreon(1, xyzMagnitude); 
            }
            else
            {
                _lockableValue.SetValue(newValue, true);
            }
        }
        
        private void GetLockedAndUnlockedLength(out float lockedVectorLength, out float unlockedVectorLength, out int lockedCount)
        {
            int _lockedCount = 0;
            float lockedLengthSquared = 0;
            float unlockedLengthSquared = 0;
            foreach (LockableFloat lockableFloat in values)
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
        
        private bool ScaleLockedVectorToLength(float lockedVectorLength, float unlockedVectorLength, float desiredLength = 1)
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
            foreach (LockableFloat value in values) //It's ok not to not create a new list for the unlocked floats, because each lockableFloat can only be changed if unlocked
            {
                value.SetValue(value.TypeValue * ratio); 
            }

            return true; 
        }
    }
}