using MathExtensions;
using NUnit.Framework;
using UnityEngine;

namespace Editor
{
    public class MathFunctionsTests
    {
        [Test]
        public void LockableVector_SubtractLengthSquaredManual_Test()
        {
            // 13^2 - 12^2 = 169 - 144 = 25
            Assert.AreEqual(25f, SubtractLengthSquaredManual(169f, 144f), 0.001f, $"{nameof(LockableVector_SubtractLengthSquaredManual_Test)} failed first");
            // 13^2 - 5^2 = 169 - 25 = 144
            Assert.AreEqual(144f, SubtractLengthSquaredManual(169f, 25f), 0.001f, $"{nameof(LockableVector_SubtractLengthSquaredManual_Test)} failed second");
            // Edge case: target < subtract should return -1
            Assert.AreEqual(-1f, SubtractLengthSquaredManual(4f, 9f), 0.001f, $"{nameof(LockableVector_SubtractLengthSquaredManual_Test)} failed third");
        }

        private float SubtractLengthSquaredManual(float targetLengthSquared, float subtractLengthSquared)
        {
            float resultSquared = targetLengthSquared - subtractLengthSquared;
            if (resultSquared < -0.0001f) return -1.0f;
            return Mathf.Max(0, resultSquared);
        }

        [Test]
        public void LockableVector_SubtractLengthPythagoreon_Test()
        {
            // sqrt(13^2 - 12^2) = sqrt(169 - 144) = sqrt(25) = 5
            Assert.AreEqual(5f, MathFunctions.SubtractMagnitudePythagoreon(13f, 12f), 0.001f, $"{nameof(LockableVector_SubtractLengthPythagoreon_Test)} failed first");
            // sqrt(13^2 - 5^2) = sqrt(169 - 25) = sqrt(144) = 12
            Assert.AreEqual(12f, MathFunctions.SubtractMagnitudePythagoreon(13f, 5f), 0.001f, $"{nameof(LockableVector_SubtractLengthPythagoreon_Test)} failed second");
            // Edge case: a < b should return -1 or warn
            // Currently it checks a - b < 0, but sqrt(a^2 - b^2) is what it calculates
            // If a = 2, b = 3: 2-3 = -1 < 0 -> returns -1
            Assert.AreEqual(-1f, MathFunctions.SubtractMagnitudePythagoreon(2f, 3f), 0.001f, $"{nameof(LockableVector_SubtractLengthPythagoreon_Test)} failed third");
        }
    }
}