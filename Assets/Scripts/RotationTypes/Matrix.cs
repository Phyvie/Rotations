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
            get => InternalMatrix[row*Width+column];
            set
            {
                InternalMatrix[row*Width+column] = value;
            }
        }
        
        public int Height
        {
            get => height;
            private set => height = value;
        }

        public int Width
        {
            get => width;
            private set => width = value;
        }
        
        //Warning this function is not tested, it might do something wrong
        public Matrix(float[,] inMatrix)
        {
            Debug.Assert(inMatrix is not null);
            InternalMatrix = new float[inMatrix.GetLength(0) * inMatrix.GetLength(1)];
            Height = inMatrix.GetLength(0);
            Width = inMatrix.GetLength(1); 
            Buffer.BlockCopy(inMatrix, 0, InternalMatrix, 0, inMatrix.GetLength(0) * inMatrix.GetLength(1));
        }

        public Matrix(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            InternalMatrix = new float[width * height]; 
        }
        
        public Matrix(Matrix inMatrix)
        {
            Debug.Assert(inMatrix is not null);
            InternalMatrix = new float[inMatrix.Height * inMatrix.Width];
            Height = inMatrix.Height;
            Width = inMatrix.Width; 
            Array.Copy(inMatrix.InternalMatrix, InternalMatrix, 9);
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
            if (Height != other.Height || Width != other.Width)
                return false;

            float tolerance = 0.0001f;
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    if (Math.Abs(InternalMatrix[row*Width+column] - other.InternalMatrix[row*Width+column]) > tolerance)
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
                    rowString += InternalMatrix[rowIndex*Width+columnIndex].ToString("F2") + " ";
                }
                Debug.Log(rowString);
            }
        }
        
        public static Matrix operator +(Matrix firstMatrix, Matrix secondMatrix)
        {
            Debug.Assert(firstMatrix.Height == secondMatrix.Height, "Matrices must have the same height.");
            Debug.Assert(firstMatrix.Width == secondMatrix.Width, "Matrices must have the same width.");

            float[,] _newMatrix = new float[firstMatrix.Height, firstMatrix.Width];
            for (int row = 0; row < firstMatrix.Height; row++)
            {
                for (int column = 0; column < firstMatrix.Width; column++)
                {
                    _newMatrix[row, column] = firstMatrix[row, column] + secondMatrix[row, column];
                }
            }
            Matrix addedMatrix = new Matrix(_newMatrix);
            return addedMatrix;
        }
        
        public static Matrix operator*(Matrix firstMatrix, Matrix secondMatrix)
        {
            Debug.Assert(firstMatrix.Width != secondMatrix.Height, "firstMatrix.width != secondMatrix.height");
            Debug.Assert(firstMatrix.Height != secondMatrix.Width, "firstMatrix.height != secondMatrix.width");

            float[,] _newMatrix = new float[firstMatrix.Height,secondMatrix.Width];
            for (int row = 0; row < firstMatrix.Height; row++)
            {
                for (int column = 0; column < secondMatrix.Width; column++)
                {
                    for (int k = 0; k < firstMatrix.Height; k++)
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
            Debug.Assert(Height == Width, "Matrix must be square to find its inverse.");

            int n = Height;
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
            Matrix transposedMatrix = new Matrix(new float[Width, Height]);
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    transposedMatrix[column, row] = InternalMatrix[row*Width+column];
                }
            }
            return transposedMatrix;
        }
        
        public float Trace()
        {
            Debug.Assert(Height == Width, "Matrix must be square to calculate the trace.");
            float trace = 0;
            for (int i = 0; i < Height; i++)
            {
                trace += InternalMatrix[i * Width + i];
            }
            return trace;
        }

        public float Determinant()
        {
            Debug.Assert(Height == Width, "Matrix must be square to calculate the determinant.");

            if (Height == 1)
                return InternalMatrix[0];

            if (Height == 2)
                return InternalMatrix[0] * InternalMatrix[3] - InternalMatrix[1] * InternalMatrix[2];

            float determinant = 0;
            for (int p = 0; p < Width; p++)
            {
                Matrix subMatrix = CreateSubMatrix(this, 0, p);
                determinant += InternalMatrix[0*Width+p] * subMatrix.Determinant() * (p % 2 == 0 ? 1 : -1);
            }
            return determinant;
        }
        
        private Matrix CreateSubMatrix(Matrix matrix, int excludingRow, int excludingCol)
        {
            float[,] result = new float[matrix.Height - 1, matrix.Width - 1];
            int r = -1;
            for (int rowIndex = 0; rowIndex < matrix.Height; rowIndex++)
            {
                if (rowIndex == excludingRow)
                    continue;
                r++;
                int c = -1;
                for (int columnIndex = 0; columnIndex < matrix.Width; columnIndex++)
                {
                    if (columnIndex == excludingCol)
                        continue;
                    result[r, ++c] = matrix.InternalMatrix[rowIndex * Width *  columnIndex];
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