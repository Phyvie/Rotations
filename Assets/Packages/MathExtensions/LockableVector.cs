using System;
using System.Collections.Generic;
using System.Linq;
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
    /// A vector that can be setup to keep a target magnitude, i.e. when one entry changes, the other values change accordingly to keep the magnitude consistent. 
    /// Individual components can be locked. For example, consider a LockableVector with three components where the first entry is locked. If the second value is increased, the first one remains constant; only the third value will decrease to maintain a consistent magnitude.
    /// </summary>
    [Serializable]
    public class LockableVector
    {
        #region Variables
        [SerializeField] private float targetMagnitude = 1;
        [SerializeField] private float[] values;
        [SerializeField] private bool[] locks;
        [SerializeField] private bool autoNormalizeToTargetMagnitude;
        #endregion Variables

        #region Properties
        public static implicit operator float[](LockableVector lockableVector) => lockableVector.values;

        public int Dimensions
        {
            get => values.Length;
            set
            {
                float[] valuesCopy = values.ToArray(); 
                bool[] locksCopy = locks.ToArray();
                int originalDimensions = Dimensions;
                
                values = new float[value];
                locks = new bool[value];
                Array.Copy(valuesCopy, values, Math.Min(originalDimensions, value));
                Array.Copy(locksCopy, locks, Math.Min(originalDimensions, value));
                if (value < Dimensions && autoNormalizeToTargetMagnitude)
                {
                    if (NormalizeToTargetMagnitude())
                    {
                        return; 
                    }
                    else
                    {
                        Debug.LogError("Can't decrease dimensions of lockableVector, because normalization will fail. Resetting to original values.");
                        values = valuesCopy;
                        locks = locksCopy;
                        return; 
                    }
                }
            }
        }

        public float this[int index]
        {
            get => values[index];
            set => SetValue(index, value);
        }

        public float GetTargetMagnitude() => targetMagnitude;

        public bool SetTargetMagnitude(float value)
        {
            float originalTargetMagnitude = targetMagnitude;
            targetMagnitude = value;
            if (autoNormalizeToTargetMagnitude)
            {
                if (NormalizeToTargetMagnitude())
                {
                    return true;
                }
                else
                {
                    targetMagnitude = originalTargetMagnitude;
                    Debug.LogWarning($"Could not set targetMagnitude to {value} because normalization failed. Resetting to original TargetMagnitude. \n Possible Problem Cause: lockedMagnitude is greater than the new targetMagnitude. To solve the problem unlock components before setting targetMagnitude");
                    return false;
                }
            }
            return true;
        }

        public bool GetAutoNormalizeToTargetMagnitude() => autoNormalizeToTargetMagnitude;

        public bool SetAutoNormalizeToTargetMagnitude(bool value)
        {
            bool originalValue = autoNormalizeToTargetMagnitude;
            autoNormalizeToTargetMagnitude = value;
            if (value)
            {
                if (NormalizeToTargetMagnitude())
                {
                    return true;
                }
                else
                {
                    autoNormalizeToTargetMagnitude = false;
                    Debug.LogWarning("Could not enable AutoNormalizeToTargetMagnitude because normalization failed. Resetting to false. \n Possible Problem Cause: lockedMagnitude is greater than the new targetMagnitude. To solve the problem unlock components before setting AutoNormalizeToTargetMagnitude to true");
                    return false;
                }
            }
            return true;
        }

        public float[] ValuesCopy => values.ToArray();
        public bool[] LocksCopy => locks.ToArray(); 
        
        public bool IsFullyLocked => locks.All(l => l == true);
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
        }

        public LockableVector(LockableVector lockableVector) : this(lockableVector.ValuesCopy, lockableVector.LocksCopy, lockableVector.targetMagnitude, lockableVector.autoNormalizeToTargetMagnitude)
        { }

        public static LockableVector SafeCreateLockableVector(float[] newValues, bool[] newLocks, float newTargetMagnitude = 1,
            bool newAutoNormalizeToTargetMagnitude = true)
        {
            LockableVector newLockableVector = new LockableVector(newValues.ToArray(), newLocks.ToArray(), newTargetMagnitude, newAutoNormalizeToTargetMagnitude);
            
            if (newAutoNormalizeToTargetMagnitude)
            {
                newLockableVector.GetLockedAndUnlockedMagnitudeSquared(out float lockedVectorMagnitudeSq, out float unlockedVectorMagnitudeSq);
                float targetMagnitudeSq = newTargetMagnitude * newTargetMagnitude;

                if (lockedVectorMagnitudeSq > targetMagnitudeSq + 0.0001f)
                {
                    float lockedVectorMagnitude = Mathf.Sqrt(lockedVectorMagnitudeSq);
                    Debug.LogWarning($"LockableVector Construction: Cannot create a Vector with a lockedMagnitude ({lockedVectorMagnitude}) longer than targetMagnitude ({newTargetMagnitude}) and autoNormalizeToTargetMagnitude active.");
                    return null;
                }

                if (newLockableVector.IsFullyLocked && Mathf.Abs(lockedVectorMagnitudeSq - targetMagnitudeSq) > 0.0001f)
                {
                    float lockedVectorMagnitude = Mathf.Sqrt(lockedVectorMagnitudeSq);
                    Debug.LogWarning($"LockableVector Construction: Cannot create a fully locked vector with a lockedMagnitude ({lockedVectorMagnitude}) different from targetMagnitude ({newTargetMagnitude}).");
                    return null;
                }

                if (!newLockableVector.NormalizeToTargetMagnitude(lockedVectorMagnitudeSq, unlockedVectorMagnitudeSq))
                {
                    Debug.LogError("LockableVector Construction: Failed to normalize Vector");
                }
            }

            return newLockableVector; 
        }
        
        public static LockableVector CreateRandom(
            int minDimension, int maxDimension,
            float maxPosValue, float maxNegValue,
            float minTargetMagnitude, float maxTargetMagnitude,
            bool autoNormalizeToTargetMagnitude,
            System.Random random = null)
        {
            random ??= new System.Random();
            maxNegValue = -Math.Abs(maxNegValue); 
            maxPosValue = Math.Abs(maxPosValue);
            if (minTargetMagnitude > maxTargetMagnitude || maxTargetMagnitude < 0)
            {
                Debug.LogWarning("Invalid targetMagnitude range");
                return null; 
            }
            
            float maxAbsValue = Mathf.Max(Mathf.Abs(maxPosValue), Mathf.Abs(maxNegValue));
            float leewayValue = maxAbsValue * 0.8f; 
            if (leewayValue * Mathf.Sqrt(maxDimension) < minTargetMagnitude)
            {
                Debug.LogWarning("Cannot efficiently create a random vector with the specified targetMagnitude range and dimensions range");
                return null; 
            }
            int minRequiredDimensionForMinMagnitude = (int)Mathf.Ceil(Mathf.Pow((minTargetMagnitude / leewayValue), 2));
            minDimension = Mathf.Clamp(minDimension, minRequiredDimensionForMinMagnitude, maxDimension);
            float maxMagnitudeAtMaxDimensions = maxAbsValue * Mathf.Sqrt(maxDimension); 
            maxTargetMagnitude = Mathf.Clamp(maxTargetMagnitude, minTargetMagnitude, maxMagnitudeAtMaxDimensions);
            
            float targetMagnitude = (float)random.NextDouble() * (maxTargetMagnitude - minTargetMagnitude) + minTargetMagnitude;
            int dimensions = random.Next(minDimension, maxDimension + 1);
            while (Mathf.Sqrt(dimensions) * leewayValue < targetMagnitude)
            {
                dimensions = Mathf.Min(maxDimension, dimensions + 1);;
                targetMagnitude = Mathf.Max(minTargetMagnitude, targetMagnitude * 0.9f);;
            }
            
            var values = new float[dimensions];
            var locks = new bool[dimensions];

            LockableVector lockableVector = null; 
            bool isValid = false;
            int tries = 0; 
            while (!isValid && tries++ < 100)
            {
                float currentMagnitudeSquared = 0;
                for (int i = 0; i < dimensions; i++)
                {
                    values[i] = (float)random.NextDouble() * (maxPosValue - maxNegValue) + maxNegValue;
                    currentMagnitudeSquared += values[i] * values[i];
                }

                float currentMagnitude = Mathf.Sqrt(currentMagnitudeSquared);
                if (currentMagnitude <= 0)
                {
                    continue; 
                }

                float scale = targetMagnitude / currentMagnitude;
                bool containsOversizedValues = false; 
                for (int i = 0; i < dimensions; i++)
                {
                    values[i] *= scale;
                    if (values[i] > maxPosValue || values[i] < maxNegValue)
                    {
                        containsOversizedValues = true;
                        break; 
                    }
                }
                isValid = !containsOversizedValues;
            }

            if (!isValid)
            {
                Debug.LogWarning("LockableVector Construction: Rejection Sampling failed to create a random vector within the specified range.");
                return null; 
            }

            for (int i = 0; i < dimensions; i++)
            {
                locks[i] = random.NextDouble() < 0.5;
            }
            
            lockableVector = SafeCreateLockableVector(values, locks, targetMagnitude, true);
            if (lockableVector is null)
            {
                Debug.LogError("LockableVector Random Construction failed to unknown reason. Please report this bug.");
                return null; 
            }
            lockableVector.autoNormalizeToTargetMagnitude = autoNormalizeToTargetMagnitude;
            
            return lockableVector;
        }
        
        private LockableVector(float[] values, bool[] locks, float targetMagnitude = 1, bool autoNormalizeToTargetMagnitude = true)
        {
            this.values = values;
            this.locks = locks;
            this.targetMagnitude = targetMagnitude;
            this.autoNormalizeToTargetMagnitude = autoNormalizeToTargetMagnitude;
        }
        #endregion Constructors

        #region PropertyFunctions
        public float Magnitude => Mathf.Sqrt(MagnitudeSquared());
        public float MagnitudeSquared()
        {
            float magnitudeSquared = 0;
            for (int i = 0; i < values.Length; i++)
            {
                magnitudeSquared += values[i] * values[i];
            }
            return magnitudeSquared;
        }

        /// <summary>
        /// Splits the vector into locked and unlocked parts, returning the magnitude of each.
        /// </summary>
        public void GetLockedAndUnlockedMagnitude(out float lockedVectorMagnitude, out float unlockedVectorMagnitude)
        {
            GetLockedAndUnlockedMagnitudeSquared(out float lockedMagnitudeSquared, out float unlockedMagnitudeSquared);
            lockedVectorMagnitude = Mathf.Sqrt(lockedMagnitudeSquared);
            unlockedVectorMagnitude = Mathf.Sqrt(unlockedMagnitudeSquared);
        }

        public void GetLockedAndUnlockedMagnitudeSquared(out float lockedMagnitudeSquared, out float unlockedMagnitudeSquared)
        {
            lockedMagnitudeSquared = 0;
            unlockedMagnitudeSquared = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (locks[i])
                {
                    lockedMagnitudeSquared += values[i] * values[i];
                }
                else
                {
                    unlockedMagnitudeSquared += values[i] * values[i];
                }
            }
        }
        #endregion PropertyFunctions

        #region GetSetFunctions
        public void SetLocks(IEnumerable<bool> newLocks)
        {
            int i = 0; 
            foreach (bool newLock in newLocks)
            {
                locks[i] = newLock;
                i++;
            }
            for (; i < locks.Length; i++)
            {
                locks[i] = false;
            }
        }
        
        public void SetVector(IEnumerable<float> newValues, ELockableValueForceSetBehaviour forceSetBehaviour = ELockableValueForceSetBehaviour.Force)
        {
            float[] valuesCopy = values.ToArray(); 
            bool[] locksCopy = locks.ToArray(); 
            
            values = newValues.ToArray(); 
            locks = new bool[values.Length];
            Array.Copy(locksCopy, locks, Math.Min(values.Length, locksCopy.Length));

            if (GetAutoNormalizeToTargetMagnitude() && !NormalizeToTargetMagnitude())
            {
                Debug.LogError("Could not normalize vector after setting new values; resetting to old values");
                values = valuesCopy;
                locks = locksCopy;
            }
        }

        /// <summary>
        /// Attempts to set the value at the specified index.
        /// 1. Temporarily locks the index.
        /// 2. Sets the value.
        /// 3. If autoNormalizeToTargetMagnitude is true:
        ///    a. If the locked part of the vector (without the new value) is greater than targetMagnitude, an error is printed and return false. 
        ///    b. If the newly typed in value would make the vector larger than the targetMagnitude, the new value is clamped to the maximum magnitude, such that targetMagnitude is reached. 
        ///    c. Default: call NormalizeToTargetMagnitude
        /// 4. Resets the lock to its original state.
        /// 5. Verifies if Magnitude equals targetMagnitude.
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
            
            //2. Set the value
            values[index] = newValue; 
            
            // 3. if enforceMagnitude do the following: 
            if (autoNormalizeToTargetMagnitude)
            {
                locks[index] = true;

                GetLockedAndUnlockedMagnitudeSquared(out float lockedMagnitudeSq, out float unlockedMagnitudeSq);
                float targetMagnitudeSq = targetMagnitude * targetMagnitude;
                float currentValSq = values[index] * values[index];
                float otherLockedMagnitudeSq = lockedMagnitudeSq - currentValSq;

                // b. if targetMagnitude < otherLockedMagnitude, reset value[index] to what it originally was, print a warning and return
                if (targetMagnitudeSq < otherLockedMagnitudeSq - 0.0001f)
                {
                    Debug.LogWarning($"TargetMagnitude ({targetMagnitude}) is smaller than locked magnitude of other components ({Mathf.Sqrt(otherLockedMagnitudeSq)})");
                    values[index] = originalVal;
                    locks[index] = originalLock;
                    return false;
                }

                // c. if newValue would make the vector larger than the targetMagnitude, the new value is clamped to the maximum magnitude
                float maxPossibleContributionSq = targetMagnitudeSq - otherLockedMagnitudeSq;
                if (maxPossibleContributionSq < -0.0001f)
                {
                    Debug.LogWarning($"Can't take squareRoot of negative number: {targetMagnitudeSq} - {otherLockedMagnitudeSq} < 0");
                    maxPossibleContributionSq = -1.0f;
                }
                maxPossibleContributionSq = Mathf.Max(0, maxPossibleContributionSq);
                
                if (currentValSq > maxPossibleContributionSq + 0.0001f)
                {
                    values[index] = Mathf.Sign(values[index]) * Mathf.Sqrt(maxPossibleContributionSq);
                    currentValSq = values[index] * values[index];
                }

                // Recalculate locked magnitude squared after potential clamping
                lockedMagnitudeSq = otherLockedMagnitudeSq + currentValSq;

                if (!NormalizeToTargetMagnitude(lockedMagnitudeSq, unlockedMagnitudeSq))
                {
                    values[index] = originalVal;
                    locks[index] = originalLock;
                    return false;
                }
            }

            // 4. reset the edited index lock back to what it was before the function
            locks[index] = originalLock;

            // 5. check whether Magnitude is equal to targetMagnitude, if not, print a warning and return
            if (autoNormalizeToTargetMagnitude && Mathf.Abs(Magnitude - targetMagnitude) > 0.001f)
            {
                Debug.LogError($"Magnitude {Magnitude} is not equal to targetMagnitude {targetMagnitude}"); 
                return false;
            }

            return true;
        }
        #endregion GetSetFunctions

        #region Functions

        public bool ValidateAndFixLockedVector()
        {
            if (values.Length != locks.Length)
            {
                Debug.LogError("Vector.values.Length != Vector.locks.Length");
                return false; 
            }

            if (values.Aggregate(false, (foundNaN, val) => foundNaN || float.IsNaN(val)))
            {
                Debug.LogError("Vector contains NaN values");
                return false;
            }
            
            if (autoNormalizeToTargetMagnitude && Mathf.Approximately(Magnitude, targetMagnitude))
            {
                Debug.LogWarning("Vector.Magnitude == targetMagnitude");
                return false; 
            }

            return true; 
        }

        public bool TryFixVector()
        {
            if (values.Length != locks.Length)
            {
                bool[] newLocks = new bool[values.Length];
                Array.Copy(locks, newLocks, Math.Min(values.Length, locks.Length));
                return true; 
            }

            if (autoNormalizeToTargetMagnitude)
            {
                return NormalizeToTargetMagnitude(); 
            }

            return true; 
        }
        
        public bool NormalizeToTargetMagnitude()
        {
            GetLockedAndUnlockedMagnitudeSquared(out float lockedVectorMagnitudeSq, out float unlockedVectorMagnitudeSq);
            return NormalizeToTargetMagnitude(lockedVectorMagnitudeSq, unlockedVectorMagnitudeSq);
        }

        /// <summary>
        /// Scales the vector to the targetMagnitude.
        /// 1. Calculates magnitude squared of locked and unlocked parts.
        /// 2. If lockedMagnitudeSquared > targetMagnitudeSquared, sets unlocked to 0 and returns false.
        /// 3. If all unlocked parts are zero, sets the first unlocked value to reach targetMagnitude.
        /// 4. Scales unlocked parts proportionally if possible.
        /// </summary>
        private bool NormalizeToTargetMagnitude(float lockedVectorMagnitudeSq, float unlockedVectorMagnitudeSq)
        {
            float targetMagnitudeSq = targetMagnitude * targetMagnitude;
            if (Mathf.Abs(MagnitudeSquared() - targetMagnitudeSq) < 0.0001f)
            {
                return true;
            }
            // 1. calculate the magnitude squared of the vectors locked parts and unlocked parts (already passed in)

            // 2. if the locked parts are already longer than the targetMagnitude, put the unlocked all to 0, return false
            if (lockedVectorMagnitudeSq > targetMagnitudeSq + 0.0001f)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (!locks[i]) values[i] = 0;
                }
                Debug.LogWarning($"Can't normalize to targetMagnitude, locked parts are longer than targetMagnitude: lockedMagnitude = {Mathf.Sqrt(lockedVectorMagnitudeSq)} > targetMagnitude = {targetMagnitude}");
                return false;
            }

            float unlockedTargetMagnitudeSq = targetMagnitudeSq - lockedVectorMagnitudeSq;
            if (unlockedTargetMagnitudeSq < -0.0001f)
            {
                Debug.LogWarning($"Can't take squareRoot of negative number: {targetMagnitudeSq} - {lockedVectorMagnitudeSq} < 0");
                unlockedTargetMagnitudeSq = -1.0f;
            }
            else
            {
                unlockedTargetMagnitudeSq = Mathf.Max(0, unlockedTargetMagnitudeSq);
            }

            // 3. if all unlocked parts are zero, set the first unlocked value to the value, such that the vector as a whole has the targetMagnitude, return true
            if (unlockedVectorMagnitudeSq < 0.0001f)
            {
                if (unlockedTargetMagnitudeSq < 0.0001f) return true; // Already at target magnitude (or close enough)

                float unlockedTargetMagnitude = Mathf.Sqrt(unlockedTargetMagnitudeSq);
                for (int i = 0; i < values.Length; i++)
                {
                    if (!locks[i])
                    {
                        values[i] = unlockedTargetMagnitude;
                        return true;
                    }
                }
                // If no unlocked parts exist, check if we're already at target magnitude
                return Mathf.Abs(lockedVectorMagnitudeSq - targetMagnitudeSq) < 0.001f;
            }

            // 4. if the unlocked parts can be downscaled / upscaled via multiplication/division enough to reach the targetMagnitude, do so, return true
            float scaleRatio = Mathf.Sqrt(unlockedTargetMagnitudeSq / unlockedVectorMagnitudeSq);
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
            return $"targetMagnitude: {targetMagnitude}, enforced: {GetAutoNormalizeToTargetMagnitude()}, data: [{string.Join(", ", mixed)}]"; 
        }
        #endregion Functions
    }
}
