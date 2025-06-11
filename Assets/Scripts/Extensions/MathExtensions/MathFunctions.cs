using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace MathExtensions
{
    public static class MathFunctions
    {
        public static Vector3 GetOrthogonalisedVector(this Vector3 reference, Vector3 toOrthogonalise)
        {
            return Vector3.Cross(Vector3.Cross(reference, toOrthogonalise), reference); 
        }
        
        public static float RangeModulo(float value, float2 range)
        {
            float min = Mathf.Min(range.x, range.y);
            float max = Mathf.Max(range.x, range.y);
            return Mathf.Repeat(value + min, max - min) + min; 
        }

        public static readonly Quaternion CylicQuaternion = Quaternion.LookRotation(Vector3.right, Vector3.forward);

        public static Vector3 CyclicAxisRotation(this Vector3 v)
        {
            return new Vector3(v.y, v.z, v.x); 
        }

        public static Vector3 ToRotationVector(this Quaternion a)
        {
            a.ToAngleAxis(out float angle, out Vector3 axis); 
            return axis * angle; 
        }

        public static Quaternion RotationVectorToQuaternion(this Vector3 a)
        {
            return Quaternion.AngleAxis(a.magnitude, a.normalized);
        }
        
        public static Quaternion InterpolateAsRotationVectors(Quaternion a, Quaternion b, float weightA, float weightB, bool normaliseWeights = true)
        {
            Vector3 lerpedRotationVector =  a.ToRotationVector() * weightA + b.ToRotationVector() * weightB;
            if (normaliseWeights)
            {
                float weightSum = weightA + weightB;
                lerpedRotationVector /= weightSum;
            }
            return lerpedRotationVector.RotationVectorToQuaternion();
        }

        //-ZyKa should I really have this in here instead of my own RotParams_Quaternion class?
        public static Quaternion QuaternionInterpolateAsRotationVectors(Quaternion[] quats, float[] weights, bool normaliseWeights = true)
        {
            Vector3 lerpedRotationVector = Vector3.zero;
            if (quats.Length != weights.Length)
            {
                Debug.LogError($"quats.Length = {quats.Length} != weights.Length = {weights.Length}"); 
                return new Quaternion(0, 0, 0, 0); 
            }
            for (int i = 0; i < quats.Length; i++)
            {
                lerpedRotationVector += quats[i].ToRotationVector() * weights[i]; 
            }
            if (normaliseWeights)
            {
                float weightSum = weights.Sum(); 
                lerpedRotationVector /= weightSum;
            }
            return lerpedRotationVector.RotationVectorToQuaternion(); 
        }

        #region Matrix
        #region floatArrayMatrix

        public static float[,] MatrixMultiply(this float[,] firstMatrix, float[,] secondMatrix)
        {
            int firstRows = firstMatrix.GetLength(0);
            int firstCols = firstMatrix.GetLength(1);
            int secondRows = secondMatrix.GetLength(0);
            int secondCols = secondMatrix.GetLength(1);
            if (firstCols != secondRows)
            {
                throw new InvalidOperationException(
                    "floathe number of columns in the first matrix must be equal to the number of rows in the second matrix.");
            }

            float[,] resultMatrix = new float[firstRows, secondCols];
            for (int i = 0; i < firstRows; i++)
            {
                for (int j = 0; j < secondCols; j++)
                {
                    resultMatrix[i, j] = 0;
                    for (int k = 0; k < firstCols; k++)
                    {
                        resultMatrix[i, j] += firstMatrix[i, k] * secondMatrix[k, j];
                    }
                }
            }

            return resultMatrix;
        }
        
        public static float[] MatrixMultiply(
            float[] firstMatrix, int firstHeight, int firstWidth,
            float[] secondMatrix, int secondHeight, int secondWidth)
        {
            if (firstWidth != secondHeight)
            {
                throw new InvalidOperationException(
                    "The number of columns in the first matrix must be equal to the number of rows in the second matrix.");
            }

            float[] resultMatrix = new float[firstHeight * secondWidth];
            for (int i = 0; i < firstHeight; i++)
            {
                for (int j = 0; j < secondWidth; j++)
                {
                    resultMatrix[i * secondWidth + j] = 0;
                    for (int k = 0; k < firstWidth; k++)
                    {
                        resultMatrix[i * secondWidth + j] +=
                            firstMatrix[i * firstWidth + k] * secondMatrix[k * secondWidth + j];
                    }
                }
            }

            return resultMatrix;
        }

        #endregion
        
        #region float3x3Matrix4x4
        public static void FromMatrix4x4(this float3x3 thisFloat3X3, Matrix4x4 otherMatrix)
        {
            thisFloat3X3.c0 = new float3(otherMatrix.m00, otherMatrix.m01, otherMatrix.m02); 
            thisFloat3X3.c1 = new float3(otherMatrix.m10, otherMatrix.m11, otherMatrix.m12); 
            thisFloat3X3.c2 = new float3(otherMatrix.m20, otherMatrix.m21, otherMatrix.m22);
        }
    
        public static float3x3 ToFloat3x3(this Matrix4x4 matrix)
        {
            float3x3 returnValue; 
            returnValue.c0 = new float3(matrix.m00, matrix.m01, matrix.m02); 
            returnValue.c1 = new float3(matrix.m10, matrix.m11, matrix.m12); 
            returnValue.c2 = new float3(matrix.m20, matrix.m21, matrix.m22);
            return returnValue; 
        }

        public static T[,] SwitchRows<T>(this T[,] array, int rowIndexA, int rowIndexB) //TODO: test
        {
            int height = array.GetLength(0); 
            int width = array.GetLength(1); 
            
            Debug.Assert(rowIndexA < height);
            Debug.Assert(rowIndexB < height); 
            
            T[] tempArray = new T[width];
            Array.Copy(array, rowIndexA * width, tempArray, 0, width);
            Array.Copy(array, rowIndexB * width, array, rowIndexA * width, width);
            Array.Copy(tempArray, 0, array, rowIndexB * width, width);
            
            return array; 
        }
        
        public static T[] SwitchRows<T>(this T[] array, int rowIndexA, int rowIndexB, int width)
        {
            Debug.Assert(rowIndexA * width < array.Length);
            Debug.Assert(rowIndexB * width < array.Length);

            T[] tempArray = new T[width];
            Array.Copy(array, rowIndexA * width, tempArray, 0, width);
            Array.Copy(array, rowIndexB * width, array, rowIndexA * width, width);
            Array.Copy(tempArray, 0, array, rowIndexB * width, width);
            
            return array; 
        }

        public static T[,] SwitchColumn<T>(this T[,] array, int columnIndexA, int columnIndexB) //TODO: test
        {
            int height = array.GetLength(0); 
            int width = array.GetLength(1); 
            
            Debug.Assert(columnIndexA < width);
            Debug.Assert(columnIndexB < width);

            for (int row = 0; row < height; row++)
            {
                (array[row, columnIndexA], array[row, columnIndexB]) =
                    (array[row, columnIndexB], array[row, columnIndexA]); 
            }

            return array; 
        }

        public static T[] SwitchColumn<T>(this T[] array, int columnIndexA, int columnIndexB, int width, int height)
        {
            Debug.Assert(width * height == array.Length);
            Debug.Assert(columnIndexA * height < array.Length);
            Debug.Assert(columnIndexB * height < array.Length);

            for (int row = 0; row < height; row++)
            {
                (array[row*width + columnIndexA], array[row*width + columnIndexB]) =
                    (array[row*width + columnIndexB], array[row*width + columnIndexA]); 
            }

            return array; 
        }

        public static T[] GetRow<T>(this T[] array, int rowIndex, int width)
        {
            if ((rowIndex + 1) * width > array.Length)
            {
                Debug.LogError($"SwitchRows error: (MaxRowIndex+1) * width = {(rowIndex+1) * width} > arrayLength = {array.Length}"); 
                return default; 
            }

            T[] row = new T[width]; 
            Array.Copy(array, rowIndex * width, row, 0, width);
            return row; 
        }

        public static T[] GetRow<T>(this T[,] array, int rowIndex) //TODO: test
        {
            if (array.GetLength(0) > array.Length)
            {
                Debug.LogError($"SwitchRows error: (MaxRowIndex+1) * width = {array.GetLength(0)} > arrayLength = {array.Length}"); 
                return default; 
            }

            T[] row = new T[array.GetLength(1)]; 
            Array.Copy(array, rowIndex * array.GetLength(1), row, 0, array.GetLength(1));
            return row;
        }

        public static void SetRow<T>(this T[] array, T[] value, int rowIndex, int width)
        {
            if ((rowIndex + 1) * width > array.Length)
            {
                Debug.LogError($"SwitchRows error: (MaxRowIndex+1) * width = {(rowIndex+1) * width} > arrayLength = {array.Length}"); 
            }

            Array.Copy(value, 0, array, rowIndex * width, width);
        }

        public static void SetRow<T>(this T[,] array, T[] value, int rowIndex)
        {
            if (array.GetLength(0) > array.Length)
            {
                Debug.LogError($"SwitchRows error: (MaxRowIndex+1) * width = {array.GetLength(0)} > arrayLength = {array.Length}"); 
            }

            Array.Copy(value, 0, array, rowIndex * array.GetLength(1), array.GetLength(1));
        }

        #endregion
        #endregion Matrix

        public static float SubtractLengthPythagoreon(float originalLength, float subtractValue)
        {
            return Mathf.Sqrt(originalLength * originalLength - subtractValue * subtractValue);
        }

        public static float AddLengthsPythagoreon(float originalLength, float addValue)
        {
            return Mathf.Sqrt(originalLength * originalLength + addValue * addValue);
        }
    }
}