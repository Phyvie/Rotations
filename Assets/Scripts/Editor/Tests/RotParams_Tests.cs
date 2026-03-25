using NUnit.Framework;
using RotParams;
using UnityEngine;

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
        
        RotParams_AxisAngle AxisAngle1 = new RotParams_AxisAngle(new Vector3(0.5f, 0.7071068f, 0), 3*Mathf.PI / 2.0f);
        RotParams_Quaternion Quaternion1 = new RotParams_Quaternion(new Vector3(0.5f, 0.7071068f, 0), 3*Mathf.PI / 2.0f);
        RotParams_Matrix Matrix1 = new RotParams_Matrix(new float[3, 3]
        {
            { 0.0000000f, -0.7071068f, -0.7071068f },
            { -0.7071068f, 0.5000000f, -0.5000000f },
            { 0.7071068f, 0.5000000f, -0.5000000f }
        }); 
        
        RotParams_AxisAngle AxisAngle2 = new RotParams_AxisAngle(new Vector3(0, -0.6f, -0.8f), Mathf.PI/6);
        RotParams_Quaternion Quaternion2 = new RotParams_Quaternion(new Vector3(0, -0.6f, -0.8f), Mathf.PI/6);
        RotParams_Matrix Matrix2 = new RotParams_Matrix(new float[3, 3]
        {
            { 0.1542515f, -0.7904253f, 0.5928190f },
            { 0.7904253f, 0.4587209f, 0.4059593f },
            { -0.5928190f, 0.4059593f, 0.6955305f }
        }); 

        RotParams_AxisAngle AxisAngle3 = new RotParams_AxisAngle(new Vector3(0.7071068f, -0.5f, 0.5f), Mathf.PI);
        RotParams_Quaternion Quaternion3 = new RotParams_Quaternion(new Vector3(0.7071068f, -0.5f, 0.5f), Mathf.PI);
        RotParams_Matrix Matrix3 = new RotParams_Matrix(new float[3, 3]
        {
            { 0.0000000f, -0.7071068f, 0.7071068f },
            { -0.7071068f, -0.5000000f, -0.5000000f },
            { 0.7071068f, -0.5000000f, -0.5000000f }
        }); 

        
        
        /*
        RotParams_AxisAngle AxisAngle = new RotParams_AxisAngle(new Vector3(0.5f, 0.5f, 1.0f/Mathf.Sqrt(2.0f)), Mathf.PI/3.0f);
        RotParams_Quaternion Quaternion = new RotParams_Quaternion(new Vector3(0.5f, 0.5f, 1.0f/Mathf.Sqrt(2.0f)), Mathf.PI/3.0f);
        RotParams_EulerAngles EulerAngles = new RotParams_EulerAngles(30.0f, -45.0f, 60.0f); 
        RotParams_Matrix Matrix = new RotParams_Matrix(new float[3,3]
        {
            {Mathf.Sqrt(3)/4.0f, -1.0f/4.0f, -Mathf.Sqrt(3)/4.0f}, 
            {1.0f/2.0f, Mathf.Sqrt(3)/2.0f, 0}, 
            {3.0f/4.0f, -Mathf.Sqrt(3)/4.0f, 1.0f/2.0f}
        });
        */
        
        #region Construction Tests
        [Test]
        public void RotParams_ConstructionTest_AxisAngle()
        {
            AxisAngle = new RotParams_AxisAngle(new Vector3(0.5f, 0.5f, 1.0f/Mathf.Sqrt(2.0f)), Mathf.PI/3.0f);
        }

        [Test]
        public void RotParams_ConstructionTest_Quaternion()
        {
            Quaternion = new RotParams_Quaternion(new Vector3(0.5f, 0.5f, 1.0f/Mathf.Sqrt(2.0f)), Mathf.PI/3.0f);
        }

        [Test]
        public void RotParams_ConstructionTest_EulerAngles()
        {
            EulerAngles = new RotParams_EulerAngles(30.0f, -45.0f, 60.0f); 
        }

        [Test]
        public void RotParams_ConstructionTest_Matrix()
        {
            Matrix = new RotParams_Matrix(new float[3,3]
            {
                {Mathf.Sqrt(3)/4.0f, -1.0f/4.0f, -Mathf.Sqrt(3)/4.0f}, 
                {1.0f/2.0f, Mathf.Sqrt(3)/2.0f, 0}, 
                {3.0f/4.0f, -Mathf.Sqrt(3)/4.0f, 1.0f/2.0f}
            });
        }
        #endregion
        
        #region Conversion Tests; Except EulerAngles
        [Test]
        public void ConversionTest_AxisAngle_Quaternion()
        {
            RotParams_AxisAngle AxisAngle_Start = new RotParams_AxisAngle(AxisAngle);
            RotParams_Quaternion Quaternion_Converted = AxisAngle_Start.ToQuaternionParams(); 
            RotParams_AxisAngle AxisAngle_BackConverted = Quaternion_Converted.ToAxisAngleParams();
            
            Assert.IsTrue(AxisAngle == AxisAngle_BackConverted, $"AxisAngle_Start: {AxisAngle_Start} != AxisAngle_BackConverted: {AxisAngle_BackConverted}");
            
            RotParams_Quaternion Quaternion_Start = new RotParams_Quaternion(Quaternion);
            RotParams_AxisAngle AxisAngle_Converted = Quaternion_Start.ToAxisAngleParams(); 
            RotParams_Quaternion Quaternion_BackConverted = AxisAngle_Converted.ToQuaternionParams();
            
            Assert.IsTrue(Quaternion == Quaternion_BackConverted, $"Quaternion_Start: {Quaternion_Start} != Quaternion_BackConverted: {Quaternion_BackConverted}");
        }

        [Test]
        public void ConversionTest_Quaternion_ToMatrix()
        {
            RotParams_Quaternion Quaternion_Start = new RotParams_Quaternion(new Vector3(0.7071068f, -0.5f, 0.5f), Mathf.PI);
            RotParams_Matrix Matrix_Converted = Quaternion_Start.ToMatrixParams();
            RotParams_Matrix ExpectedMatrix = new RotParams_Matrix(new float[3, 3]
            {
                { 0.0f, -0.7071068f, 0.7071068f },
                { -0.7071068f, -0.5f, -0.5f },
                { 0.7071068f, -0.5f, -0.5f }
            }); 
            Assert.IsTrue(Matrix_Converted == ExpectedMatrix, $"Matrix_Converted: {Matrix_Converted} != ExpectedMatrix: {ExpectedMatrix}");
        }

        public void ConversionTest_MatrixToQuaternion()
        {
            
        }
        
        [Test]
        public void ConversionTest_Quaternion_Matrix()
        {
            RotParams_Matrix Matrix_Start = new RotParams_Matrix(Matrix);
            RotParams_Quaternion Quaternion_Converted = Matrix_Start.ToQuaternionParams();
            RotParams_Matrix Matrix_BackConverted = Quaternion_Converted.ToMatrixParams();
            
            Assert.IsTrue(Matrix == Matrix_BackConverted, $"Matrix_Start: {Matrix_Start} != Matrix_BackConverted: {Matrix_BackConverted}");
            
            RotParams_Quaternion Quaternion_Start = new RotParams_Quaternion(Quaternion);
            RotParams_Matrix Matrix_Converted = Quaternion_Start.ToMatrixParams();
            Debug.Log("Matrix_Converted: " + Matrix_Converted);
            RotParams_Quaternion Quaternion_BackConverted = Matrix_Converted.ToQuaternionParams();
            
            Assert.IsTrue(Quaternion == Quaternion_BackConverted, $"Quaternion_Start: {Quaternion_Start} != Quaternion_BackConverted: {Quaternion_BackConverted}");
        }
        
        [Test]
        public void ConversionTest_AxisAngle_Matrix()
        {
            RotParams_AxisAngle AxisAngle_Start = new RotParams_AxisAngle(AxisAngle);
            RotParams_Matrix Matrix_Converted = AxisAngle_Start.ToMatrixParams();
            RotParams_AxisAngle AxisAngle_BackConverted = Matrix_Converted.ToAxisAngleParams();
            
            Assert.AreEqual(AxisAngle, AxisAngle_BackConverted, $"AxisAngle_Start: {AxisAngle_Start} != AxisAngle_BackConverted: {AxisAngle_BackConverted}");
            
            RotParams_Matrix Matrix_Start = new RotParams_Matrix(Matrix);
            RotParams_AxisAngle AxisAngle_Converted = Matrix_Start.ToAxisAngleParams();
            RotParams_Matrix Matrix_BackConverted = AxisAngle_Converted.ToMatrixParams();
            
            Assert.AreEqual(Matrix, Matrix_BackConverted, $"Matrix_Start: {Matrix_Start} != Matrix_BackConverted: {Matrix_BackConverted}");
        }
        #endregion
        
        #region Conversion Tests; EulerAngles
        [Test]
        public void ConversionTest_AxisAngle_EulerAngles()
        {
            RotParams_AxisAngle AxisAngle_Start = new RotParams_AxisAngle(AxisAngle);
            RotParams_EulerAngles EulerAngles_Converted = AxisAngle_Start.ToEulerParams();
            RotParams_AxisAngle AxisAngle_BackConverted = EulerAngles_Converted.ToAxisAngleParams();
            
            Assert.AreEqual(AxisAngle, AxisAngle_BackConverted, $"AxisAngle_Start: {AxisAngle_Start} != AxisAngle_BackConverted: {AxisAngle_BackConverted}");
            
            RotParams_EulerAngles EulerAngles_Start = new RotParams_EulerAngles(EulerAngles);
            RotParams_AxisAngle AxisAngle_Converted = EulerAngles_Start.ToAxisAngleParams();
            RotParams_EulerAngles EulerAngles_BackConverted = AxisAngle_Converted.ToEulerParams();
            
            Assert.AreEqual(EulerAngles, EulerAngles_BackConverted, $"EulerAngles_Start: {EulerAngles_Start} != EulerAngles_BackConverted: {EulerAngles_BackConverted}");
        }

        [Test]
        public void ConversionTest_Quaternion_EulerAngles()
        {
            RotParams_Quaternion Quaternion_Start = new RotParams_Quaternion(Quaternion);
            RotParams_EulerAngles EulerAngles_Converted = Quaternion_Start.ToEulerParams();
            RotParams_Quaternion Quaternion_BackConverted = EulerAngles_Converted.ToQuaternionParams();
            
            Assert.AreEqual(Quaternion, Quaternion_BackConverted, $"Quaternion_Start: {Quaternion_Start} != Quaternion_BackConverted: {Quaternion_BackConverted}");
            
            RotParams_EulerAngles EulerAngles_Start = new RotParams_EulerAngles(EulerAngles);
            RotParams_Quaternion Quaternion_Converted = EulerAngles_Start.ToQuaternionParams();
            RotParams_EulerAngles EulerAngles_BackConverted = Quaternion_Converted.ToEulerParams();
            
            Assert.AreEqual(EulerAngles, EulerAngles_BackConverted, $"EulerAngles_Start: {EulerAngles_Start} != EulerAngles_BackConverted: {EulerAngles_BackConverted}");
        }

        [Test]
        public void ConversionTest_EulerAngles_Matrix()
        {
            RotParams_EulerAngles EulerAngles_Start = new RotParams_EulerAngles(EulerAngles);
            RotParams_Matrix Matrix_Converted = EulerAngles_Start.ToMatrixParams();
            RotParams_EulerAngles EulerAngles_BackConverted = Matrix_Converted.ToEulerParams();
            
            Assert.AreEqual(EulerAngles, EulerAngles_BackConverted, $"EulerAngles_Start: {EulerAngles_Start} != EulerAngles_BackConverted: {EulerAngles_BackConverted}");
            
            RotParams_Matrix Matrix_Start = new RotParams_Matrix(Matrix);
            RotParams_EulerAngles EulerAngles_Converted = Matrix_Start.ToEulerParams();
            RotParams_Matrix Matrix_BackConverted = EulerAngles_Converted.ToMatrixParams();
            
            Assert.AreEqual(Matrix, Matrix_BackConverted, $"Matrix_Start: {Matrix_Start} != Matrix_BackConverted: {Matrix_BackConverted}");
        }
        #endregion 
    }
}