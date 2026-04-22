using NUnit.Framework;
using RotParams;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Editor.Tests
{
    public class RotParams_Tests
    {
        RotParams_AxisAngle AxisAngle0 = new RotParams_AxisAngle(new Vector3(0.7071068f, -0.5f, 0.5f), Mathf.PI);
        RotParams_Quaternion Quaternion0 = new RotParams_Quaternion(new Vector3(0.7071068f, -0.5f, 0.5f), Mathf.PI);
        RotParams_Matrix Matrix0 = new RotParams_Matrix(new float[3, 3]
        {
            { 0.0000000f, -0.7071068f, 0.7071068f },
            { -0.7071068f, -0.5000000f, -0.5000000f },
            { 0.7071068f, -0.5000000f, -0.5000000f }
        });
        RotParams_EulerAngles EulerAngles0_YXZ = new RotParams_EulerAngles(Mathf.Deg2Rad * 125.2643901f, Mathf.Deg2Rad * 30.0f, Mathf.Deg2Rad * -125.2643901f);
        
        RotParams_AxisAngle AxisAngle1 = new RotParams_AxisAngle(new Vector3(-0.5f, 0.8164966f, 0), 2*Mathf.PI / 3.0f);
        RotParams_Quaternion Quaternion1 = new RotParams_Quaternion(new Vector3(-0.5f, 0.8164966f, 0), 2*Mathf.PI / 3.0f);
        RotParams_Matrix Matrix1 = new RotParams_Matrix(new float[3, 3]
        {
            { -0.0909091f, -0.6680427f, 0.7385489f },
            { -0.6680427f, 0.5909091f, 0.4522670f },
            { -0.7385489f, -0.4522670f, -0.5000000f }
        });
        RotParams_EulerAngles EulerAngles1_YXZ = new RotParams_EulerAngles(Mathf.Deg2Rad * 124.0981264f, Mathf.Deg2Rad * -26.889227f, Mathf.Deg2Rad * -48.5060188f); 
        
        RotParams_AxisAngle AxisAngle2 = new RotParams_AxisAngle(new Vector3(0, -0.6f, -0.8f), 3*Mathf.PI/4);
        RotParams_Quaternion Quaternion2 = new RotParams_Quaternion(new Vector3(0, -0.6f, -0.8f), 3*Mathf.PI/4);
        RotParams_Matrix Matrix2 = new RotParams_Matrix(new float[3, 3]
        {
            { -0.7071068f, 0.5656855f, -0.4242641f },
            { -0.5656855f, -0.0925483f, 0.8194113f },
            { 0.4242641f, 0.8194113f, 0.3854416f }
        });
        RotParams_EulerAngles EulerAngles2_YXZ = new RotParams_EulerAngles(Mathf.Deg2Rad * -47.7450264f, Mathf.Deg2Rad * -55.0259038f, Mathf.Deg2Rad * -99.2914965f);

        RotParams_AxisAngle AxisAngle3 = new RotParams_AxisAngle(new Vector3(0.7071068f, -0.5f, 0.5f), Mathf.PI * 2 / 3);
        RotParams_Quaternion Quaternion3 = new RotParams_Quaternion(new Vector3(0.7071068f, -0.5f, 0.5f), Mathf.PI * 2 / 3);
        RotParams_Matrix Matrix3 = new RotParams_Matrix(new float[3, 3]
        {
            { 0.2500000f, -0.9633428f, 0.0973174f },
            { -0.0973174f, -0.1250000f, -0.9873725f },
            { 0.9633428f, 0.2373725f, -0.1250000f }
        });
        private RotParams_EulerAngles EulerAngles3_YXZ = new RotParams_EulerAngles(Mathf.Deg2Rad * 142.09785f, Mathf.Deg2Rad * 80.8850328f, Mathf.Deg2Rad * -142.09785f); 

        #region Construction Tests
        [Test]
        public void RotParams_ConstructionTest_AxisAngle()
        {
            
        }

        [Test]
        public void RotParams_ConstructionTest_Quaternion()
        {
            
        }

        [Test]
        public void RotParams_ConstructionTest_EulerAngles()
        {
            
        }

        [Test]
        public void RotParams_ConstructionTest_Matrix()
        {
            
        }
        #endregion
        
        #region Conversion Tests; Except EulerAngles
        [Test]
        public void ConversionTest_AxisAngle_Quaternion()
        {
            Assert.AreEqual(AxisAngle0.ToQuaternionParams(), Quaternion0, "AxisAngle0 to Quaternion0 failed"); 
            Assert.AreEqual(AxisAngle1.ToQuaternionParams(), Quaternion1, "AxisAngle1 to Quaternion1 failed"); 
            Assert.AreEqual(AxisAngle2.ToQuaternionParams(), Quaternion2, "AxisAngle2 to Quaternion2 failed"); 
            Assert.AreEqual(AxisAngle3.ToQuaternionParams(), Quaternion3, "AxisAngle3 to Quaternion3 failed");
            
            Assert.AreEqual(Quaternion0.ToAxisAngleParams(), AxisAngle0, "Quaternion0 to AxisAngle0 failed"); 
            Assert.AreEqual(Quaternion1.ToAxisAngleParams(), AxisAngle1, "Quaternion1 to AxisAngle1 failed"); 
            Assert.AreEqual(Quaternion2.ToAxisAngleParams(), AxisAngle2, "Quaternion2 to AxisAngle2 failed"); 
            Assert.AreEqual(Quaternion3.ToAxisAngleParams(), AxisAngle3, "Quaternion3 to AxisAngle3 failed");
        }

        [Test]
        public void ConversionTest_Quaternion_Matrix()
        {
            Assert.AreEqual(Matrix0.ToQuaternionParams(), Quaternion0, "Matrix0 to Quaternion0 failed"); 
            Assert.AreEqual(Matrix1.ToQuaternionParams(), Quaternion1, "Matrix1 to Quaternion1 failed"); 
            Assert.AreEqual(Matrix2.ToQuaternionParams(), Quaternion2, "Matrix2 to Quaternion2 failed"); 
            Assert.AreEqual(Matrix3.ToQuaternionParams(), Quaternion3, "Matrix3 to Quaternion3 failed");
            
            Assert.AreEqual(Quaternion0.ToMatrixParams(), Matrix0, "Quaternion0 to Matrix0 failed"); 
            Assert.AreEqual(Quaternion1.ToMatrixParams(), Matrix1, "Quaternion1 to Matrix1 failed"); 
            Assert.AreEqual(Quaternion2.ToMatrixParams(), Matrix2, "Quaternion2 to Matrix2 failed"); 
            Assert.AreEqual(Quaternion3.ToMatrixParams(), Matrix3, "Quaternion3 to Matrix3 failed");
        }
        
        [Test]
        public void ConversionTest_AxisAngle_Matrix()
        {
            Assert.AreEqual(Matrix0.ToAxisAngleParams(), AxisAngle0, "Matrix0 to AxisAngle0 failed"); 
            Assert.AreEqual(Matrix1.ToAxisAngleParams(), AxisAngle1, "Matrix1 to AxisAngle1 failed"); 
            Assert.AreEqual(Matrix2.ToAxisAngleParams(), AxisAngle2, "Matrix2 to AxisAngle2 failed"); 
            Assert.AreEqual(Matrix3.ToAxisAngleParams(), AxisAngle3, "Matrix3 to AxisAngle3 failed");
            
            Assert.AreEqual(AxisAngle0.ToMatrixParams(), Matrix0, "AxisAngle0 to Matrix0 failed"); 
            Assert.AreEqual(AxisAngle1.ToMatrixParams(), Matrix1, "AxisAngle1 to Matrix1 failed"); 
            Assert.AreEqual(AxisAngle2.ToMatrixParams(), Matrix2, "AxisAngle2 to Matrix2 failed"); 
            Assert.AreEqual(AxisAngle3.ToMatrixParams(), Matrix3, "AxisAngle3 to Matrix3 failed");
        }

        #region Randomised Tests
        [Test]
        public void ConversionTest_AxisAngle_Quaternion_Randomised()
        {
            UnityEngine.Random.InitState(42);
            for (int i = 0; i < 100; i++)
            {
                RotParams_AxisAngle axisAngle = new RotParams_AxisAngle(Random.insideUnitSphere, Random.Range(0.001f, 2*Mathf.PI));
                Assert.AreEqual(axisAngle, axisAngle.ToQuaternionParams().ToAxisAngleParams(), $"AxisAngle to Quaternion to AxisAngle failed at iteration {i}");
            }
        }
        
        [Test]
        public void ConversionTest_Quaternion_AxisAngle_Randomised()
        {
            UnityEngine.Random.InitState(42);
            for (int i = 0; i < 100; i++)
            {
                RotParams_Quaternion quaternion = new RotParams_Quaternion(Random.insideUnitSphere, Random.Range(0.001f, 2*Mathf.PI));
                Assert.AreEqual(quaternion, quaternion.ToAxisAngleParams().ToQuaternionParams(), $"Quaternion to AxisAngle to Quaternion failed at iteration {i}");
            }
        }

        [Test]
        public void ConversionTest_Quaternion_Matrix_Randomised()
        {
            UnityEngine.Random.InitState(42);
            for (int i = 0; i < 100; i++)
            {
                RotParams_Quaternion quaternion = new RotParams_Quaternion(Random.insideUnitSphere, Random.Range(0.001f, 2*Mathf.PI));
                Assert.AreEqual(quaternion, quaternion.ToMatrixParams().ToQuaternionParams(), $"Quaternion to Matrix to Quaternion failed at iteration {i}");
            }
        }

        [Test]
        public void ConversionTest_Matrix_Quaternion_Randomised()
        {
            UnityEngine.Random.InitState(42);
            for (int i = 0; i < 100; i++)
            {
                RotParams_Matrix matrix = new RotParams_Matrix(Random.insideUnitSphere, Random.insideUnitSphere, Random.insideUnitSphere);
                matrix = matrix.ToRotationMatrixFromTwoAxes(0, 1); 
                Assert.AreEqual(matrix, matrix.ToQuaternionParams().ToMatrixParams(), $"Matrix to Quaternion to Matrix failed at iteration {i}");
            }
        }
        
        
        [Test]
        public void ConversionTest_AxisAngle_Matrix_Randomised()
        {
            UnityEngine.Random.InitState(42);
            for (int i = 0; i < 100; i++)
            {
                RotParams_AxisAngle AxisAngle = new RotParams_AxisAngle(Random.insideUnitSphere, Random.Range(0.001f, 2*Mathf.PI));
                Assert.AreEqual(AxisAngle, AxisAngle.ToMatrixParams().ToAxisAngleParams(), $"AxisAngle to Matrix to AxisAngle failed at iteration {i}");
            }
        }

        [Test]
        public void ConversionTest_Matrix_AxisAngle_Randomised()
        {
            UnityEngine.Random.InitState(42);
            for (int i = 0; i < 100; i++)
            {
                RotParams_Matrix matrix = new RotParams_Matrix(Random.insideUnitSphere, Random.insideUnitSphere, Random.insideUnitSphere);
                matrix = matrix.ToRotationMatrixFromTwoAxes(0, 1); 
                Assert.AreEqual(matrix, matrix.ToAxisAngleParams().ToMatrixParams(), $"Matrix to AxisAngle to Matrix failed at iteration {i}");
            }
        }
        #endregion Randomised Tests
        #endregion 
        
        #region Conversion Tests; EulerAngles
        #region defined tests
        [Test]
        public void ConversionTest_EulerAngles_ToQuaternion()
        {
            Assert.AreEqual(EulerAngles0_YXZ.ToQuaternionParams(), Quaternion0, "EulerAngles0 to Quaternion0 failed"); 
            Assert.AreEqual(EulerAngles1_YXZ.ToQuaternionParams(), Quaternion1, "EulerAngles1 to Quaternion1 failed"); 
            Assert.AreEqual(EulerAngles2_YXZ.ToQuaternionParams(), Quaternion2, "EulerAngles2 to Quaternion2 failed"); 
            Assert.AreEqual(EulerAngles3_YXZ.ToQuaternionParams(), Quaternion3, "EulerAngles3 to Quaternion3 failed"); 
        }

        [Test]
        public void ConversionTest_EulerAngles_ToAxisAngle()
        {
            Assert.AreEqual(EulerAngles0_YXZ.ToAxisAngleParams(), AxisAngle0, "EulerAngles0 to AxisAngle0 failed"); 
            Assert.AreEqual(EulerAngles1_YXZ.ToAxisAngleParams(), AxisAngle1, "EulerAngles1 to AxisAngle1 failed"); 
            Assert.AreEqual(EulerAngles2_YXZ.ToAxisAngleParams(), AxisAngle2, "EulerAngles2 to AxisAngle2 failed"); 
            Assert.AreEqual(EulerAngles3_YXZ.ToAxisAngleParams(), AxisAngle3, "EulerAngles3 to AxisAngle3 failed"); 
        }

        [Test]
        public void ConversionTest_EulerAngles_ToMatrix()
        {
            Assert.AreEqual(EulerAngles0_YXZ.ToMatrixParams(), Matrix0, "EulerAngles0 to Matrix0 failed"); 
            Assert.AreEqual(EulerAngles1_YXZ.ToMatrixParams(), Matrix1, "EulerAngles1 to Matrix1 failed"); 
            Assert.AreEqual(EulerAngles2_YXZ.ToMatrixParams(), Matrix2, "EulerAngles2 to Matrix2 failed"); 
            Assert.AreEqual(EulerAngles3_YXZ.ToMatrixParams(), Matrix3, "EulerAngles3 to Matrix3 failed"); 
        }

        [Test]
        public void ConversionTest_Quaternion_ToEulerAngles()
        {
            Assert.AreEqual(EulerAngles0_YXZ, Quaternion0.ToEulerParams(), "Quaternion0 to EulerAngles0 failed"); 
            Assert.AreEqual(EulerAngles1_YXZ, Quaternion1.ToEulerParams(), "Quaternion1 to EulerAngles1 failed"); 
            Assert.AreEqual(EulerAngles2_YXZ, Quaternion2.ToEulerParams(), "Quaternion2 to EulerAngles2 failed"); 
            Assert.AreEqual(EulerAngles3_YXZ, Quaternion3.ToEulerParams(), "Quaternion3 to EulerAngles3 failed"); 
        }
        
        [Test]
        public void ConversionTest_Matrix_ToEulerAngles()
        {
            Assert.AreEqual(EulerAngles0_YXZ, Matrix0.ToEulerParams(), "Matrix0 to EulerAngles0 failed"); 
            Assert.AreEqual(EulerAngles1_YXZ, Matrix1.ToEulerParams(), "Matrix1 to EulerAngles1 failed"); 
            Assert.AreEqual(EulerAngles2_YXZ, Matrix2.ToEulerParams(), "Matrix2 to EulerAngles2 failed"); 
            Assert.AreEqual(EulerAngles3_YXZ, Matrix3.ToEulerParams(), "Matrix3 to EulerAngles3 failed"); 
        }
        
        [Test]
        public void ConversionTest_AxisAngle_ToEulerAngles()
        {
            Assert.AreEqual(EulerAngles0_YXZ, AxisAngle0.ToEulerParams(), "AxisAngle0 to EulerAngles0 failed"); 
            Assert.AreEqual(EulerAngles1_YXZ, AxisAngle1.ToEulerParams(), "AxisAngle1 to EulerAngles1 failed"); 
            Assert.AreEqual(EulerAngles2_YXZ, AxisAngle2.ToEulerParams(), "AxisAngle2 to EulerAngles2 failed"); 
            Assert.AreEqual(EulerAngles3_YXZ, AxisAngle3.ToEulerParams(), "AxisAngle3 to EulerAngles3 failed"); 
        }
        #endregion defined tests
        #region randomised tests

        [Test]
        public void ConversionTest_Quaternion_EulerAngles_Quaternion_Randomised()
        {
            Random.InitState(42);

            for (int gimbalNumber = 1; gimbalNumber <= 12; gimbalNumber++)
            {
                for (int i = 0; i < 100; i++)
                {
                    RotParams_Quaternion quaternion =
                        new RotParams_Quaternion(Random.insideUnitSphere, Random.Range(0.001f, 2 * Mathf.PI));
                    RotParams_EulerAngles eulerAngles = new RotParams_EulerAngles((EGimbalOrder)gimbalNumber);
                    Assert.AreEqual(quaternion, quaternion.ToEulerParams(eulerAngles).ToQuaternionParams(),
                        $"Quaternion to EulerAngles to Quaternion failed at gimbalNumber {gimbalNumber} at iteration {i}, with EulerGimbal: {eulerAngles}");
                }
            }
        }

        [Test]
        public void ConversionTest_Matrix_EulerAngles_Matrix_Randomised()
        {
            Random.InitState(42);

            for (int gimbalNumber = 1; gimbalNumber <= 12; gimbalNumber++)
            {
                for (int i = 0; i < 100; i++)
                {
                    RotParams_Matrix matrix = new RotParams_Matrix(Random.insideUnitSphere, Random.insideUnitSphere, Random.insideUnitSphere);
                    matrix = matrix.ToRotationMatrixFromTwoAxes(0, 1);
                    RotParams_EulerAngles eulerAngles = new RotParams_EulerAngles((EGimbalOrder)gimbalNumber); 
                    matrix.ToEulerParams(eulerAngles);
                    RotParams_Matrix resultMatrix = eulerAngles.ToMatrixParams(); 
                    Assert.AreEqual(matrix, resultMatrix, $"Matrix to EulerAngles to Matrix failed at gimbalNumber {gimbalNumber} at iteration {i}, with EulerGimbal: {eulerAngles}");
                }
            }
        }
        #endregion
        #endregion 
    }
}