using System;
using System.Collections.Generic;
using System.Linq;
using MathExtensions;
using UnityEngine;

namespace Extensions.MathExtensions
{
    public enum ELockableValueForceSetBehaviour
    {
        Force, //Forces that a LockableValues value will be set by functions, even if it is locked; For LockableVectors.SetToScale this unlocks everything scales everything and relocks
        BlockWithMessage, //Default behaviour: Blocks changes when the value is locked & prints a warning that a change was attempted
        BlockWithoutMessage //Blocks attempted changes, prints no warning
    }

    /// <summary>
    /// A vector where individual components can be locked.
    /// Supports automatic normalization to a target length, even with locked components.
    /// </summary>
    [Serializable]
    public class LockableVector
    {
        #region Variables
        [SerializeField] private float targetLength = 1;
        [SerializeField] private float[] values;
        [SerializeField] private bool[] locks;
        [SerializeField] private bool autoNormalizeToTargetLength;
        #endregion Variables

        #region Properties
        public static implicit operator float[](LockableVector lockableVector) => lockableVector.values; 

        public float this[int index]
        {
            get => values[index];
            set => SetValue(index, value);
        }

        public float GetTargetLength() => targetLength;

        public bool SetTargetLength(float value)
        {
            float originalTargetLength = targetLength;
            targetLength = value;
            if (autoNormalizeToTargetLength)
            {
                if (NormalizeToTargetLength())
                {
                    return true;
                }
                else
                {
                    targetLength = originalTargetLength;
                    Debug.LogWarning($"Could not set targetLength to {value} because normalization failed. Resetting to original TargetLength. \n Possible Problem Cause: lockedLength is greater than the new targetLength. To solve the problem unlock components before setting targetLength");
                    return false;
                }
            }
            return true;
        }

        public bool GetAutoNormalizeToTargetLength() => autoNormalizeToTargetLength;

        public bool SetAutoNormalizeToTargetLength(bool value)
        {
            bool originalValue = autoNormalizeToTargetLength;
            autoNormalizeToTargetLength = value;
            if (value)
            {
                if (NormalizeToTargetLength())
                {
                    return true;
                }
                else
                {
                    autoNormalizeToTargetLength = false;
                    Debug.LogWarning("Could not enable AutoNormalizeToTargetLength because normalization failed. Resetting to false. \n Possible Problem Cause: lockedLength is greater than the new targetLength. To solve the problem unlock components before setting AutoNormalizeToTargetLength to true");
                    return false;
                }
            }
            return true;
        }

        public float[] ValuesCopy => values.ToArray();
        public bool[] LocksCopy => locks.ToArray(); 
        #endregion

        #region GetSetFunctions
        public float GetValueAtIndex(int index)
        {
            return values[index];
        }

        public void ReplaceValueAtIndex(int index, float newValue)
        {
            values[index] = newValue;
        }
        
        public bool GetLock(int index)
        {
            return locks[index];
        }

        public void SetLock(int index, bool value)
        {
            locks[index] = value;
        }
        #endregion GetSetFunctions

        #region Constructors
        public LockableVector(int dimensions)
        {
            values = new float[dimensions];
            locks = new bool[dimensions];
            for (int i = 0; i < dimensions; i++)
            {
                values[i] = 0f;
                locks[i] = false;
            }
        }

        public LockableVector(LockableVector lockableVector) : this(lockableVector.ValuesCopy, lockableVector.LocksCopy, lockableVector.targetLength, lockableVector.autoNormalizeToTargetLength)
        { }

        //!!!ZyKa SafeCreate works in the case that everything is locked and lockedLength is smaller than targetLength
        public static LockableVector SafeCreateLockableVector(float[] newValues, bool[] newLocks, float newTargetLength = 1,
            bool newAutoNormalizeToTargetLength = true)
        {
            LockableVector newLockableVector = new LockableVector(newValues, newLocks, newTargetLength, newAutoNormalizeToTargetLength);
            newLockableVector.GetLockedAndUnlockedLength(out float lockedVectorLength, out float unlockedVectorLength);
            if (lockedVectorLength > newTargetLength)
            {
                Debug.LogWarning("LockableVector Construction: Cannot create a Vector with a lockedLength longer than targetLength and autoNormalizeToTargetLength active.");
                return null; 
            }
            if (newAutoNormalizeToTargetLength)
            {
                if (!newLockableVector.NormalizeToTargetLength())
                {
                    Debug.LogError("LockableVector Construction: Failed to normalize Vector");
                }
            }

            return newLockableVector; 
        }
        
