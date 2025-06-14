using System;
using MathExtensions;
using Unity.Properties;
using UnityEngine;


namespace RotParams
{
    [Serializable]
    public class RotParams_Matrix : RotParams
    {
        public bool isRotationMatrix => InternalMatrix.IsSpecialOrthogonal();
        [SerializeField] public Matrix InternalMatrix;

        #region Constructors
        public RotParams_Matrix()
        {
            InternalMatrix = new Matrix(Matrix.Identity(3)); 
        }
        
        public RotParams_Matrix(RotParams_Matrix copiedRotParamsMatrix)
        {
            InternalMatrix = new Matrix(copiedRotParamsMatrix.InternalMatrix);
        }

        public RotParams_Matrix(Matrix matrix)
        {
            InternalMatrix = new Matrix(matrix);
        }
        
        public static RotParams_Matrix RotationIdentity()
        {
            return new RotParams_Matrix(Matrix.Identity(3));
        }

        public static RotParams_Matrix TransformIdentity()
        {
            return new RotParams_Matrix(Matrix.Identity(4)); 
        }
        #endregion //Constructors
        
        #region GetSet
        [CreateProperty]
        public Vector3 XVector
        {
            get => GetColumn(0);
            set
            {
                if (Mathf.Approximately((XVector - value).sqrMagnitude, 0f))
                {
                    return; 
                }
                SetColumn(0, value);
                OnPropertyChanged(nameof(XVector)); 
            }
        }

        [CreateProperty]
        public Vector3 YVector
        {
            get => GetColumn(1);
            set
            {
                if (Mathf.Approximately((XVector - value).sqrMagnitude, 0f))
                {
                    return; 
                }
                SetColumn(1, value);
                OnPropertyChanged(nameof(YVector));
            }
        }

        [CreateProperty]
        public Vector3 ZVector
        {
            get => GetColumn(2);
            set
            {
                if (Mathf.Approximately((XVector - value).sqrMagnitude, 0f))
                {
                    return; 
                }
                SetColumn(2, value); 
                OnPropertyChanged(nameof(ZVector));
            }
        }

        public float this[int row, int column]
        {
            get => InternalMatrix[row, column];
            set
            {
                if (Mathf.Approximately(this[row, column], value))
                {
                    return;
                }
                InternalMatrix[row, column] = value;
                OnPropertyChanged($"{nameof(InternalMatrix)}[{row},{column}]");
            }
        }
        
        public Vector3 GetRow(int row)
        {
            return new Vector3(InternalMatrix[row, 0], InternalMatrix[row, 1], InternalMatrix[row, 2]); 
        }

        public Vector3 GetColumn(int column)
        {
            return new Vector3(InternalMatrix[0, column], InternalMatrix[1, column], InternalMatrix[2, column]);
        }

        public void SetRow(int rowIndex, Vector3 row)
        {
            InternalMatrix[rowIndex, 0] = row.x;
            InternalMatrix[rowIndex, 1] = row.y;
            InternalMatrix[rowIndex, 2] = row.z;
            OnPropertyChanged($"{nameof(InternalMatrix)}[{row}");
        }

        public void SetColumn(int columnIndex, Vector3 column)
        {
            InternalMatrix[0, columnIndex] = column.x;
            InternalMatrix[1, columnIndex] = column.y;
            InternalMatrix[2, columnIndex] = column.z;
            OnPropertyChanged($"{nameof(InternalMatrix)}[{column}");
        }
        #endregion

        #region Comparison
        public static bool operator==(RotParams_Matrix first, RotParams_Matrix second)
        {
            if (first is null && second is null)
            {
                return true; 
            }
            
            return first is not null && first.Equals(second); 
        }

        public static bool operator !=(RotParams_Matrix first, RotParams_Matrix second)
        {
            if (first is null != second is null)
            {
                return true; 
            } 
            else if (first is null)
            {
                return true; 
            }
            else
            {
                return first.Equals(second); 
            }
        }

        public static readonly float EqualsTolerance = 0.0001f; 
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            
            RotParams_Matrix other = (RotParams_Matrix)obj; 
            if (InternalMatrix == other.InternalMatrix)
            {
                return true; 
            }

            return false; 
        }
        #endregion //Comparison
        
        #region Math
        public RotParams_Matrix Inverse()
        {
            return new RotParams_Matrix(InternalMatrix.Inverse()); 
        }
        
        public static RotParams_Matrix operator*(RotParams_Matrix first, RotParams_Matrix second)
        {
            return new RotParams_Matrix(first.InternalMatrix * second.InternalMatrix); 
        }
        #endregion //Math
        
        public override int GetHashCode()
        {
            return HashCode.Combine(InternalMatrix, isRotationMatrix);
        }
        
        #region Converters
        public override RotParams_EulerAngles ToEulerAngleRotation()
        {
            RotParams_EulerAngles newRotParamsEulerAngles = new RotParams_EulerAngles(0, 0, 0); 
            newRotParamsEulerAngles.GetValuesFromMatrix(this);
            return newRotParamsEulerAngles; 
        }

        public override RotParams_Quaternion ToQuaternionRotation()
        {
            if (!isRotationMatrix)
            {
                Debug.LogWarning($"{nameof(ToQuaternionRotation)} error: Matrix is not a RotationMatrix");
                // return new RotParams_Quaternion(); //!ZyKa
            
                //-ZyKa MatrixToRotationMatrix, adjust so that the priority of directions can be chosen by user
                Vector3 Forward = GetColumn(2).normalized;
                Vector3 Up = GetColumn(1).normalized;
                Vector3 Right = Vector3.Cross(Up,Forward).normalized;
                Up = Vector3.Cross(Forward,Right);
                
                RotParams_Matrix AdjustedMatrix = new RotParams_Matrix(InternalMatrix);
                
                AdjustedMatrix.SetColumn(0, Right);
                AdjustedMatrix.SetColumn(1, Up);
                AdjustedMatrix.SetColumn(2, Forward);
                
                if (!AdjustedMatrix.isRotationMatrix)
                {
                    Debug.LogError($"{nameof(ToQuaternionRotation)}: error: Matrix column-vectors are parallel");
                    return new RotParams_Quaternion(); 
                }
                
                return AdjustedMatrix.ToQuaternionRotation(); 
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

            return new RotParams_Quaternion(w, x, y, z);
        }

        public override RotParams_Matrix ToMatrixRotation()
        {
            return new RotParams_Matrix(InternalMatrix); 
        }

        public override RotParams_AxisAngle ToAxisAngleRotation()
        {
            return ToQuaternionRotation().ToAxisAngleRotation(); 
        }

        public override void ResetToIdentity()
        {
            InternalMatrix = Matrix.Identity(3);
        }

        #endregion //Converters

        public override Vector3 RotateVector(Vector3 inVector)
        {
            return InternalMatrix * inVector; 
        }
    }
}
