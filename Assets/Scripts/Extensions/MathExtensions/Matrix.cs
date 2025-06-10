using System;
using System.Linq;
using UnityEngine;

namespace MathExtensions
{
    [Serializable]
    public class Matrix
    {
        #region Variables
        [SerializeField] private float[] InternalMatrix = Array.Empty<float>();

        [SerializeField] private int height; 
        [SerializeField] private int width;
        
        public float this[int row, int column]
        {
            get
            {
                Debug.Assert(row < height, $"row({row}) > height{(height)}");
                Debug.Assert(column < width, $"column({column}) > width{(width)}");
                return InternalMatrix[row * Width + column];
            }
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
        #endregion Variables
        
        #region Constructors
        public Matrix(float[] inMatrix, int width, int height, bool isArrayRef = false)
        {
            Debug.Assert(inMatrix is not null);
            Debug.Assert(inMatrix.Length == width * height);
            Height = height;
            Width = width;

            if (isArrayRef)
            {
                InternalMatrix = inMatrix; 
            }
            else
            {
                InternalMatrix = new float[inMatrix.GetLength(0) * inMatrix.GetLength(1)];
                Array.Copy(inMatrix, 0, InternalMatrix, 0, width * height);     
            }
        }
        
        //Warning this is not tested, Buffer.BlockCopy might not do what it's supposed to
        public Matrix(float[,] inMatrix)
        {
            Debug.Assert(inMatrix is not null);
            Height = inMatrix.GetLength(0);
            Width = inMatrix.GetLength(1);

            InternalMatrix = new float[inMatrix.GetLength(0) * inMatrix.GetLength(1)];
            Buffer.BlockCopy(inMatrix, 0, InternalMatrix, 0, sizeof(float) * inMatrix.GetLength(0) * inMatrix.GetLength(1)); //TODO: this BlockCopy does not do what it's supposed to do
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

        public static Matrix Zero(int size)
        {
            return new Matrix(size, size); 
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
        #endregion Constructors
        
        #region Comparison
        public static bool operator ==(Matrix firstMatrix, Matrix secondMatrix)
        {
            return firstMatrix != null && firstMatrix.Equals(secondMatrix);
        }

        public static bool operator !=(Matrix firstMatrix, Matrix secondMatrix)
        {
            return firstMatrix is not null && !firstMatrix.Equals(secondMatrix);
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
                    if (System.Math.Abs(InternalMatrix[row*Width+column] - other.InternalMatrix[row*Width+column]) > tolerance)
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return InternalMatrix.GetHashCode();
        }
        #endregion Comparison
        
        #region MatrixArithmetic
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
            Debug.Assert(firstMatrix.Width == secondMatrix.Height, "firstMatrix.width != secondMatrix.height");

            return new Matrix(Math.MatrixMultiply(
                firstMatrix.InternalMatrix, firstMatrix.height, firstMatrix.width, 
                secondMatrix.InternalMatrix, secondMatrix.width, secondMatrix.height), 
                firstMatrix.height, secondMatrix.width, true); 
        }
        #endregion MatrixArithmetic

        #region MatrixVectorArithmetic
        public static Vector2 operator *(Matrix matrix, Vector2 vector)
        {
            Debug.Assert(matrix.width == 2, "Matrix width must be 2 for multiplication with Vector2.");
            return new Vector2(
                matrix[0, 0] * vector.x + matrix[0, 1] * vector.y,
                matrix[1, 0] * vector.x + matrix[1, 1] * vector.y
            );
        }

        // Operator* for Vector2Int
        public static Vector2Int operator *(Matrix matrix, Vector2Int vector)
        {
            Debug.Assert(matrix.width == 2, "Matrix width must be 2 for multiplication with Vector2Int.");
            return new Vector2Int(
                Mathf.RoundToInt(matrix[0, 0] * vector.x + matrix[0, 1] * vector.y),
                Mathf.RoundToInt(matrix[1, 0] * vector.x + matrix[1, 1] * vector.y)
            );
        }

        // Operator* for Vector3
        public static Vector3 operator *(Matrix matrix, Vector3 vector)
        {
            Debug.Assert(matrix.width == 3, "Matrix width must be 3 for multiplication with Vector3.");
            return new Vector3(
                matrix[0, 0] * vector.x + matrix[0, 1] * vector.y + matrix[0, 2] * vector.z,
                matrix[1, 0] * vector.x + matrix[1, 1] * vector.y + matrix[1, 2] * vector.z,
                matrix[2, 0] * vector.x + matrix[2, 1] * vector.y + matrix[2, 2] * vector.z
            );
        }

        // Operator* for Vector3Int
        public static Vector3Int operator *(Matrix matrix, Vector3Int vector)
        {
            Debug.Assert(matrix.width == 3, "Matrix width must be 3 for multiplication with Vector3Int.");
            return new Vector3Int(
                Mathf.RoundToInt(matrix[0, 0] * vector.x + matrix[0, 1] * vector.y + matrix[0, 2] * vector.z),
                Mathf.RoundToInt(matrix[1, 0] * vector.x + matrix[1, 1] * vector.y + matrix[1, 2] * vector.z),
                Mathf.RoundToInt(matrix[2, 0] * vector.x + matrix[2, 1] * vector.y + matrix[2, 2] * vector.z)
            );
        }

        // Operator* for Vector4
        public static Vector4 operator *(Matrix matrix, Vector4 vector)
        {
            Debug.Assert(matrix.width == 4, "Matrix width must be 4 for multiplication with Vector4.");
            return new Vector4(
                matrix[0, 0] * vector.x + matrix[0, 1] * vector.y + matrix[0, 2] * vector.z + matrix[0, 3] * vector.w,
                matrix[1, 0] * vector.x + matrix[1, 1] * vector.y + matrix[1, 2] * vector.z + matrix[1, 3] * vector.w,
                matrix[2, 0] * vector.x + matrix[2, 1] * vector.y + matrix[2, 2] * vector.z + matrix[2, 3] * vector.w,
                matrix[3, 0] * vector.x + matrix[3, 1] * vector.y + matrix[3, 2] * vector.z + matrix[3, 3] * vector.w
            );
        }
        #endregion MatrixVectorArithmetic
        
        #region RowColumnOperators
        public void SwitchRows(int rowIndexA, int rowIndexB)
        {
            InternalMatrix.SwitchRows(rowIndexA, rowIndexB, width); 
        }

        public void AddRowOntoRow(int from, int to, float multiplier)
        {
            if (Mathf.Max(from, to) > height)
            {
                throw new IndexOutOfRangeException($"{Mathf.Max(from, to)} > {height}"); 
            }
            
            for (int column = 0; column < width; column++)
            {
                this[to, column] += this[from, column] * multiplier; 
            }
        }

        public void MultiplyRow(int row, float multiplier)
        {
            if (row > height)
            {
                throw new IndexOutOfRangeException($"{row} > {height}"); 
            }
            
            for (int column = 0; column < width; column++)
            {
                this[row, column] *= multiplier; 
            }
        }
        #endregion RowColumnArithmetic
        
        #region MatrixSpecificOperations
        public Matrix Inverse() 
        {
            Debug.Assert(Height == Width, "Matrix must be square to find its inverse.");
            
            PLUDecomposition(out Matrix permutationMatrix, out Matrix lowerLeftMatrix, out Matrix upperRightMatrix);

            Matrix upperRightInverse = upperRightMatrix.InverseUpperTriangularMatrix();
            Matrix lowerLeftInverse = lowerLeftMatrix.InverseLowerTriangularMatrix();
            Matrix permutationInverse = permutationMatrix.InversePermutationMatrix();
            Matrix inverseMatrix = upperRightInverse * lowerLeftInverse * permutationInverse; 
            
            Debug.Assert(this == permutationMatrix * lowerLeftMatrix * upperRightMatrix, "PLU not correct");
            Debug.Assert(inverseMatrix * this == Identity(height), "Inverse not correct");
            return inverseMatrix;
        }

        public void PLUDecomposition(out Matrix outP, out Matrix outL, out Matrix outU) //TODO: numerically stable pivot choosing
        {
            Matrix lowerLeft = Identity(width);
            Matrix upperRight = new Matrix(this);
            int[] permutation = Enumerable.Range(0, width).ToArray(); 
            Matrix permutationMatrix = Zero(width);

            {
                int column = 0;
                int row = 0;
                
                while (row < height && column < width)
                {
                    int pivotRow = row;
                    float maxValue = 0;
                    for (int i = row; i < height; i++)
                    {
                        if (maxValue < Mathf.Abs(upperRight[i, column]))
                        {
                            pivotRow = i; 
                            maxValue = Mathf.Abs(upperRight[i, column]); 
                        }    
                    }
                    if (maxValue == 0)
                    {
                        column++; 
                        continue; 
                    }
                    PLUSwitchRows(row, pivotRow);
                
                    for (int rowBelow = row + 1; rowBelow < height; rowBelow++)
                    {
                        PLUAddRowOntoRow(row, rowBelow, -(upperRight[rowBelow, column]/upperRight[row, column]));
                    }
                    
                    row++; 
                    column++; 
                }
            }

            for (int row = 0; row < permutation.Length; row++)
            {
                permutationMatrix[row, permutation[row]] = 1; 
            }
            
            outP = permutationMatrix.InversePermutationMatrix();
            outL = lowerLeft;
            outU = upperRight;

            /*
            if (outP * outL * outU != this)
            {
                Debug.LogError("PLU Decomposition failed");
            }
            else
            {
                Debug.Log("PLU Decomposition succeeded");
            }
            */
            

            void PLUSwitchRows(int rowIndexA, int rowIndexB)
            {
                upperRight.SwitchRows(rowIndexA, rowIndexB);
                for (int i = 0; i < Mathf.Min(rowIndexA, rowIndexB); i++)
                {
                    (lowerLeft[rowIndexA, i], lowerLeft[rowIndexB, i]) =
                        (lowerLeft[rowIndexB, i], lowerLeft[rowIndexA, i]); 
                }
                (permutation[rowIndexA], permutation[rowIndexB]) = (permutation[rowIndexB], permutation[rowIndexA]); 
            }
            
            void PLUAddRowOntoRow(int rowIndexA, int rowIndexB, float multiplier)
            {
                Debug.Assert(lowerLeft[rowIndexB, rowIndexA] == 0);
                
                upperRight.AddRowOntoRow(rowIndexA, rowIndexB, multiplier);
                lowerLeft[rowIndexB, rowIndexA] = -multiplier; 
            }
        }

        private Matrix InversePermutationMatrix()
        {
            return Transpose(); 
        }

        private Matrix InverseUpperTriangularMatrix()
        {
            Matrix resultMatrix = Identity(height);

            for (int i = height - 1; i >= 0; i--)
            {
                resultMatrix[i, i] = 1 / this[i, i];
                for (int j = i - 1; j >= 0; j--)
                {
                    float sum = 0;
                    for (int k = j + 1; k <= i; k++)
                    {
                        sum += this[j, k] * resultMatrix[k, i];
                    }

                    resultMatrix[j, i] = -sum / this[j, j];
                }
            }

            return resultMatrix; 
        }

        private Matrix InverseLowerTriangularMatrix()
        {
            Matrix resultMatrix = Identity(height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    float sum = (i == j) ? 1 : 0;
                    for (int k = i - 1; k >= j; k--)
                    {
                        sum -= this[i, k] * resultMatrix[k, j];
                    }

                    resultMatrix[i, j] = sum / this[i, i];
                }
            } 
            
            return resultMatrix; 
        }
        
        public Matrix Transpose()
        {
            Matrix transposedMatrix = new Matrix(new float[Width, Height]);
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    transposedMatrix[column, row] = this[row, column];
                }
            }
            return transposedMatrix;
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
                    result[r, ++c] = matrix.InternalMatrix[rowIndex * Width + columnIndex];
                }
            }
            return new Matrix(result);
        }
        #endregion MatrixSpecificOperations
        
        #region MatrixProperties
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

            if (Height == 0)
            {
                return Single.NaN;
            }

            if (Height == 1)
            {
                return InternalMatrix[0];
            }

            if (Height == 2)
            {
                return InternalMatrix[0] * InternalMatrix[3] - InternalMatrix[1] * InternalMatrix[2];
            }

            if (Height == 3)
            {
                return 
                    this[0, 0] * (this[1, 1] * this[2, 2] - this[1, 2] * this[2, 1]) + 
                    this[0, 1] * (this[1, 0] * this[2, 2] - this[1, 2] * this[2, 0]) +
                    this[0, 2] * (this[1, 0] * this[2, 1] - this[2, 0] * this[1, 1]); 
            }

            if (Height == 4)
            {
                return 0
                       + this[0, 0] * CreateSubMatrix(this, 0, 0).Determinant()
                       - this[0, 1] * CreateSubMatrix(this, 0, 1).Determinant()
                       + this[0, 2] * CreateSubMatrix(this, 0, 2).Determinant()
                       - this[0, 3] * CreateSubMatrix(this, 0, 3).Determinant()
                    ; 
            }
            
            PLUDecomposition(out Matrix permutation, out Matrix lowerLeft, out Matrix upperRight);
            
            return lowerLeft.DiagonalProduct() * upperRight.DiagonalProduct();
        }

        public int PermutationMatrixDeterminant()
        {
            int[] permutation = new int[height];
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (Mathf.Approximately(this[row, col], 1.0f)) 
                    {
                        permutation[row] = col;
                        break;
                    }
                    if (!Mathf.Approximately(this[row, col], 0.0f))
                    {
                        Debug.LogError($"{nameof(PermutationMatrixDeterminant)} error: should only be used for Permutation matrices");
                        return 0; 
                    }
                }
            }

            int swaps = PermutationMatrixCountSwaps(permutation);
            return (swaps % 2 == 0) ? 1 : -1;
        }
        
        private int PermutationMatrixCountSwaps(int[] perm)
        {
            bool[] visited = new bool[perm.Length];
            int swapCount = 0;

            for (int i = 0; i < perm.Length; i++)
            {
                if (!visited[i])
                {
                    int cycleLength = 0;
                    int j = i;

                    while (!visited[j])
                    {
                        visited[j] = true;
                        j = perm[j]; 
                        cycleLength++;
                    }

                    swapCount += (cycleLength - 1);
                }
            }

            return swapCount;
        }
        
        public bool IsDiagonal()
        {
            if (Width != Height)
            {
                return false; 
            }

            if (Width == 0)
            {
                return true; 
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (i == j)
                    {
                        continue; 
                    }
                    else
                    {
                        if (this[i, j] != 0)
                        {
                            return false;
                        }
                    }
                }
            }
            
            return true; 
        }
        
        public bool IsSpecialOrthogonal()
        {
            //This function is manually calculating the inverse instead of using .Inverse(), because that way the determinant can be calculated via the results from the PLU decomposition
            if (Width != Height)
            {
                return false;
            }
            
            if (Width == 0)
            {
                return true; 
            }
            
            PLUDecomposition(out Matrix permutationMatrix, out Matrix lowerLeft, out Matrix upperRight);
            
            float determinant = permutationMatrix.PermutationMatrixDeterminant() * lowerLeft.DiagonalProduct() * upperRight.DiagonalProduct();

            if (System.Math.Abs(determinant - 1) > 0.001)
            {
                return false; 
            }
            
            Matrix inverseMatrix = upperRight.InverseUpperTriangularMatrix() *
                                   lowerLeft.InverseLowerTriangularMatrix() *
                                   permutationMatrix.InversePermutationMatrix();

            Matrix transposeMatrix = Transpose(); 
            
            if (inverseMatrix == transposeMatrix)
            {
                return true; 
            }
            else
            {
                return false; 
            }
        }

        public bool IsOrthogonalUpToScale()
        {
            if (Width != Height)
            {
                return false;
            }
            
            if (Width == 0)
            {
                return true; 
            }
            
            PLUDecomposition(out Matrix permutationMatrix, out Matrix lowerLeft, out Matrix upperRight);
            
            float determinant = lowerLeft.DiagonalProduct() * upperRight.DiagonalProduct();

            if (System.Math.Abs(determinant - 1) > 0.001)
            {
                return false; 
            }
            
            Matrix inverseMatrix = upperRight.InverseUpperTriangularMatrix() *
                                   lowerLeft.InverseLowerTriangularMatrix() *
                                   permutationMatrix.InversePermutationMatrix();

            Matrix GramMatrix = inverseMatrix * Transpose();
            if (GramMatrix.IsDiagonal())
            {
                return true;                
            }
            else
            {
                return false;
            }
        }
        
        public bool IsOrthonormal()
        {
            if (Width != Height)
            {
                return false; 
            }
            if (Width == 0)
            {
                return true; 
            }
            return Inverse() == Transpose(); 
        }

        public float DiagonalProduct()
        {
            Debug.Assert(Height == Width);
            float result = 1;
            for (int diagonalIndex = 0; diagonalIndex < Height; diagonalIndex++)
            {
                result *= this[diagonalIndex, diagonalIndex]; 
            }
            return result; 
        }
        #endregion MatrixProperties
        
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
    }
}