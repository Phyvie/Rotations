using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class ComplexRotation : RotationType
    {
        [SerializeField] private Vector2 _complexNumber = Vector2.zero; 
        [SerializeField] private float _rotationAngle = 0;
        [SerializeField] private Matrix _matrix = new float[2, 2]
        {
            { 1, 0 },
            { 0, 1 }
        };

        public Vector2 ComplexNumber
        {
            get => _complexNumber;
            set
            {
                if (_complexNumber == value)
                {
                    return; 
                }
                    
                _complexNumber = value;
                
                _rotationAngle = Mathf.Atan2(_complexNumber.y, _complexNumber.x);
                _rotationAngle = AngleType.ConvertAngle(_rotationAngle, AngleType.Radian, angleType);
                
                _matrix = new Matrix(new float[2,2]
                {
                    {_complexNumber.x, -_complexNumber.y },
                    {_complexNumber.y,  _complexNumber.x }
                }); 
            }
        }

        public float RotationAngle
        {
            get => _rotationAngle;
            set
            {
                if (_rotationAngle == value)
                {
                    return; 
                }
                
                _rotationAngle = value;
                float radianAngle = AngleType.ConvertAngle(_rotationAngle, angleType, AngleType.Radian); 
                
                _complexNumber = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)); //+x/+y = tan(radianValue) = -x/-y => atan(y/x) = +-radianValue = atan(-y/-x) (use atan2 to get the correct +-)
                
                _matrix = new float[2,2]
                {
                    {_complexNumber.x, -_complexNumber.y },
                    {_complexNumber.y,  _complexNumber.x }
                }; 
            }
        }

        public Matrix Matrix2X2
        {
            get => _matrix;
            set
            {
                if (_matrix == value)
                {
                    return; 
                }
                
                _matrix = value;

                _complexNumber = new Vector2(_matrix[0,0], _matrix[0,1]); 
                
                _rotationAngle = Mathf.Atan2(_complexNumber.y, _complexNumber.x);
                _rotationAngle = AngleType.ConvertAngle(_rotationAngle, AngleType.Radian, angleType);
            }
        }

        public override EulerAngleRotation ToEulerAngleRotation()
        {
            return new EulerAngleRotation(0, 0, _rotationAngle, AngleType.Radian); 
        }

        public override QuaternionRotation ToQuaternionRotation()
        {
            return new QuaternionRotation(_complexNumber.x, 0, 0, _complexNumber.y); 
        }

        public override MatrixRotation ToMatrixRotation()
        {
            return new MatrixRotation(
                new float[3,3]
                {
                    { _matrix[0,0], _matrix[0,1], 0 },
                    { _matrix[1,0], _matrix[1,1], 0 },
                    {       0,           0,       1 }
                }
            ); 
        }

        public override AxisAngleRotation ToAxisAngleRotation()
        {
            return new AxisAngleRotation(Vector3.back * _rotationAngle, AngleType.Radian); 
        }

        public override void SetAngleType(AngleType value)
        {
            _rotationAngle = AngleType.ConvertAngle(_rotationAngle, angleType, value); 
            _angleType = value; 
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            Vector2 rotatedVector = RotateVector2_ViaComplex(new Vector2(inVector.x, inVector.y)); 
            return new Vector3(rotatedVector.x, rotatedVector.y, inVector.z); 
        }

        public Vector2 RotateVector2_ViaComplex(Vector2 inVector)
        {
            return new Vector2(
                inVector.x * _complexNumber.x - inVector.y * _complexNumber.y, 
                inVector.x * _complexNumber.y + inVector.y * _complexNumber.x
                ); 
        }

        public Vector2 RotateVector2_ViaMatrix(Vector2 inVector)
        {
            return new Vector2(
                inVector.x * _matrix[0,0] + inVector.y * _matrix[0,1], 
                inVector.x * _matrix[1,0] + inVector.y * _matrix[1,1]
            );  
        }
    }
}
