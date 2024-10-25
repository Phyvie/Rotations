using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class Matrix
    {
        [SerializeField] private float[] InternalMatrix = Array.Empty<float>();

        [SerializeField] private int height; 
        [SerializeField] private int width;
        
        public float this[int row, int column]
        {
            get => InternalMatrix[row*width+column];
            set
            {
                InternalMatrix[row*width+column] = value;
            }
        }
        
        //Warning this function is not tested, it might do something wrong
        public Matrix(float[,] inMatrix)
        {
            Debug.Assert(inMatrix is not null);
            InternalMatrix = new float[inMatrix.GetLength(0) * inMatrix.GetLength(1)];
            height = inMatrix.GetLength(0);
            width = inMatrix.GetLength(1); 
            Buffer.BlockCopy(inMatrix, 0, InternalMatrix, 0, inMatrix.GetLength(0) * inMatrix.GetLength(1));
        }

        public Matrix(int width, int height)
        {
            this.width = width;
            this.height = height;
            InternalMatrix = new float[width * height]; 
        }
        
        public static Matrix Identity(int size)
        {
            Matrix identity = new Matrix(size , size);
            for (int i = 0; i < size; i++)
            {
                identity[i, i] = 1.0f;
            }
            return identity;
        }

        public static implicit operator Matrix(float[,] inMatrix)
        {
            return new Matrix(inMatrix); 
        }
        
        public Matrix(Matrix inMatrix)
        {
            Debug.Assert(inMatrix is not null);
            InternalMatrix = new float[inMatrix.height * inMatrix.width];
            height = inMatrix.height;
            width = inMatrix.width; 
            Array.Copy(inMatrix.InternalMatrix, InternalMatrix, 9);
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
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    if (Math.Abs(InternalMatrix[row*width+column] - other.InternalMatrix[row*width+column]) > tolerance)
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
            for (int rowIndex = 0; rowIndex < InternalMatrix.GetLength(0); rowIndex++)
            {
                string rowString = "";
                for (int columnIndex = 0; columnIndex < InternalMatrix.GetLength(1); columnIndex++)
                {
                    rowString += InternalMatrix[rowIndex*width+columnIndex].ToString("F2") + " ";
                }
                Debug.Log(rowString);
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
                    transposedMatrix[column, row] = InternalMatrix[row*width+column];
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
                trace += InternalMatrix[i * width + i];
            }
            return trace;
        }

        public float Determinant()
        {
            Debug.Assert(height == width, "Matrix must be square to calculate the determinant.");

            if (height == 1)
                return InternalMatrix[0];

            if (height == 2)
                return InternalMatrix[0] * InternalMatrix[3] - InternalMatrix[1] * InternalMatrix[2];

            float determinant = 0;
            for (int p = 0; p < width; p++)
            {
                Matrix subMatrix = CreateSubMatrix(this, 0, p);
                determinant += InternalMatrix[0*width+p] * subMatrix.Determinant() * (p % 2 == 0 ? 1 : -1);
            }
            return determinant;
        }
        
        private Matrix CreateSubMatrix(Matrix matrix, int excludingRow, int excludingCol)
        {
            float[,] result = new float[matrix.height - 1, matrix.width - 1];
            int r = -1;
            for (int rowIndex = 0; rowIndex < matrix.height; rowIndex++)
            {
                if (rowIndex == excludingRow)
                    continue;
                r++;
                int c = -1;
                for (int columnIndex = 0; columnIndex < matrix.width; columnIndex++)
                {
                    if (columnIndex == excludingCol)
                        continue;
                    result[r, ++c] = matrix.InternalMatrix[rowIndex * width *  columnIndex];
                }
            }
            return new Matrix(result);
        }

        public bool IsSpecialOrthogonal()
        {
            return Math.Abs(Determinant() - 1) < 0.001 && Inverse() == Transpose(); 
        }
    }
}