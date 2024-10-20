using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class MatrixRotation : RotationType
    {
        [SerializeField] private float [][] matrix;
        [SerializeField] private bool isRotationMatrix = true;

        public MatrixRotation(MatrixRotation copiedMatrix)
        {
            matrix = new float[3][];
            matrix[0] = new float[3]; 
            matrix[1] = new float[3]; 
            matrix[2] = new float[3];
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    matrix[row][column] = copiedMatrix[row, column]; 
                }
            }
        }
        
        public MatrixRotation(float[][] inMatrix)
        {
            Debug.Assert(inMatrix.Length == 3); 
            for (int i = 0; i < 3; i++)
            {
                Debug.Assert(inMatrix[i].Length == 3);
            }

            matrix = inMatrix;
            isRotationMatrix = IsSpecialOrthogonal(); 
        }

        public MatrixRotation(Vector3 axis, float angle, AngleType angleType)
        {
            if (angleType != AngleType.Radian)
            {
                angle = AngleType.ConvertAngle(angle, angleType, AngleType.Radian); 
            }

            axis = axis.normalized; 
        }

        public float this[int row, int column]
        {
            get => matrix[row][column]; 
            set => matrix[row][column] = value; 
        }

        public static readonly MatrixRotation Identity = new MatrixRotation(
            new float[][]
            {
                new float[]{1, 0, 0}, 
                new float[]{0, 1, 0}, 
                new float[]{0, 0, 1}
            }
        );
        
        public static MatrixRotation operator*(MatrixRotation first, MatrixRotation second)
        {
            MatrixRotation result = new MatrixRotation(Identity.matrix);
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    float value = 0; 
                    for (int i = 0; i < 3; i++)
                    {
                        value += first[row, i] + second[i, column]; 
                    }
                    result[row, column] = value; 
                }
            }
            return result; 
        }
        
        public static bool operator==(MatrixRotation firstRotation, MatrixRotation secondRotation)
        {
            if (firstRotation is null && secondRotation is null)
            {
                return true; 
            }
            
            return firstRotation is not null && firstRotation.Equals(secondRotation); 
        }

        public static bool operator !=(MatrixRotation firstRotation, MatrixRotation secondRotation)
        {
            if (firstRotation is null != secondRotation is null)
            {
                return true; 
            } 
            else if (firstRotation is null)
            {
                return true; 
            }
            else
            {
                return firstRotation.Equals(secondRotation); 
            }
        }

        public static readonly float EqualsTolerance = 0.0001f; 
        public bool Equals(MatrixRotation other)
        {
            if (other is null)
            {
                return false; 
            }
            if (this == other)
            {
                return true; 
            }
            if (matrix == other.matrix)
            {
                return true; 
            }
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (this[i, j] - other[i, j] > EqualsTolerance)
                    {
                        return false; 
                    }
                }
            }

            return true; 
        }
        
        public MatrixRotation Inverse()
        {
            float[][] GetMatrixInverse(float[][] matrix)
            {
                float det = Determinant();

                if (det == 0)
                {
                    throw new InvalidOperationException("Matrix is not invertible");
                }

                float invDet = 1.0f / det;

                float[][] inverse = new float[3][];
                inverse[0] = new float[3];
                inverse[1] = new float[3];
                inverse[2] = new float[3];

                inverse[0][0] = invDet * (matrix[1][1] * matrix[2][2] - matrix[1][2] * matrix[2][1]);
                inverse[0][1] = invDet * (matrix[0][2] * matrix[2][1] - matrix[0][1] * matrix[2][2]);
                inverse[0][2] = invDet * (matrix[0][1] * matrix[1][2] - matrix[0][2] * matrix[1][1]);

                inverse[1][0] = invDet * (matrix[1][2] * matrix[2][0] - matrix[1][0] * matrix[2][2]);
                inverse[1][1] = invDet * (matrix[0][0] * matrix[2][2] - matrix[0][2] * matrix[2][0]);
                inverse[1][2] = invDet * (matrix[0][2] * matrix[1][0] - matrix[0][0] * matrix[1][2]);

                inverse[2][0] = invDet * (matrix[1][0] * matrix[2][1] - matrix[1][1] * matrix[2][0]);
                inverse[2][1] = invDet * (matrix[0][1] * matrix[2][0] - matrix[0][0] * matrix[2][1]);
                inverse[2][2] = invDet * (matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0]);

                return inverse;
            }

            return new MatrixRotation(GetMatrixInverse(matrix)); 
        }
        
        private MatrixRotation Transpose()
        {
            float[][] transposedMatrix = new float[3][];
            transposedMatrix[0] = new float[3]; 
            transposedMatrix[1] = new float[3]; 
            transposedMatrix[2] = new float[3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    transposedMatrix[i][j] = matrix[j][i]; 
                }
            }
            
            return new MatrixRotation(transposedMatrix); 
        }
        
        private bool IsSpecialOrthogonal()
        {
            return IsOrthogonal() && (Math.Abs(Determinant() - 1.0f) < 0.001f); 
        }

        private bool IsOrthogonal()
        {
            return Transpose() == Inverse(); 
        }

        private float Determinant()
        {
            return 0
                   + matrix[0][0] * matrix[1][1] * matrix[2][2]
                   + matrix[0][1] * matrix[1][2] * matrix[2][0]
                   + matrix[0][2] * matrix[1][0] * matrix[2][1]
                   - matrix[0][2] * matrix[1][1] * matrix[2][0]
                   - matrix[0][0] * matrix[1][2] * matrix[2][1] 
                   - matrix[0][1] * matrix[1][0] * matrix[2][2]
                ; 
        }

        private float Trace()
        {
            return this[0, 0] + this[1, 1] + this[2, 2]; 
        }
        
        public override EulerAngleRotation ToEulerAngleRotation()
        {
            throw new System.NotImplementedException();
        }

        public override QuaternionRotation ToQuaternionRotation()
        {
            if (!isRotationMatrix)
            {
                isRotationMatrix = IsSpecialOrthogonal(); 
            }

            if (!isRotationMatrix)
            {
                throw new Exception("Can't transform matrix to Quaternion, because it isn't a rotationMatrix"); 
            }

            float trace = Trace();
            float w, x, y, z;

            if (trace > 0)
            {
                float s = 0.5f / Mathf.Sqrt(trace + 1.0f);
                w = 0.25f / s;
                x = (matrix[2][1] - matrix[1][2]) * s;
                y = (matrix[0][2] - matrix[2][0]) * s;
                z = (matrix[1][0] - matrix[0][1]) * s;
            }
            else
            {
                if (matrix[0][0] > matrix[1][1] && matrix[0][0] > matrix[2][2])
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + matrix[0][0] - matrix[1][1] - matrix[2][2]);
                    w = (matrix[2][1] - matrix[1][2]) / s;
                    x = 0.25f * s;
                    y = (matrix[0][1] + matrix[1][0]) / s;
                    z = (matrix[0][2] + matrix[2][0]) / s;
                }
                else if (matrix[1][1] > matrix[2][2])
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + matrix[1][1] - matrix[0][0] - matrix[2][2]);
                    w = (matrix[0][2] - matrix[2][0]) / s;
                    x = (matrix[0][1] + matrix[1][0]) / s;
                    y = 0.25f * s;
                    z = (matrix[1][2] + matrix[2][1]) / s;
                }
                else
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + matrix[2][2] - matrix[0][0] - matrix[1][1]);
                    w = (matrix[1][0] - matrix[0][1]) / s;
                    x = (matrix[0][2] + matrix[2][0]) / s;
                    y = (matrix[1][2] + matrix[2][1]) / s;
                    z = 0.25f * s;
                }
            }

            return new QuaternionRotation(w, x, y, z);
        }

        public override MatrixRotation ToMatrixRotation()
        {
            return new MatrixRotation(matrix); 
        }

        public override AxisAngleRotation ToAxisAngleRotation()
        {
            return ToQuaternionRotation().ToAxisAngleRotation(); 
        }

        public override void SetAngleType(AngleType value)
        {
            _angleType = value; 
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            throw new NotImplementedException();
        }
    }
}
