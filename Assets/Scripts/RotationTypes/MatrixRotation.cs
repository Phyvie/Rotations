using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotationTypes
{
    [Serializable]
    public class MatrixRotation : RotationType
    {
        [SerializeField] private bool isRotationMatrix = true;
        // [SerializeField] private bool bExpandTo4x4; //TODO
        [SerializeField] public Matrix InternalMatrix;

        public MatrixRotation()
        {
            angleType = AngleType.Radian;
            InternalMatrix = new Matrix(Matrix.Identity(3)); 
        }
        
        public MatrixRotation(MatrixRotation copiedMatrix)
        {
            InternalMatrix = new Matrix(copiedMatrix.InternalMatrix);
            isRotationMatrix = InternalMatrix.IsSpecialOrthogonal(); 
        }

        public MatrixRotation(Matrix matrix)
        {
            matrix = new Matrix(matrix);
            isRotationMatrix = matrix.IsSpecialOrthogonal(); 
        }
        
        public float this[int row, int column]
        {
            get => InternalMatrix[row, column];
            set
            {
                InternalMatrix[row, column] = value;
                isRotationMatrix = InternalMatrix.IsSpecialOrthogonal(); 
            }
        }

        public static MatrixRotation RotationIdentity()
        {
            return new MatrixRotation(Matrix.Identity(3));
        }

        public static MatrixRotation TransformIdentity()
        {
            return new MatrixRotation(Matrix.Identity(4)); 
        }

        public MatrixRotation Inverse()
        {
            return new MatrixRotation(InternalMatrix.Inverse()); 
        }
        
        public static MatrixRotation operator*(MatrixRotation first, MatrixRotation second)
        {
            return new MatrixRotation(first.InternalMatrix * second.InternalMatrix); 
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
            if (InternalMatrix == otherRotation.InternalMatrix)
            {
                return true; 
            }

            return false; 
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(InternalMatrix, isRotationMatrix);
        }
        
        public override EulerAngleRotation ToEulerAngleRotation()
        {
            EulerAngleRotation newEulerAngle = new EulerAngleRotation(0, 0, 0); 
            newEulerAngle.GetValuesFromMatrix(this);
            return newEulerAngle; 
        }

        public override QuaternionRotation ToQuaternionRotation()
        {
            if (!isRotationMatrix)
            {
                isRotationMatrix = InternalMatrix.IsSpecialOrthogonal(); 
            }

            if (!isRotationMatrix)
            {
                throw new Exception("Can't transform matrix to Quaternion, because it isn't a rotationMatrix"); 
            }

            float trace = InternalMatrix.Trace();
            float w, x, y, z;

            if (trace > 0)
            {
                float s = 0.5f / Mathf.Sqrt(trace + 1.0f);
                w = 0.25f / s;
                x = (InternalMatrix[2, 1] - InternalMatrix[1, 2]) * s;
                y = (InternalMatrix[0, 2] - InternalMatrix[2, 0]) * s;
                z = (InternalMatrix[1, 0] - InternalMatrix[0, 1]) * s;
            }
            else
            {
                if (InternalMatrix[0, 0] > InternalMatrix[1, 1] && InternalMatrix[0, 0] > InternalMatrix[2, 2])
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + InternalMatrix[0, 0] - InternalMatrix[1, 1] - InternalMatrix[2, 2]);
                    w = (InternalMatrix[2, 1] - InternalMatrix[1, 2]) / s;
                    x = 0.25f * s;
                    y = (InternalMatrix[0, 1] + InternalMatrix[1, 0]) / s;
                    z = (InternalMatrix[0, 2] + InternalMatrix[2, 0]) / s;
                }
                else if (InternalMatrix[1, 1] > InternalMatrix[2, 2])
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + InternalMatrix[1, 1] - InternalMatrix[0, 0] - InternalMatrix[2, 2]);
                    w = (InternalMatrix[0, 2] - InternalMatrix[2, 0]) / s;
                    x = (InternalMatrix[0, 1] + InternalMatrix[1, 0]) / s;
                    y = 0.25f * s;
                    z = (InternalMatrix[1, 2] + InternalMatrix[2, 1]) / s;
                }
                else
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + InternalMatrix[2, 2] - InternalMatrix[0, 0] - InternalMatrix[1, 1]);
                    w = (InternalMatrix[1, 0] - InternalMatrix[0, 1]) / s;
                    x = (InternalMatrix[0, 2] + InternalMatrix[2, 0]) / s;
                    y = (InternalMatrix[1, 2] + InternalMatrix[2, 1]) / s;
                    z = 0.25f * s;
                }
            }

            return new QuaternionRotation(w, x, y, z);
        }

        public override MatrixRotation ToMatrixRotation()
        {
            return new MatrixRotation(InternalMatrix); 
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
            return InternalMatrix * inVector; 
        }
    }
}
