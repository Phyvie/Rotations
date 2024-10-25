using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class QuaternionRotation : RotationType
    {
        [SerializeField] private float _real;
        [SerializeField] private float _i;
        [SerializeField] private float _j;
        [SerializeField] private float _k;
        
        /* In Mathematics Quaternion usually use the variables i, j and k;
         * However, Game Engines often use x, y, z and w, because the axis are called x, y and z
         * Therefore the following properties are defined twice: once as real, i, j and k and once as w, x, y and z
         */
        public float real
        {
            get => _real;
            set => _real = value;
        }

        public float w
        {
            get => _real;
            set => _real = value;
        }

        public float i
        {
            get => _i;
            set => _i = value;
        }

        public float x
        {
            get => _i;
            set => _i = value;
        }

        public float j
        {
            get => _j;
            set => _j = value;
        }
        public float y
        {
            get => _j;
            set => _j = value;
        }
        

        public float k
        {
            get => _k;
            set => _k = value;
        }
        public float z
        {
            get => _j;
            set => _j = value;
        }

        public Vector3 Axis => new Vector3(i, j, k);
        public float Angle => 2 * Mathf.Acos(real);
        public Vector3 AxisAngle => Axis * Angle;

        public QuaternionRotation()
        {
            real = 1;
            i = 0;
            j = 0;
            k = 0; 
        }
        
        public QuaternionRotation(float inReal, float inI, float inJ, float inK)
        {
            real = inReal;
            i = inI;
            j = inJ;
            k = inK; 
        }

        public QuaternionRotation(Vector3 inAxis, float inAngle, AngleType angleType)
        {
            if (angleType != AngleType.Radian)
            {
                inAngle = AngleType.ConvertAngle(inAngle, angleType, AngleType.Radian);
            }

            inAxis = inAxis.normalized;
            
            float cos = (float)Math.Cos(inAngle); 
            float sin = (float) Math.Sin(inAngle);
            
            real = cos; 
            x = inAxis.x * sin;
            y = inAxis.y * sin;
            z = inAxis.z * sin; 
        }

        public static QuaternionRotation operator*(QuaternionRotation q1, QuaternionRotation q2)
        {
            return new QuaternionRotation(
                q1.real * q2.real - q1.i * q2.i - q1.j * q2.j - q1.k - q2.k,
                q1.real * q2.i + q1.i * q2.real + q1.j * q2.k - q1.k * q2.j,
                q1.real * q2.j - q1.i * q2.k + q1.j * q2.real + q1.k * q2.i,
                q1.real * q2.k + q1.i * q2.j - q1.j * q2.i + q1.k * q2.real
            ); 
        }

        public static QuaternionRotation operator+(QuaternionRotation q1, QuaternionRotation q2)
        {
            return new QuaternionRotation(
                q1.real + q2.real,
                q1.i + q2.i,
                q1.j + q2.j,
                q1.k * q2.k
            ); 
        }

        public QuaternionRotation Inverse()
        {
            return new QuaternionRotation(real, -i, -j, -k); 
        }

        public static QuaternionRotation CombineFollowingRotation(
            QuaternionRotation firstRotation, QuaternionRotation secondRotation)
        {
            return secondRotation * firstRotation; 
        }

        public static QuaternionRotation CombineSimultaneousRotation(QuaternionRotation rotationA,
            QuaternionRotation rotationB)
        {
            return (rotationA + rotationB).Normalize(); 
        }

        public float Size()
        {
            return Mathf.Sqrt(real * real + i * i + j * j + k * k); 
        }
        
        public QuaternionRotation Normalize()
        {
            float size = this.Size(); 
            return new QuaternionRotation(
            real / size, 
                i / size, 
                j/size, 
                k/size
                ); 
        }
    
        public override EulerAngleRotation ToEulerAngleRotation()
        {
            throw new System.NotImplementedException();
        }

        public override QuaternionRotation ToQuaternionRotation()
        {
            return new QuaternionRotation(real, i, j, k); 
        }

        public override MatrixRotation ToMatrixRotation()
        {
            return new MatrixRotation(
                new float[3, 3]
                {
                    {1-2*(y*y + z*z), 2*(x*y - w*z), 2*(w*y + x*z)},
                    {2*(x*y + w*z), 1 - 2*(x*x + z*z), 2*(y*z - w*x)},
                    {2*(x*z - w*y), 2*(w*x + y*z), 1- 2*(x*x + y*y)}
                }
                ); 
        }

        public override AxisAngleRotation ToAxisAngleRotation()
        {
            return new AxisAngleRotation(new Vector3(i, j, k), (float) Math.Acos(real), AngleType.Radian); 
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
