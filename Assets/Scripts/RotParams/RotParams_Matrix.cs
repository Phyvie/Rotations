using System;
using MathExtensions;
using Unity.Properties;
using UnityEngine;


namespace RotParams
{
    [Serializable]
    public class RotParams_Matrix : RotParams_Base
    {
        #region Variables
        public bool isRotationMatrix => InternalMatrix.IsSpecialOrthogonal();
        [SerializeField] public Matrix InternalMatrix;

        [SerializeField] private int primaryAxisIndex = 2;
        [SerializeField] private int secondaryAxisIndex = 1;
        #endregion
        
        #region Properties
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

        [CreateProperty]
        public int PrimaryAxisIndex
        {
            get => primaryAxisIndex;
            set
            {
                primaryAxisIndex = value;
                OnPropertyChanged(nameof(PrimaryAxisIndex));
            }
        }

        [CreateProperty]
        public int SecondaryAxisIndex
        {
            get => secondaryAxisIndex;
            set
            {
                secondaryAxisIndex = value;
                OnPropertyChanged(nameof(SecondaryAxisIndex));
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
        #endregion Properties

        #region Constructors
        public override void CopyValues(RotParams_Base toCopy)
        {
            if (toCopy is RotParams_Matrix rotParams)
            {
                this.InternalMatrix = new Matrix(rotParams.InternalMatrix);
            }
            else
            {
                CopyValues(toCopy.ToAxisAngleParams());
            }
        }

        public RotParams_Matrix()
        {
            InternalMatrix = new Matrix(Matrix.Identity(3)); 
        }
        
        public RotParams_Matrix(RotParams_Matrix toCopy)
        {
            InternalMatrix = new Matrix(toCopy.InternalMatrix);
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
        
        public override RotParams_Base GetIdentity()
        {
            return RotationIdentity();
        }
        #endregion //Constructors

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
        
        public override int GetHashCode()
        {
            return HashCode.Combine(InternalMatrix, isRotationMatrix);
        }
        #endregion //Comparison
        
        #region operators&arithmetic
        public static RotParams_Matrix operator*(RotParams_Matrix matrix, RotParams_Matrix second)
        {
            return new RotParams_Matrix(matrix.InternalMatrix * second.InternalMatrix); 
        }

        public static RotParams_Matrix operator +(RotParams_Matrix first, RotParams_Matrix second)
        {
            return new RotParams_Matrix(first.InternalMatrix + second.InternalMatrix);
        }
        
        public static RotParams_Matrix operator -(RotParams_Matrix first, RotParams_Matrix second)
        {
            return new RotParams_Matrix(first.InternalMatrix - second.InternalMatrix);
        }

        public static RotParams_Matrix operator *(float scalar, RotParams_Matrix matrix)
        {
            return matrix * scalar; 
        }
        
        public static RotParams_Matrix operator *(RotParams_Matrix matrix, float scalar)
        {
            return new RotParams_Matrix(matrix.InternalMatrix * scalar);
        }
        
        public RotParams_Matrix Inverse()
        {
            return new RotParams_Matrix(InternalMatrix.Inverse()); 
        }

        public RotParams_Matrix Transpose()
        {
            return new RotParams_Matrix(InternalMatrix.Transpose());
        }
        #endregion operators&arithmetic
        
        #region Converters

        public override RotParams_Base ToSelfType(RotParams_Base toConvert)
        {
            return toConvert.ToMatrixParams(); 
        }

        public override RotParams_EulerAngles ToEulerParams()
        {
            RotParams_EulerAngles newRotParamsEulerAngles = new RotParams_EulerAngles(0, 0, 0);
            newRotParamsEulerAngles.OuterAngle = Mathf.Atan2(this[0, 1], this[1, 1]);
            newRotParamsEulerAngles.MiddleAngle = Mathf.Asin(this[2, 1]);  
            newRotParamsEulerAngles.InnerAngle = Mathf.Atan2(this[2, 0], this[2, 2]); 
            return newRotParamsEulerAngles; 
        }

        public override RotParams_Quaternion ToQuaternionParams()
        {
            if (!isRotationMatrix)
            {
                Debug.LogWarning($"{nameof(ToQuaternionParams)} error: Matrix is not a RotationMatrix, getting Quaternion for XYRotationMatrix");
                return ToRotationMatrixFromTwoAxes(primaryAxisIndex, secondaryAxisIndex).ToQuaternionParams(); //-ZyKa this should use GetClosestRotationMatrix
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

        public override RotParams_Matrix ToMatrixParams()
        {
            return new RotParams_Matrix(InternalMatrix); 
        }

        public override RotParams_AxisAngle ToAxisAngleParams()
        {
            return ToQuaternionParams().ToAxisAngleParams(); 
        }
        
        public override void ResetToIdentity()
        {
            InternalMatrix = Matrix.Identity(3);
        }

        public RotParams_Matrix ToRotationMatrixFromTwoAxes(int firstAxisIndex, int secondAxisIndex)
        {
            if (firstAxisIndex < 0 || firstAxisIndex > 2 || secondAxisIndex < 0 || secondAxisIndex > 2)
            {
                Debug.LogError("Axis indices must be between 0 and 2 (inclusive).");
                return null;
            }

            if (firstAxisIndex == secondAxisIndex)
            {
                Debug.LogError("First and second axis indices must be different.");
                return null;
            }
            int thirdAxisIndex = 3 - firstAxisIndex - secondAxisIndex;
            
            Vector3 firstAxis = GetColumn(firstAxisIndex).normalized;
            Vector3 secondAxis = GetColumn(secondAxisIndex).normalized;
            Vector3 thirdAxis; 

            if (Mathf.Abs(Vector3.Dot(firstAxis, secondAxis)) > 0.999f)
            {
                Debug.LogWarning("Provided axes are nearly parallel; trying to construct rotation matrix from other axes.");

                thirdAxis = GetColumn(thirdAxisIndex).normalized;
                if (Mathf.Abs(Vector3.Dot(firstAxis, thirdAxis)) < 0.995f)
                {
                    return ToRotationMatrixFromTwoAxes(firstAxisIndex, thirdAxisIndex);
                }
                else if (Mathf.Abs(Vector3.Dot(secondAxis, thirdAxis)) < 0.995f)
                {
                    return ToRotationMatrixFromTwoAxes(secondAxisIndex, thirdAxisIndex);
                }
                else
                {
                    Debug.LogError("Matrix axes are all parallel, returning IdentityMatrix");
                    return RotationIdentity(); 
                }
                return null;
            }
            
            bool crossProductForward = (firstAxisIndex + 1) % 3 == secondAxisIndex; 
            thirdAxis = crossProductForward ? 
                Vector3.Cross(firstAxis, secondAxis).normalized : 
                Vector3.Cross(secondAxis, firstAxis).normalized;
            
            secondAxis = crossProductForward ? 
                Vector3.Cross(thirdAxis, firstAxis).normalized :
                Vector3.Cross(firstAxis, thirdAxis).normalized;
            
            RotParams_Matrix adjustedMatrix = new RotParams_Matrix();
            adjustedMatrix.SetColumn(firstAxisIndex, firstAxis);
            adjustedMatrix.SetColumn(secondAxisIndex, secondAxis);
            adjustedMatrix.SetColumn(thirdAxisIndex, thirdAxis);

            if (!adjustedMatrix.isRotationMatrix)
            {        
                Debug.LogError("Constructed matrix is not a valid rotation matrix. Axes may be parallel or ill-defined.");
                return null;
            }
            return adjustedMatrix;
        }

        public RotParams_Matrix GetClosestRotationMatrix()
        {
            throw new NotImplementedException("GetClosestRotationMatrix is not implemented.");
        }
        #endregion //Converters

        #region Functions
        public override RotParams_Base GetInverse()
        {
            return Inverse(); 
        }

        protected override RotParams_Base Concatenate_Implementation(RotParams_Base otherRotation, bool otherFirst = false)
        {
            return otherFirst ? 
                this * (otherRotation as RotParams_Matrix) : 
                (otherRotation as RotParams_Matrix) * this; 
        }
        
        public override Vector3 RotateVector(Vector3 inVector)
        {
            return InternalMatrix * inVector; 
        }

        public override string ToString()
        {
            return $"{InternalMatrix.ToString()}"; 
        }

        public static RotParams_Matrix Lerp(RotParams_Matrix from, RotParams_Matrix to, float t)
        {
            return from + t * (to - from); 
        }
        
        public static RotParams_Matrix LerpThenToRotationByAxes(RotParams_Matrix from, RotParams_Matrix to, float t, int primaryAxisIndex, int secondaryAxisIndex)
        {
            return Lerp(from, to, t).ToRotationMatrixFromTwoAxes(primaryAxisIndex, secondaryAxisIndex); 
        }

        public static RotParams_Matrix LerpThenGetClosestRotationMatrix(RotParams_Matrix from, RotParams_Matrix to,
            float t)
        {
            return Lerp(from, to, t).GetClosestRotationMatrix();
        }
        #endregion Functions
    }
}
