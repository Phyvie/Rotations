using System.Collections;
using System.Linq;
using Extensions.MathExtensions;
using NUnit.Framework;
using UnityEngine;

namespace Extensions.MathExtensions.Tests
{
    public static class TestExtensions
    {
        public static string ToFormattedString(this IEnumerable enumerable) 
        {
            var items = enumerable.Cast<object>();
            return $"({string.Join(", ", items)})";
        }
    }
    
    public class LockableVectorTests
    {
        [Test]
        public void LockableVector_ConstructionTest()
        {
            var values = new float[] { 1f, 1f };
            var locks = new bool[] { false, false };
            
            var vector_unenforced = LockableVector.SafeCreateLockableVector(values, locks, 2f, false);
            // We need to create a copy because LockableVector uses the same array
            var vector_enforced = LockableVector.SafeCreateLockableVector(new float[] { 1f, 1f }, new bool[] { false, false },
                2f, true); 
            var vector_enforced_lockedFirst = LockableVector.SafeCreateLockableVector(new float[] { 1f, 1f }, new bool[] { true, false }, 
                2f, true);
            
            CollectionAssert.AreEqual(new float[] { 1f, 1f }, vector_unenforced.ValuesCopy);
            var vector_enforced_should_values = new float[] { Mathf.Sqrt(2.0f), Mathf.Sqrt(2.0f) };
            CollectionAssert.AreEqual(vector_enforced_should_values, vector_enforced.ValuesCopy, 
                $"vector_enforced.Values = ({vector_enforced}, but should be {vector_enforced_should_values.ToFormattedString()}");
            var vector_enforced_lockedFirst_should_values = new float[] { 1f, Mathf.Sqrt(3.0f) };
            CollectionAssert.AreEqual(vector_enforced_lockedFirst_should_values, vector_enforced_lockedFirst.ValuesCopy, 
                $"vector_enforced_lockedFirst.Values = ({vector_enforced_lockedFirst}, but should be {vector_enforced_lockedFirst_should_values.ToFormattedString()}");
        }
        
        [Test]
        public void LockableVector_GetLockedAndUnlockedLength_CorrectlyCalculates()
        {
            var values = new float[] { 3f, 4f, 12f };
            var locks = new bool[] { false, false, true };
            
            var vector = LockableVector.SafeCreateLockableVector(values, locks, 13f, true);
            vector.GetLockedAndUnlockedLength(out float locked, out float unlocked);
            
            Assert.AreEqual(5f, unlocked, 0.001f);
            Assert.AreEqual(12f, locked, 0.001f);
            Assert.AreEqual(13f, vector.Magnitude(), 0.001f);
        }

        [Test]
        public void LockableVector_SetValue_RenormalizesCorrectly()
        {
            var values = new float[] { 13f, 4f, 0f };
            var locks = new bool[] { false, true, false };
            
            var vector = LockableVector.SafeCreateLockableVector(values, locks, 5f, true);
            Debug.Log($"[DEBUG_LOG] Initial state: {vector.ToLongString()}, Magnitude: {vector.Magnitude()}");
            vector.SetValue(0, 3.0f);
            Debug.Log($"[DEBUG_LOG] Post-set state: {vector.ToLongString()}, Magnitude: {vector.Magnitude()}");
            float[] shouldValues = new float[] { 3f, 4f, 0f }; // wait, if targetLength=5, and index 1 is locked to 4. 
            // lockedLength = 4. UnlockedLength needed = sqrt(5^2 - 4^2) = 3.
            // If we set index 0 to 3, then unlockedLength becomes 3. 
            // So index 2 should stay 0.
            CollectionAssert.AreEqual(shouldValues, vector.ValuesCopy, $"{vector} != {shouldValues.ToFormattedString()}");
        }

        [Test]
        public void RandomisedTests()
        {
            /* !!!ZyKa VectorConstruction: write an extra test for vector construction */
            /* !!!ZyKa VectorConstruction: see screenshot */
            var random = new System.Random(42);
            for (int i = 0; i < 250; i++)
            {
                var vector = LockableVector.CreateRandom(
                    minDimension: 2, maxDimension: 5,
                    minValue: -10f, maxValue: 10f,
                    minTargetLength: 0.1f, maxTargetLength: 10.1f,
                    enforceLength: true,
                    random: random
                );
                if (vector == null)
                {
                    continue; 
                }

                Assert.AreEqual(vector.GetTargetLength(), vector.Magnitude(), 0.01f,
                    $"Iteration {i}, RandomVectorConstruction Failed {vector.ToLongString()}: \nMagnitude {vector.Magnitude()} != TargetLength {vector.GetTargetLength()}");
                
                int dimensions = vector.ValuesCopy.Length;

                bool lastActionSucceeded = true; 
                LockableVector preChange = new LockableVector(vector); 
                for (int j = 0; j < 250; j++)
                {
                    preChange = new LockableVector(vector);
                    
                    int action = random.Next(0, 5);
                    switch (action)
                    {
                        case 0: // Set a random value
                            int indexVal = random.Next(0, dimensions);
                            float newValue = (float)random.NextDouble() * 10f;
                            lastActionSucceeded = vector.SetValue(indexVal, newValue, ELockableValueForceSetBehaviour.Force);
                            break;
                        case 1: // Set a random lock
                            int indexLock = random.Next(0, dimensions);
                            bool newLock = random.Next(0, 2) == 1;
                            vector.SetLock(indexLock, newLock);
                            break;
                        case 2: // Set a random targetLength
                            float newTargetLength = (float)random.NextDouble() * 10f + 0.1f;
                            vector.GetLockedAndUnlockedLength(out float lockedLength, out float unlockedLength);
                            if (newTargetLength < lockedLength - 0.0001f)
                            {
                                newTargetLength += lockedLength; 
                            }
                            lastActionSucceeded = vector.SetTargetLength(newTargetLength);
                            break;
                        case 3: // Set a random enforceLength
                            lastActionSucceeded = vector.SetAutoNormalizeToTargetLength(random.Next(0, 2) == 1);
                            break;
                        case 4: //Set a random value to 0
                            indexVal = random.Next(0, dimensions);
                            lastActionSucceeded = vector.SetValue(indexVal, 0f, ELockableValueForceSetBehaviour.Force);
                            break; 
                    }
                    
                    if (lastActionSucceeded && vector.GetAutoNormalizeToTargetLength())
                    {
                        Assert.AreEqual(vector.GetTargetLength(), vector.Magnitude(), 0.01f, 
                            $"Iteration {i}, Action {j}, ActionType {action}, \npreChange {preChange.ToLongString()}, \npostChange {vector.ToLongString()}: \nMagnitude {vector.Magnitude()} != TargetLength {vector.GetTargetLength()}");
                    }
                }
            }
        }
    }
}
