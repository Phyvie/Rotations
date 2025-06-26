using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathExtensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Extensions.MathExtensions
{
    public enum ELockableValueForceSetBehaviour
    {
        Force, //Forces that a LockableValues value will be set by functions, even if it is locked; For LockableVectors.SetToScale this unlocks everything scales everything and relocks
        BlockWithMessage, //Default behaviour: Blocks changes when the value is locked
        BlockWithoutMessage //Same as BlockWithMessage, but doesn't print a warning
    }
    
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
        
        public void SetValue(Type value, ELockableValueForceSetBehaviour forceSetBehaviour = ELockableValueForceSetBehaviour.BlockWithMessage)
        {
            if (!isLocked || forceSetBehaviour == ELockableValueForceSetBehaviour.Force)
            {
                typeValue = value;
            }
            else
            {
                if (forceSetBehaviour == ELockableValueForceSetBehaviour.BlockWithMessage)
                {
                    Debug.Log("Value is locked and cannot be changed.");
                }
            }
        }

        public override string ToString()
        {
            return typeValue.ToString();
        }

        [InitializeOnLoadMethod]
        public static void RegisterConverters()
        {
            ConverterGroup boolGroup = new ConverterGroup("bool");

            boolGroup.AddConverter((ref bool v) => !v);
            
            ConverterGroups.RegisterConverterGroup(boolGroup);
        }
    }

    [Serializable]
    public class LockableFloat : LockableValue<float>
    {
        public LockableFloat(float newTypeValue, bool isLocked) : base(newTypeValue, isLocked)
        {
        }
    }

    [Serializable]
    public class LockableVector
    {
        #region Variables
        private float targetLength = 1;
        public List<LockableFloat> values;
        private bool enforceLength; //0ZyKa implement
        #endregion Variables
        
        #region GetSet
        #region Properties
        public float this[int index]
        {
            get => values[index].TypeValue;
            set => SetFloatValue(index, value); 
        }
       
        public float TargetLength
        {
            get => targetLength;
            set
            {
                targetLength = value;
                if (enforceLength)
                {
                    ScaleLockedVectorToLength(targetLength, ELockableValueForceSetBehaviour.Force);
                }
            }
        }
        
        public bool EnforceLength
        {
            get => enforceLength;
            set
            {
                enforceLength = value;
                if (value)
                {
                    ScaleLockedVectorToLength(targetLength, ELockableValueForceSetBehaviour.Force); 
                }
            }
        }
        #endregion
        
        #region GetSetFunctions

        public bool GetLock(int index)
        {
            return values[index].isLocked;
        }

        public void SetLock(int index, bool value)
        {
            values[index].isLocked = value; 
        }
        
        public LockableFloat GetLockableFloatAtIndex(int index)
        {
            return values[index]; 
        }

        public void ReplaceLockableFloatAtIndex(int index, LockableFloat newValue)
        {
            values.RemoveAt(index);
            values.Insert(index, newValue);
        }
        #endregion GetSetFunctions
        #endregion GetSet
        
        #region Constructors
        public LockableVector(int dimensions)
        {
            values = new List<LockableFloat>();
            for (int i = 0; i < dimensions; i++)
            {
                values.Add(new LockableFloat(0, false));
            }
            CheckMissingMagnitude(null);
        }

        public LockableVector(List<LockableFloat> values, float targetLength = 1, bool enforceLength = true, bool checkMissingMagnitude = true)
        {
            this.values = values; 
            this.targetLength = targetLength;
            this.enforceLength = enforceLength;
            if (checkMissingMagnitude)
            {
                CheckMissingMagnitude(null);
            }
        }
        #endregion Constructors

        #region PropertyFunctions
        public float Magnitude()
        {
            float MagnitudeSquared = 0; 
            foreach (LockableFloat lockableFloat in values)
            {
                MagnitudeSquared += lockableFloat.TypeValue * lockableFloat.TypeValue;
            }
            return Mathf.Sqrt(MagnitudeSquared);
        }
        
        public void GetLockedAndUnlockedLength(out float lockedVectorLength, out float unlockedVectorLength)
        {
            int _lockedCount = 0;
            float lockedLengthSquared = 0;
            float unlockedLengthSquared = 0;
            foreach (LockableFloat lockableFloat in values)
            {
                AddToLength(lockableFloat);
            }
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
        #endregion PropertyFunctions
        
        #region GetSetFunctions
        public void SetVector(List<float> newValues, ELockableValueForceSetBehaviour forceSetBehaviour = ELockableValueForceSetBehaviour.Force)
        {
            for (int i = 0; i < newValues.Count; i++)
            {
                values[i].SetValue(newValues[i], forceSetBehaviour);
            }

            if (enforceLength)
            {
                ScaleLockedVectorToLength(targetLength, forceSetBehaviour); 
            }
        }

        public void SetVector3(Vector3 newValue, ELockableValueForceSetBehaviour forceSetBehaviour = ELockableValueForceSetBehaviour.Force)
        {
            if ((newValue - GetVector3()).sqrMagnitude < 0.0001)
            {
                return; 
            }
            if (values.Count != 3)
            {
                throw new Exception("LockableVector.SetVector3(...) can only be used if LockableVector.values.Count == 3"); 
            }
            SetVector(new List<float>(){newValue.x, newValue.y, newValue.z}); 
        }

        public Vector3 GetVector3()
        {
            if (values.Count != 3)
            {
                throw new Exception("LockableVector.GetVector3() can only be used if LockableVector.values.Count == 3"); 
            }

            return new Vector3(values[0], values[1], values[2]); 
        }
        
        public void SetFloatValue(int index, float newValue, ELockableValueForceSetBehaviour forceSetBehaviour = ELockableValueForceSetBehaviour.BlockWithMessage , float newTargetLength = -1)
        {
            SetFloatValue(values[index], newValue, forceSetBehaviour, newTargetLength);
        }
        
        public void SetFloatValue(LockableFloat _lockableValue, float newValue, ELockableValueForceSetBehaviour forceSetBehaviour = ELockableValueForceSetBehaviour.BlockWithMessage, float newTargetLength = -1)
        {
            if (!values.Contains(_lockableValue))
            {
                Debug.Log("Can't set value that is not in vector");
                return; 
            }
            
            if (_lockableValue.isLocked && forceSetBehaviour != ELockableValueForceSetBehaviour.Force)
            {
                Debug.LogWarning("Can't set locked value");
                return; 
            }
            
            if (Mathf.Approximately(_lockableValue.TypeValue, newValue))
            {
                return; 
            }

            if (newTargetLength != -1)
            {
                targetLength = newTargetLength;
            }
            
            if (enforceLength)
            {
                bool isLockedBuffer = _lockableValue.isLocked; 
                _lockableValue.isLocked = true; 
                
                GetLockedAndUnlockedLength(out float lockedLength, out float unlockedLength);
                
                if (_lockableValue.isLocked)
                {
                    lockedLength = MathFunctions.SubtractLengthPythagoreon(lockedLength, _lockableValue.TypeValue); 
                    float maxAbsLength = Mathf.Sqrt(targetLength - lockedLength * lockedLength);
                    _lockableValue.SetValue(Mathf.Clamp(newValue, -maxAbsLength, maxAbsLength), ELockableValueForceSetBehaviour.Force);
                    lockedLength = MathFunctions.AddLengthsPythagoreon(lockedLength, _lockableValue.TypeValue);
                }
                else
                {
                    unlockedLength = MathFunctions.SubtractLengthPythagoreon(unlockedLength, _lockableValue.TypeValue); 
                    float maxAbsLength = Mathf.Sqrt(targetLength - lockedLength * lockedLength);
                    _lockableValue.SetValue(Mathf.Clamp(newValue, -maxAbsLength, maxAbsLength), ELockableValueForceSetBehaviour.Force);
                    unlockedLength = MathFunctions.AddLengthsPythagoreon(unlockedLength, _lockableValue.TypeValue);
                }
                ScaleLockedVectorToLength(lockedLength, unlockedLength);
                
                _lockableValue.isLocked = isLockedBuffer; 
                
                CheckMissingMagnitude(_lockableValue);
            }
            else
            {
                _lockableValue.SetValue(newValue, ELockableValueForceSetBehaviour.Force);
            }
        }
        #endregion GetSetFunctions
        
        #region Functions
        public bool ScaleLockedVectorToLength(float newTargetLength = 1, ELockableValueForceSetBehaviour forceSetBehaviour = ELockableValueForceSetBehaviour.BlockWithMessage)
        {
            GetLockedAndUnlockedLength(out float lockedVectorLength, out float unlockedVectorLength);
            return ScaleLockedVectorToLength(lockedVectorLength, unlockedVectorLength, newTargetLength, forceSetBehaviour);
        }
        
        public bool ScaleLockedVectorToLength(float lockedVectorLength, float unlockedVectorLength, float newTargetLength = -1, ELockableValueForceSetBehaviour forceSetBehaviour = ELockableValueForceSetBehaviour.BlockWithMessage)
        {
            if (newTargetLength != -1)
            {
                targetLength = newTargetLength;
            }
            
            float ratio;
            
            if (forceSetBehaviour == ELockableValueForceSetBehaviour.Force)
            {
                float fullLength = Magnitude(); 
                ratio = targetLength / fullLength;
            }
            else
            {
                if (lockedVectorLength > targetLength)
                {
                    Debug.LogError($"Can't normalize LockableVector when values are locked to be at a length > targetLength");
                    return false; 
                }
                
                float UnlockedMaxLength = MathFunctions.SubtractLengthPythagoreon(targetLength, lockedVectorLength);
                    // Mathf.Sqrt(desiredLength - lockedVectorLength * lockedVectorLength);//!ZyKa
                ratio = unlockedVectorLength != 0 ? 
                    UnlockedMaxLength / unlockedVectorLength : 
                    0;
            }
            foreach (LockableFloat value in values) //It's ok not to not create a new list for the unlocked floats, because each lockableFloat can only be changed if unlocked
            {
                value.SetValue(value.TypeValue * ratio, ELockableValueForceSetBehaviour.BlockWithoutMessage); 
            }

            return true; 
        }

        private void CheckMissingMagnitude(LockableFloat _lockableFloat)
        {
            float missingMagnitude = MathFunctions.SubtractLengthPythagoreon(1, Magnitude());
            foreach (LockableFloat lockableFloat in values)
            {
                if (missingMagnitude < 0.0001f)
                {
                    break; 
                }
                if (lockableFloat == _lockableFloat)
                {
                    continue; 
                }
                if (lockableFloat.isLocked)
                {
                    continue; 
                }
                lockableFloat.SetValue(lockableFloat.TypeValue + missingMagnitude, ELockableValueForceSetBehaviour.BlockWithoutMessage);
                break; 
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
    
            foreach (LockableFloat lockable in values)
            {
                sb.Append(lockable.TypeValue.ToString());
                sb.Append(", ");
            }

            if (values.Count > 0)
                sb.Length -= 2; // Remove the last ", "
    
            sb.Append(")");
            return sb.ToString();
        }


        #endregion Functions
    }
}