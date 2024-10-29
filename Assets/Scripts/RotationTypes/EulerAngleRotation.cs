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
        [SerializeField] public List<AngleType> ZyKa;
        
        [SerializeField] private List<SingleGimbleRotation> gimble;
        [SerializeField] private bool isIntrinsic = true;
        
        public bool IsIntrinsic
        {
            get => isIntrinsic;
            set => isIntrinsic = value;
        }

        public EulerAngleRotation()
        {
            SingleGimbleRotation yaw = SingleGimbleRotation.Yaw.Clone();
            SingleGimbleRotation pitch = SingleGimbleRotation.Pitch.Clone();
            SingleGimbleRotation roll = SingleGimbleRotation.Roll.Clone();

            gimble = new List<SingleGimbleRotation>() { yaw, pitch, roll}; 
        }

        public EulerAngleRotation(List<SingleGimbleRotation> gimble)
        {
            this.gimble = new List<SingleGimbleRotation>();
            foreach (SingleGimbleRotation gimbleRing in gimble)
            {
                this.gimble.Add(gimbleRing);
            }
        }

        public EulerAngleRotation(float inRoll, float inYaw, float inPitch) : 
            this(inRoll, inYaw, inPitch, AngleType.Radian)
        {
            
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

        public bool IsGimbleValid()
        {
            return GetGimbleType() != EGimbleType.Invalid; 
        }

        public EGimbleType GetGimbleType()
        {
            HashSet<EGimbleAxis> gimbleAxisSet = new HashSet<EGimbleAxis>();
            IEnumerator<SingleGimbleRotation> gimbleIterator = gimble.GetEnumerator();
            gimbleIterator.MoveNext(); 
            EGimbleAxis lastGimbleAxis = gimbleIterator.Current.eAxis;
            gimbleAxisSet.Add(gimbleIterator.Current.eAxis); 
            while (gimbleIterator.MoveNext())
            {
                if (gimbleAxisSet.Contains(gimbleIterator.Current.eAxis))
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

                lastGimbleAxis = gimbleIterator.Current.eAxis; 
                gimbleAxisSet.Add(gimbleIterator.Current.eAxis); 
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

        public String GetAddButtonName()
        {
            EGimbleType gimbleType = GetGimbleType(); 
            if (gimbleType != EGimbleType.Invalid)
            {
                return "Oversize Gimble"; 
            }

            if (gimble.Count >= 3)
            {
                return "Oversize Invalid Gimble"; 
            }

            return "Add GimbleRing"; 
        }

        public String GetRemoveButtonName()
        {
            EGimbleType gimbleType = GetGimbleType(); 
            if (gimble.Count == 3)
            {
                return "Undersize Gimble"; 
            }

            return "Remove surplus GimbleRing"; 
        }

        public override EulerAngleRotation ToEulerAngleRotation()
        {
            return new EulerAngleRotation(gimble); 
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

        public void GetValuesFromQuaternion(QuaternionRotation quaternionRotation)
        {
            QuaternionRotation quaternionRotationCopy = new QuaternionRotation(quaternionRotation); 
            List<SingleGimbleRotation> incompleteGimble = new List<SingleGimbleRotation>(); 
            EulerAngleRotation incompleteEulerAngleRotation = new EulerAngleRotation(incompleteGimble); 
            
            for (int i = 0; i < gimble.Count; i++)
            {
                gimble[i].ExtractValueFromQuaternion(quaternionRotationCopy);  
                incompleteGimble.Add(gimble[i]);
                QuaternionRotation inverseRotation = incompleteEulerAngleRotation.ToQuaternionRotation().Inverse();
                if (isIntrinsic || !isIntrinsic)
                {
                    quaternionRotationCopy = inverseRotation * quaternionRotationCopy; 
                }

                if (quaternionRotationCopy == QuaternionRotation.GetIdentity())
                {
                    return; 
                }
            }
        }
        
        //ToQuaternionRotation.ToMatrixRotation is cheaper, because converting from EulerAngles to another RotationType requires combining multiple rotations throughout the process which is cheaper in Quaternions
        public override MatrixRotation ToMatrixRotation() //TODO: test this function
        {
            MatrixRotation result = new MatrixRotation(MatrixRotation.RotationIdentity());
            foreach (SingleGimbleRotation rotation in gimble)
            {
                result = result * rotation.toMatrixRotation() * result.Inverse(); 
            }
            return result; 
        }

        public void GetValuesFromMatrix(MatrixRotation matrixRotation)
        {
            MatrixRotation matrixRotationCopy = new MatrixRotation(matrixRotation); 
            List<SingleGimbleRotation> incompleteGimble = new List<SingleGimbleRotation>(); 
            EulerAngleRotation incompleteEulerAngleRotation = new EulerAngleRotation(incompleteGimble); 
            
            for (int i = 0; i < gimble.Count; i++)
            {
                gimble[i].ExtractValueFromMatrix(matrixRotationCopy); 
                incompleteGimble.Add(gimble[i]);
                MatrixRotation inverseRotation = incompleteEulerAngleRotation.ToMatrixRotation().Inverse();
                if (isIntrinsic || !isIntrinsic)
                {
                    matrixRotationCopy = inverseRotation * matrixRotationCopy; 
                }

                if (matrixRotationCopy == MatrixRotation.RotationIdentity())
                {
                    return; 
                }
            }
        }
        
        public override AxisAngleRotation ToAxisAngleRotation()
        {
            return ToQuaternionRotation().ToAxisAngleRotation(); 
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            throw new Exception("Rotating by an EulerAngle is useless, because the mathematics is the same as applying multiple MatrixRotations");
        }
    }
}
