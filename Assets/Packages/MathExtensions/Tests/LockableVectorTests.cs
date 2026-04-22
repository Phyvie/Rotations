using System.Collections;
using System.Linq;
using Extensions.MathExtensions;
using NUnit.Framework;
using UnityEngine;

namespace Packages.MathExtensions.Tests
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
        public void LockableVector_ConstructionTest_ShouldFail()
        {
            LockableVector lockableVector0 = LockableVector.SafeCreateLockableVector(new float[] { 1f, 1f }, new bool[] { true, true }, 2f, true);
            LockableVector lockableVector1 = LockableVector.SafeCreateLockableVector(new float[] { 3f, 0f }, new bool[] { true, false }, 2f, true);
            
            Assert.IsNull(lockableVector0);
            Assert.IsNull(lockableVector1);
        }
        
        [Test]
        public void LockableVector_ConstructionNormalizationTest()
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
        public void LockableVector_RandomConstructionTest()
        {
            var random = new System.Random(42);
            for (int i = 0; i < 500; i++)
            {
                LockableVector lockableVector = LockableVector.CreateRandom(
                    minDimension: random.Next(3), maxDimension: random.Next(6),
                    maxNegValue: (float)(-random.NextDouble() * random.Next(10)), 
                    maxPosValue: (float)(random.NextDouble() * random.Next(10)),
                    minTargetMagnitude: (float)(random.NextDouble() * random.Next(10)), 
                    maxTargetMagnitude: (float)(random.NextDouble() * random.Next(10)),
                    autoNormalizeToTargetMagnitude: random.NextDouble() > 0.5,
                    random: random
                );
                if (lockableVector is not null && lockableVector.GetAutoNormalizeToTargetMagnitude())
                {
                    Assert.AreEqual(lockableVector.Magnitude, lockableVector.GetTargetMagnitude(), 0.01f, $"Random Vector Generation failed: {lockableVector}");
                }
            }
        }
        
        [Test]
        public void LockableVector_GetLockedAndUnlockedMagnitude_CorrectlyCalculates()
        {
            var values = new float[] { 3f, 4f, 12f };
            var locks = new bool[] { false, false, true };
            
            var vector = LockableVector.SafeCreateLockableVector(values, locks, 13f, true);
            vector.GetLockedAndUnlockedMagnitude(out float locked, out float unlocked);
            
            Assert.AreEqual(5f, unlocked, 0.001f);
            Assert.AreEqual(12f, locked, 0.001f);
            Assert.AreEqual(13f, vector.Magnitude, 0.001f);
        }

        [Test]
        public void LockableVector_SetValue_RenormalizesCorrectly()
        {
            var values = new float[] { 0f, 5f, 12f };
            var locks = new bool[] { false, false, true };
            
            var vector = LockableVector.SafeCreateLockableVector(values, locks, 13f, true);
            vector.SetValue(0, 3.0f);
            float[] shouldValues = new float[] { 3f, 4f, 12f };
            CollectionAssert.AreEqual(shouldValues, vector.ValuesCopy, $"vector: {vector} != expectedVector: {shouldValues.ToFormattedString()}");
        }

        [Test]
        public void RandomVectorConstruction()
        {
            var random = new System.Random(42);
            for (int i = 0; i < 250; i++)
            {
                var vector = LockableVector.CreateRandom(
                    minDimension: 2, maxDimension: 5,
                    maxNegValue: -10f, maxPosValue: 10f,
                    minTargetMagnitude: 0.1f, maxTargetMagnitude: 10.1f,
                    autoNormalizeToTargetMagnitude: true,
                    random: random
                );
                if (vector == null)
                {
                    continue;
                }

                Assert.AreEqual(vector.GetTargetMagnitude(), vector.Magnitude, 0.01f,
                    $"Iteration {i}, RandomVectorConstruction Failed {vector.ToLongString()}: \nMagnitude {vector.Magnitude} != TargetMagnitude {vector.GetTargetMagnitude()}");
            }
        }

        [Test]
        public void LockableVector_Normalization_NoUnlockedParts()
        {
            var values = new float[] { 10f };
            var locks = new bool[] { true };
            // targetMagnitude = 10, lockedMagnitude = 10. Should succeed.
            var vector = LockableVector.SafeCreateLockableVector(values, locks, 10f, true);
            Assert.IsNotNull(vector);
            Assert.AreEqual(10f, vector.Magnitude, 0.001f);

            // targetMagnitude = 5, lockedMagnitude = 10. Should fail.
            var vector2 = LockableVector.SafeCreateLockableVector(new float[] { 10f }, new bool[] { true }, 5f, true);
            Assert.IsNull(vector2);
        }

        [Test]
        public void LockableVector_Normalization_AllUnlockedZero()
        {
            var values = new float[] { 0f, 0f };
            var locks = new bool[] { false, false };
            var vector = LockableVector.SafeCreateLockableVector(values, locks, 5f, true);
            // Should set the first unlocked component to 5
            Assert.AreEqual(5f, vector.Magnitude, 0.001f);
            Assert.AreEqual(5f, vector.GetValueAtIndex(0), 0.001f);
            Assert.AreEqual(0f, vector.GetValueAtIndex(1), 0.001f);
        }

        [Test]
        public void LockableVector_SetTargetMagnitude_UpdatesMagnitude()
        {
            var vector = LockableVector.SafeCreateLockableVector(new float[] { 3f, 4f }, new bool[] { false, false }, 5f, true);
            Assert.AreEqual(5f, vector.Magnitude, 0.001f);
            
            bool success = vector.SetTargetMagnitude(10f);
            Assert.IsTrue(success);
            Assert.AreEqual(10f, vector.Magnitude, 0.001f);
            // 3:4 ratio should be preserved: 6:8
            Assert.AreEqual(6f, vector.GetValueAtIndex(0), 0.001f);
            Assert.AreEqual(8f, vector.GetValueAtIndex(1), 0.001f);
        }

        [Test]
        public void RandomizedTests()
        {
            var random = new System.Random(40);
            for (int i = 0; i < 10; i++)
            {
                var vector = LockableVector.CreateRandom(
                    minDimension: 2, maxDimension: 5,
                    maxNegValue: -10f, maxPosValue: 10f,
                    minTargetMagnitude: 0.1f, maxTargetMagnitude: 10.1f,
                    autoNormalizeToTargetMagnitude: true,
                    random: random
                );
                if (vector == null)
                {
                    continue; 
                }

                Assert.AreEqual(vector.GetTargetMagnitude(), vector.Magnitude, 0.01f,
                    $"Iteration {i}, RandomVectorConstruction Failed {vector.ToLongString()}: \nMagnitude {vector.Magnitude} != TargetMagnitude {vector.GetTargetMagnitude()}");
                
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
                        case 2: // Set a random targetMagnitude
                            float newTargetMagnitude = (float)random.NextDouble() * 10f + 0.1f;
                            vector.GetLockedAndUnlockedMagnitude(out float lockedMagnitude, out float unlockedMagnitude);
                            if (newTargetMagnitude < lockedMagnitude - 0.0001f)
                            {
                                newTargetMagnitude += lockedMagnitude; 
                            }
                            lastActionSucceeded = vector.SetTargetMagnitude(newTargetMagnitude);
                            break;
                        case 3: // Set a random enforceMagnitude
                            lastActionSucceeded = vector.SetAutoNormalizeToTargetMagnitude(random.Next(0, 2) == 1);
                            break;
                        case 4: //Set a random value to 0
                            indexVal = random.Next(0, dimensions);
                            lastActionSucceeded = vector.SetValue(indexVal, 0f, ELockableValueForceSetBehaviour.Force);
                            break; 
                    }
                    
                    if (lastActionSucceeded && vector.GetAutoNormalizeToTargetMagnitude())
                    {
                        Assert.AreEqual(vector.GetTargetMagnitude(), vector.Magnitude, 0.01f, 
                            $"Iteration {i}, Action {j}, ActionType {action}, \npreChange {preChange.ToLongString()}, \npostChange {vector.ToLongString()}: \nMagnitude {vector.Magnitude} != TargetMagnitude {vector.GetTargetMagnitude()}");
                    }
                }
            }
        }
    }
}