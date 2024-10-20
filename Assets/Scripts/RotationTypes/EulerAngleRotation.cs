using System;
using System.Collections.Generic;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class EulerAngleRotation : RotationType
    {
        [SerializeField] private SingleGimbleRotation yaw = SingleGimbleRotation.Yaw.Clone();
        [SerializeField] private SingleGimbleRotation pitch = SingleGimbleRotation.Pitch.Clone();
        [SerializeField] private SingleGimbleRotation roll = SingleGimbleRotation.Roll.Clone();

        [SerializeField] private List<SingleGimbleRotation> gimble; 
        
        public float Roll
        {
            get => roll.angle;
            set => roll.angle = value;
        }

        public float Yaw
        {
            get => yaw.angle;
            set => yaw.angle = value;
        }

        public float Pitch
        {
            get => pitch.angle;
            set => pitch.angle = value;
        }

        public EulerAngleRotation(float inRoll, float inYaw, float inPitch, AngleType inAngleType)
        {
            gimble = new List<SingleGimbleRotation>(){yaw, pitch, roll}; 
            
            if (this.angleType is null && inAngleType is null)
            {
                Debug.LogError("Can't Construct EulerAngleRotation without angleType");
                return; 
            }

            angleType = inAngleType; 
            Roll = inRoll;
            Yaw = inYaw;
            Pitch = inPitch; 
        }

        EulerAngleRotation changeRotationValues(float inRoll, float inYaw, float inPitch, AngleType inAngleType = null)
        {
            if (angleType is not null && angleType != this.angleType)
            {
                inRoll = AngleType.ConvertAngle(inRoll, inAngleType, angleType); 
                inYaw = AngleType.ConvertAngle(inYaw, inAngleType, angleType); 
                inPitch = AngleType.ConvertAngle(inPitch, inAngleType, angleType); 
            }
            
            return new EulerAngleRotation(inRoll, inYaw, inPitch, angleType); 
        }

        EulerAngleRotation changeAngleType(AngleType inAngleType)
        {
            Roll = AngleType.ConvertAngle(Roll, angleType, inAngleType); 
            Yaw = AngleType.ConvertAngle(Yaw, angleType, inAngleType); 
            Pitch = AngleType.ConvertAngle(Pitch, angleType, inAngleType); 
            _angleType = inAngleType;
            return this; 
        }

        public void SwitchGimbleOrder(SingleGimbleRotation firstRotation, SingleGimbleRotation secondRotation)
        {
            (firstRotation, secondRotation) = (secondRotation, firstRotation); //TODO: test this function
        }
        
        public override EulerAngleRotation ToEulerAngleRotation()
        {
            return this; 
        }
        
        public override QuaternionRotation ToQuaternionRotation() //TODO: test this function
        {
            QuaternionRotation result = new QuaternionRotation();
            foreach (SingleGimbleRotation rotation in gimble)
            {
                result = result * rotation.toQuaternionRotation() * result.Inverse(); 
            }
            return result; 
        }

        public EulerAngleRotation FromQuaternion()
        {
            EulerAngleRotation euler = new EulerAngleRotation(0, 0, 0, AngleType.Radian);
            throw new NotImplementedException(); 
        }
        
        //ToQuaternionRotation.ToMatrixRotation is cheaper, because converting from EulerAngles to another RotationType requires combining multiple rotations throughout the process which is cheaper in Quaternions
        public override MatrixRotation ToMatrixRotation() //TODO: test this function
        {
            MatrixRotation result = new MatrixRotation(MatrixRotation.Identity);
            foreach (SingleGimbleRotation rotation in gimble)
            {
                result = result * rotation.toMatrixRotation() * result.Inverse(); 
            }
            return result; 
        }

        public override AxisAngleRotation ToAxisAngleRotation()
        {
            return ToQuaternionRotation().ToAxisAngleRotation(); 
        }

        public override void SetAngleType(AngleType value)
        {
            Yaw = AngleType.ConvertAngle(Yaw, angleType, value); 
            Pitch = AngleType.ConvertAngle(Pitch, angleType, value); 
            Roll = AngleType.ConvertAngle(Roll, angleType, value); 
            _angleType = value; 
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            throw new NotImplementedException();
        }
    }
}
