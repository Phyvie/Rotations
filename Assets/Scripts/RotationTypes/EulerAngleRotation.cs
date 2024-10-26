using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RotationTypes
{
    public enum EGimbleType
    {
        Invalid, 
        TrueEulerAngle, 
        TaitBryanAngle, 
        OversizedEulerAngle, 
        OversizedTaitBryanAngle
    }
    
    [Serializable]
    public class EulerAngleRotation : RotationType
    {
        [SerializeField] private List<SingleGimbleRotation> gimble;
        [SerializeField] private bool isIntrinsic = true;
        [SerializeField] private EGimbleType _gimbleType = EGimbleType.Invalid; 
        
        public bool IsIntrinsic
        {
            get => isIntrinsic;
            set => isIntrinsic = value;
        }

        public EGimbleType GimbleType
        {
            get
            {
                return (_gimbleType = GetGimbleType());
            }
        }

        public EulerAngleRotation()
        {
            SingleGimbleRotation yaw = SingleGimbleRotation.Yaw.Clone();
            SingleGimbleRotation pitch = SingleGimbleRotation.Pitch.Clone();
            SingleGimbleRotation roll = SingleGimbleRotation.Roll.Clone();

            gimble = new List<SingleGimbleRotation>() { yaw, pitch, roll}; 
        }
        
        public EulerAngleRotation(float inRoll, float inYaw, float inPitch, AngleType inAngleType)
        {
            gimble = new List<SingleGimbleRotation>()
            {
                SingleGimbleRotation.Yaw.Clone(), 
                SingleGimbleRotation.Pitch.Clone(), 
                SingleGimbleRotation.Roll.Clone(), 
            }; 
            
            if (this.angleType is null && inAngleType is null)
            {
                Debug.LogError("Can't Construct EulerAngleRotation without angleType");
                return; 
            }

            angleType = inAngleType; 
        }

        public override void SetAngleType(AngleType value)
        {
            foreach (SingleGimbleRotation gimbleRing in gimble)
            {
                gimbleRing.angleType = value; 
            }
            _angleType = value; 
        }
        
        public void SetRotationValue(EGimbleAxis gimbleAxis, float value)
        {
            gimble.First(gimbleRing => gimbleRing.eAxis == gimbleAxis).angle = value; 
        }

        public float GetRotationValue(EGimbleAxis gimbleAxis)
        {
            return gimble.First(gimbleRing => gimbleRing.eAxis == gimbleAxis).angle; 
        }

        public void SwitchGimbleOrder(SingleGimbleRotation firstRotation, SingleGimbleRotation secondRotation)
        {
            (firstRotation, secondRotation) = (secondRotation, firstRotation); //TODO: test this function
        }

        public bool ValidateGimble()
        {
            return GimbleType != EGimbleType.Invalid; 
        }

        private EGimbleType GetGimbleType()
        {
            HashSet<EGimbleAxis> gimbleAxisSet = new HashSet<EGimbleAxis>();
            IEnumerator<SingleGimbleRotation> gimbleIterator = gimble.GetEnumerator();
            EGimbleAxis lastGimbleAxis = gimbleIterator.Current.eAxis;
            while (gimbleIterator.MoveNext())
            {
                if (gimbleAxisSet.Contains(lastGimbleAxis))
                {
                    if (lastGimbleAxis != gimbleIterator.Current.eAxis)
                    {
                        if (gimble.Count > 3)
                        {
                            gimbleIterator.Dispose();
                            return EGimbleType.OversizedEulerAngle; 
                        }
                        gimbleIterator.Dispose();
                        return EGimbleType.TrueEulerAngle; 
                    }
                }
            }
            if (gimbleAxisSet.Count == 3)
            {
                if (gimble.Count > 3)
                {
                    gimbleIterator.Dispose();
                    return EGimbleType.OversizedTaitBryanAngle; 
                }
                gimbleIterator.Dispose();
                return EGimbleType.TaitBryanAngle; 
            }
            
            gimbleIterator.Dispose();
            return EGimbleType.Invalid; 
        }

        public override EulerAngleRotation ToEulerAngleRotation()
        {
            throw new NotImplementedException();
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
            MatrixRotation result = new MatrixRotation(MatrixRotation.Identity(3));
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

        public override Vector3 RotateVector(Vector3 inVector)
        {
            throw new NotImplementedException();
        }
    }
}