        //TodoZyKa LockableVector: Improve how random Vector creation works
        public static LockableVector CreateRandom(
            int minDimension, int maxDimension,
            float minValue, float maxValue,
            float minTargetLength, float maxTargetLength,
            bool enforceLength,
            System.Random random = null)
        {
            if (random == null) random = new System.Random();
            
            int dimensions = random.Next(minDimension, maxDimension + 1);
            var values = new float[dimensions];
            var locks = new bool[dimensions];
            
            for (int d = 0; d < dimensions; d++)
            {
                values[d] = (float)random.NextDouble() * (maxValue - minValue) + minValue;
                locks[d] = random.Next(0, 2) == 1;
            }
            
            float targetLength = (float)random.NextDouble() * (maxTargetLength - minTargetLength) + minTargetLength;
            
            LockableVector newLockableVector = SafeCreateLockableVector(values, locks, targetLength, enforceLength);
            List<int> lockedIndices = new List<int>();
            for (int i = 0; i < dimensions; i++)
            {
                if (locks[i])
                {
                    lockedIndices.Add(i);
                }
            }

            while (newLockableVector is null && lockedIndices.Count > 0)
            {
                int randomNumber = random.Next(0, lockedIndices.Count);
                int unlockIndex = lockedIndices[randomNumber];
                locks[unlockIndex] = false;
                lockedIndices.RemoveAt(randomNumber);
                newLockableVector = SafeCreateLockableVector(values, locks, targetLength, enforceLength);
            }

            if (newLockableVector == null)
            {
                Debug.LogError("LockableVector Construction: Failed to create a random LockableVector");
                return null;
            }
            return new LockableVector(values, locks, targetLength, enforceLength);
        }
        
        private LockableVector(float[] values, bool[] locks, float targetLength = 1, bool autoNormalizeToTargetLength = true)
        {
            this.values = values;
            this.locks = locks;
            this.targetLength = targetLength;
            this.autoNormalizeToTargetLength = autoNormalizeToTargetLength;
        }
        #endregion Constructors

        #region PropertyFunctions
        /// <summary>
        /// Returns the magnitude of the vector.
        /// </summary>
        public float Magnitude()
        {
            float magnitudeSquared = 0;
            for (int i = 0; i < values.Length; i++)
            {
                magnitudeSquared += values[i] * values[i];
            }
            return Mathf.Sqrt(magnitudeSquared);
        }

        /// <summary>
        /// Splits the vector into locked and unlocked parts, returning the length of each.
        /// </summary>
        public void GetLockedAndUnlockedLength(out float lockedVectorLength, out float unlockedVectorLength)
        {
            float lockedLengthSquared = 0;
            float unlockedLengthSquared = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (locks[i])
                {
                    lockedLengthSquared += values[i] * values[i];
                }
                else
                {
                    unlockedLengthSquared += values[i] * values[i];
                }
            }
            lockedVectorLength = Mathf.Sqrt(lockedLengthSquared);
            unlockedVectorLength = Mathf.Sqrt(unlockedLengthSquared);
        }
        #endregion PropertyFunctions

        #region GetSetFunctions
        public void SetVector(IEnumerable<float> newValues, ELockableValueForceSetBehaviour forceSetBehaviour = ELockableValueForceSetBehaviour.Force)
        {
            float[] newVals = newValues.ToArray();
            if (newVals.Length != values.Length)
            {
                Debug.LogError("Vector dimension mismatch");
                return;
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (locks[i] && forceSetBehaviour != ELockableValueForceSetBehaviour.Force)
                {
                    continue;
                }
                values[i] = newVals[i];
            }

            if (GetAutoNormalizeToTargetLength())
            {
                NormalizeToTargetLength(); 
            }
        }

