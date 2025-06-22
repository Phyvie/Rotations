using System;
using MathExtensions;
using UnityEngine;

namespace RotParams
{
    [Serializable]
    public class RotParams_Complex : RotParams_Base
    {
        [SerializeField] private Vector2 _complexNumber = Vector2.zero; 
        [SerializeField] private float _rotationAngle;
        private Matrix Matrix => new float[2,2]
        {
            {_complexNumber.x, -_complexNumber.y },
            {_complexNumber.y,  _complexNumber.x }
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
                
                _complexNumber = new Vector2(Mathf.Cos(_rotationAngle), Mathf.Sin(_rotationAngle)); //+x/+y = tan(radianValue) = -x/-y => atan(y/x) = +-radianValue = atan(-y/-x) (use atan2 to get the correct +-)
            }
        }
        
        public override RotParams_EulerAngles ToEulerAngleRotation()
        {
            return new RotParams_EulerAngles(0, 0, _rotationAngle); 
        }

        public override RotParams_Quaternion ToQuaternionRotation()
        {
            return new RotParams_Quaternion(_complexNumber.x, 0, 0, _complexNumber.y); 
        }

        public override RotParams_Matrix ToMatrixRotation()
        {
            return new RotParams_Matrix(
                new float[3,3]
                {
                    { Matrix[0,0], Matrix[0,1], 0 },
                    { Matrix[1,0], Matrix[1,1], 0 },
                    {       0,           0,       1 }
                }
            ); 
        }

        public override RotParams_AxisAngle ToAxisAngleRotation()
        {
            return new RotParams_AxisAngle(Vector3.back * _rotationAngle); 
        }

        public override void ResetToIdentity()
        {
            ComplexNumber = new Vector2(0, 0);
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            Vector2 rotatedVector = RotateVector2_ViaComplex(new Vector2(inVector.x, inVector.y)); 
            return new Vector3(rotatedVector.x, rotatedVector.y, inVector.z); 
        }

        public override string ToString()
        {
            return $"{_complexNumber}"; 
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
                inVector.x * Matrix[0,0] + inVector.y * Matrix[0,1], 
                inVector.x * Matrix[1,0] + inVector.y * Matrix[1,1]
            );  
        }
    }
}
