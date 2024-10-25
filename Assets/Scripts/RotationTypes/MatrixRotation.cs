using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class MatrixRotation : RotationType
    {
        [SerializeField] private bool isRotationMatrix = true;
        // [SerializeField] private bool bExpandTo4x4; //TODO
        [SerializeField] private Matrix matrix;

        public MatrixRotation()
        {
            angleType = AngleType.Radian;
            matrix = new Matrix(Matrix.Identity(3)); 
        }
        
        public MatrixRotation(MatrixRotation copiedMatrix)
        {
            matrix = new Matrix(copiedMatrix.matrix);
            isRotationMatrix = matrix.IsSpecialOrthogonal(); 
        }

        public MatrixRotation(Matrix matrix)
        {
            matrix = new Matrix(matrix);
            isRotationMatrix = matrix.IsSpecialOrthogonal(); 
        }
        
        public float this[int row, int column]
        {
            get => matrix[row, column];
            set
            {
                matrix[row, column] = value;
                isRotationMatrix = matrix.IsSpecialOrthogonal(); 
            }
        }

        public static MatrixRotation Identity(int size)
        {
            return new MatrixRotation(Matrix.Identity(size));
        }

        public MatrixRotation Inverse()
        {
            return new MatrixRotation(matrix.Inverse()); 
        }
        
        public static MatrixRotation operator*(MatrixRotation first, MatrixRotation second)
        {
            return new MatrixRotation(first.matrix * second.matrix); 
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
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            
            MatrixRotation otherRotation = (MatrixRotation)obj; 
            if (matrix == otherRotation.matrix)
            {
                return true; 
            }

            return false; 
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(matrix, isRotationMatrix);
        }
        
        public override EulerAngleRotation ToEulerAngleRotation()
        {
            throw new System.NotImplementedException();
        }

        public override QuaternionRotation ToQuaternionRotation()
        {
            if (!isRotationMatrix)
            {
                isRotationMatrix = matrix.IsSpecialOrthogonal(); 
            }

            if (!isRotationMatrix)
            {
                throw new Exception("Can't transform matrix to Quaternion, because it isn't a rotationMatrix"); 
            }

            float trace = matrix.Trace();
            float w, x, y, z;

            if (trace > 0)
            {
                float s = 0.5f / Mathf.Sqrt(trace + 1.0f);
                w = 0.25f / s;
                x = (matrix[2, 1] - matrix[1, 2]) * s;
                y = (matrix[0, 2] - matrix[2, 0]) * s;
                z = (matrix[1, 0] - matrix[0, 1]) * s;
            }
            else
            {
                if (matrix[0, 0] > matrix[1, 1] && matrix[0, 0] > matrix[2, 2])
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + matrix[0, 0] - matrix[1, 1] - matrix[2, 2]);
                    w = (matrix[2, 1] - matrix[1, 2]) / s;
                    x = 0.25f * s;
                    y = (matrix[0, 1] + matrix[1, 0]) / s;
                    z = (matrix[0, 2] + matrix[2, 0]) / s;
                }
                else if (matrix[1, 1] > matrix[2, 2])
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + matrix[1, 1] - matrix[0, 0] - matrix[2, 2]);
                    w = (matrix[0, 2] - matrix[2, 0]) / s;
                    x = (matrix[0, 1] + matrix[1, 0]) / s;
                    y = 0.25f * s;
                    z = (matrix[1, 2] + matrix[2, 1]) / s;
                }
                else
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + matrix[2, 2] - matrix[0, 0] - matrix[1, 1]);
                    w = (matrix[1, 0] - matrix[0, 1]) / s;
                    x = (matrix[0, 2] + matrix[2, 0]) / s;
                    y = (matrix[1, 2] + matrix[2, 1]) / s;
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