        /// <summary>
        /// Attempts to set the value at the specified index.
        /// 1. Temporarily locks the index.
        /// 2. Sets the value.
        /// 3. If autoNormalizeToTargetLength is true:
        ///    a. If the locked part of the vector (without the new value) is greater than targetLength, an error is printed and return false. 
        ///    b. If the newly typed in value would make the vector larger than the targetLength, the new value is clamped to the maximum length, such that targetLength is reached. 
        ///    c. Default: call NormalizeToTargetLength
        /// 4. Resets the lock to its original state.
        /// 5. Verifies if Magnitude equals targetLength.
        /// </summary>
        /// <returns>True if the value was set successfully (possibly clamped).</returns>
        public bool SetValue(int index, float newValue, ELockableValueForceSetBehaviour forceSetBehaviour = ELockableValueForceSetBehaviour.BlockWithMessage)
        {
            if (index < 0 || index >= values.Length)
            {
                Debug.LogError("Index out of bounds");
                return false;
            }

            if (locks[index] && forceSetBehaviour != ELockableValueForceSetBehaviour.Force)
            {
                if (forceSetBehaviour == ELockableValueForceSetBehaviour.BlockWithMessage)
                {
                    Debug.LogWarning("Can't set locked value");
                }
                return false;
            }

            //1. Store Original values and lock
            float originalVal = values[index];
            bool originalLock = locks[index];
            locks[index] = true;
            
            //2. Set the new value
            values[index] = newValue;

            // 3. if enforceLength do the following: 
            if (autoNormalizeToTargetLength)
            {
                // a. calculate locked and unlocked lengths, subtract the current value's contribution from the locked length
                GetLockedAndUnlockedLength(out float lockedLength, out float unlockedLength);
                float otherLockedLength = MathFunctions.SubtractLengthPythagoreon(lockedLength, values[index]);

                // b. if targetLength < lockedLength, reset value[index] to what it originally was, print a warning and return
                if (targetLength < otherLockedLength - 0.0001f)
                {
                    Debug.LogWarning("TargetLength is smaller than locked length of other components");
                    values[index] = originalVal;
                    locks[index] = originalLock;
                    return false;
                }

                // c. if targetLength with contribution from value[index] is greater than lockedLength, set value[index] to the maximum length it can be such that the overall Magnitude is targetLength and return
                float maxPossibleContribution = MathFunctions.SubtractLengthPythagoreon(targetLength, otherLockedLength);
                if (Mathf.Abs(values[index]) > maxPossibleContribution + 0.0001f)
                {
                    values[index] = Mathf.Sign(values[index]) * maxPossibleContribution;
                    //Must call NormalizeToTargetLength even after clamping, in case that the unlockedValues need to be decreased such that the newly set value can have its value
                }

                if (!NormalizeToTargetLength(lockedLength, unlockedLength))
                {
                    values[index] = originalVal;
                    locks[index] = originalLock;
                    return false; 
                }
            }

            // 4. reset the edited index lock back to what it was before the function
            locks[index] = originalLock;

            // 5. check whether Magnitude is equal to targetLength, if not, print a warning and return
            if (autoNormalizeToTargetLength && Mathf.Abs(Magnitude() - targetLength) > 0.001f)
            {
                Debug.LogError($"Magnitude {Magnitude()} is not equal to targetLength {targetLength}");
                return false;
            }

            return true;
        }
        #endregion GetSetFunctions

        #region Functions
        public bool NormalizeToTargetLength()
        {
            GetLockedAndUnlockedLength(out float lockedVectorLength, out float unlockedVectorLength);
            return NormalizeToTargetLength(lockedVectorLength, unlockedVectorLength);
        }
        
        /// <summary>
        /// Scales the vector to the targetLength.
        /// 1. Calculates length of locked and unlocked parts.
        /// 2. If lockedLength > targetLength, sets unlocked to 0 and returns false.
        /// 3. If all unlocked parts are zero, sets the first unlocked value to reach targetLength.
        /// 4. Scales unlocked parts proportionally if possible.
        /// </summary>
        private bool NormalizeToTargetLength(float lockedVectorLength, float unlockedVectorLength)
        {
            // 1. calculate the length of the vectors locked parts and unlocked parts (already passed in)
            
            // 2. if the locked parts are already longer than the targetLength, put the unlocked all to 0, return false
            if (lockedVectorLength > targetLength + 0.0001f)
            {
                Debug.LogWarning($"Can't normalize to targetLength, locked parts are longer than targetLength: {lockedVectorLength} > targetLength = {(lockedVectorLength - targetLength)}");
                return false;
            }

            float unlockedTargetLength = MathFunctions.SubtractLengthPythagoreon(targetLength, lockedVectorLength);
            
            // 3. if all unlocked parts are zero, set the first unlocked value to the value, such that the vector as a whole has the targetLength, return true
            if (unlockedVectorLength < 0.0001f)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (!locks[i])
                    {
                        values[i] = unlockedTargetLength;
                        return true;
                    }
                }
                // If no unlocked parts exist, check if we're already at target length
                return lockedVectorLength > targetLength - 0.001f;
            }

            // 4. if the unlocked parts can be downscaled / upscaled via multiplication/division enough to reach the targetLength, do so, return true
            float scaleRatio = unlockedTargetLength / unlockedVectorLength;
            for (int i = 0; i < values.Length; i++)
            {
                if (!locks[i])
                {
                    values[i] *= scaleRatio;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return $"({string.Join(", ", values)})";
        }

        public string ToLongString()
        {
            var mixed = values.Zip(locks, (v, l) => $"({v}, {l})");
            return $"targetLength: {targetLength}, enforced: {GetAutoNormalizeToTargetLength()}, data: [{string.Join(", ", mixed)}]"; 
        }
        #endregion Functions
    }
}
