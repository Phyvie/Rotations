using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class Matrix
    {
        [SerializeField] private float[,] _matrix;
        [SerializeField] private float[] _propertyDrawerMatrix = Array.Empty<float>();

        [SerializeField] private int height; 
        [SerializeField] private int width;

        public float[,] InternalMatrix
        {
            get => _matrix;
            set
            {
                _matrix = value;
                height = _matrix.GetLength(0); 
                width = _matrix.GetLength(1); 
                _propertyDrawerMatrix = new float[width * height]; 
                for (int row = 0; row < height; row++)
                {
                    for (int column = 0; column < width; column++)
                    {
                        _propertyDrawerMatrix[row*width+column] = _matrix[row, column]; 
                    }
                }
            }
        }

        public float this[int row, int column]
        {
            get => InternalMatrix[row,column];
            set
            {
                InternalMatrix[row, column] = value;
                _propertyDrawerMatrix[row*height + column] = value; 
            }
        }
        
        public Matrix(float[,] inMatrix)
        {
            Debug.Assert(inMatrix is not null);
            InternalMatrix = new float[inMatrix.GetLength(0),inMatrix.GetLength(1)];
            height = inMatrix.GetLength(0);
            width = inMatrix.GetLength(1); 
            Buffer.BlockCopy(inMatrix, 0, InternalMatrix, 0, inMatrix.GetLength(0) * inMatrix.GetLength(1));
        }

        public static implicit operator Matrix(float[,] inMatrix)
        {
            return new Matrix(inMatrix); 
        }
        
        public Matrix(Matrix inMatrix)
        {
            Debug.Assert(inMatrix is not null);
            InternalMatrix = new float[inMatrix.height, inMatrix.width];
            height = inMatrix.height;
            width = inMatrix.width; 
            Buffer.BlockCopy(inMatrix.InternalMatrix, 0, InternalMatrix, 0, width * height);
        }
        
        public static bool operator ==(Matrix firstMatrix, Matrix secondMatrix)
        {
            return firstMatrix != null && firstMatrix.Equals(secondMatrix);
        }

        public static bool operator !=(Matrix firstMatrix, Matrix secondMatrix)
        {
            return firstMatrix != null && !firstMatrix.Equals(secondMatrix);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Matrix other = (Matrix)obj;
            if (height != other.height || width != other.width)
                return false;

            float tolerance = 0.0001f;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (Math.Abs(InternalMatrix[i, j] - other.InternalMatrix[i, j]) > tolerance)
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return InternalMatrix.GetHashCode();
        }
        
        public void PrintMatrix()
        {
            for (int i = 0; i < InternalMatrix.GetLength(0); i++)
            {
                string row = "";
                for (int j = 0; j < InternalMatrix.GetLength(1); j++)
                {
                    row += InternalMatrix[i, j].ToString("F2") + " ";
                }
                Debug.Log(row);
            }
        }
        
        public static Matrix operator +(Matrix firstMatrix, Matrix secondMatrix)
        {
            Debug.Assert(firstMatrix.height == secondMatrix.height, "Matrices must have the same height.");
            Debug.Assert(firstMatrix.width == secondMatrix.width, "Matrices must have the same width.");

            float[,] _newMatrix = new float[firstMatrix.height, firstMatrix.width];
            for (int row = 0; row < firstMatrix.height; row++)
            {
                for (int column = 0; column < firstMatrix.width; column++)
                {
                    _newMatrix[row, column] = firstMatrix[row, column] + secondMatrix[row, column];
                }
            }
            Matrix addedMatrix = new Matrix(_newMatrix);
            return addedMatrix;
        }
        
        public static Matrix operator*(Matrix firstMatrix, Matrix secondMatrix)
        {
            Debug.Assert(firstMatrix.width != secondMatrix.height, "firstMatrix.width != secondMatrix.height");
            Debug.Assert(firstMatrix.height != secondMatrix.width, "firstMatrix.height != secondMatrix.width");

            float[,] _newMatrix = new float[firstMatrix.height,secondMatrix.width];
            for (int row = 0; row < firstMatrix.height; row++)
            {
                for (int column = 0; column < secondMatrix.width; column++)
                {
                    for (int k = 0; k < firstMatrix.height; k++)
                    {
                        _newMatrix[row, column] += firstMatrix[k, column] * secondMatrix[row, k];
                    }
                }
            }
            
            Matrix multipliedMatrix = new Matrix(_newMatrix); 
            return multipliedMatrix; 
        }
        
        public Matrix Inverse()
        {
            Debug.Assert(height == width, "Matrix must be square to find its inverse.");

            int n = height;
            Matrix result = new Matrix(new float[n, n]);
            Matrix copy = new Matrix(this);

            for (int i = 0; i < n; i++)
                result[i, i] = 1;

            for (int i = 0; i < n; i++)
            {
                float diagVal = copy[i, i];
                for (int j = 0; j < n; j++)
                {
                    copy[i, j] /= diagVal;
                    result[i, j] /= diagVal;
                }

                for (int row = 0; row < n; row++)
                {
                    if (row != i)
                    {
                        float rowFactor = copy[row, i];
                        for (int col = 0; col < n; col++)
                        {
                            copy[row, col] -= rowFactor * copy[i, col];
                            result[row, col] -= rowFactor * result[i, col];
                        }
                    }
                }
            }

            return result;
        }
        
        public Matrix Transpose()
        {
            Matrix transposedMatrix = new Matrix(new float[width, height]);
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    transposedMatrix[column, row] = InternalMatrix[row, column];
                }
            }
            return transposedMatrix;
        }
        
        public float Trace()
        {
            Debug.Assert(height == width, "Matrix must be square to calculate the trace.");
            float trace = 0;
            for (int i = 0; i < height; i++)
            {
                trace += InternalMatrix[i, i];
            }
            return trace;
        }

        public float Determinant()
        {
            Debug.Assert(height == width, "Matrix must be square to calculate the determinant.");

            if (height == 1)
                return InternalMatrix[0, 0];

            if (height == 2)
                return InternalMatrix[0, 0] * InternalMatrix[1, 1] - InternalMatrix[0, 1] * InternalMatrix[1, 0];

            float determinant = 0;
            for (int p = 0; p < width; p++)
            {
                Matrix subMatrix = CreateSubMatrix(this, 0, p);
                determinant += InternalMatrix[0, p] * subMatrix.Determinant() * (p % 2 == 0 ? 1 : -1);
            }
            return determinant;
        }
        
        private Matrix CreateSubMatrix(Matrix matrix, int excludingRow, int excludingCol)
        {
            float[,] result = new float[matrix.height - 1, matrix.width - 1];
            int r = -1;
            for (int i = 0; i < matrix.height; i++)
            {
                if (i == excludingRow)
                    continue;
                r++;
                int c = -1;
                for (int j = 0; j < matrix.width; j++)
                {
                    if (j == excludingCol)
                        continue;
                    result[r, ++c] = matrix.InternalMatrix[i, j];
                }
            }
            return new Matrix(result);
        }
        
        
    }
}