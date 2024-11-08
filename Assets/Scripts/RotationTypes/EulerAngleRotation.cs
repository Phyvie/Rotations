using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotationTypes
{
    public enum EGimbleType
    {
        Invalid, 
        TrueEulerAngle, 
        TaitBryanAngle, 
    }
    
    [Serializable]
    public class EulerAngleRotation : RotationType
    {
        public GimbleRing firstGimbleRing; 
        public GimbleRing secondGimbleRing; 
        public GimbleRing thirdGimbleRing;
        private GimbleRing[] gimble; 
        private GimbleRing Yaw => GetRingForAxis(EGimbleAxis.Yaw); 
        private GimbleRing Pitch => GetRingForAxis(EGimbleAxis.Pitch); 
        private GimbleRing Roll => GetRingForAxis(EGimbleAxis.Roll); 
        
        private GimbleRing GetRingForAxis(EGimbleAxis eAxis)
        {
            GimbleRing result = null;
            if (firstGimbleRing.eAxis == eAxis)
            {
                result = firstGimbleRing; 
            }
            if (secondGimbleRing.eAxis == eAxis)
            {
                if (result is not null)
                {
                    Debug.LogWarning($"EulerAngleRotation has multiple axis of type: {eAxis.ToString()}; returning the first rotation of given axis"); 
                }
                else
                {
                    result = secondGimbleRing; 
                }
            }
            if (thirdGimbleRing.eAxis == eAxis)
            {
                if (result is not null)
                {
                    Debug.LogWarning($"EulerAngleRotation has multiple axis of type: {eAxis.ToString()}; returning the first rotation of given axis"); 
                }
                else
                {
                    result = thirdGimbleRing; 
                }
            }

            return result; 
        }
        
        [SerializeField] private bool isIntrinsic = true;
        
        public bool IsIntrinsic
        {
            get => isIntrinsic;
            set => isIntrinsic = value;
        }

        [SerializeField] protected AngleType angleType = AngleType.Radian;

        public AngleType AngleType
        {
            get => angleType;
            set => SetAngleType(value);
        }
        
        public EulerAngleRotation() : this (0, 0, 0, AngleType.Radian)
        {
        }

        public EulerAngleRotation(EulerAngleRotation toCopy) : 
            this(toCopy.firstGimbleRing, toCopy.secondGimbleRing, toCopy.thirdGimbleRing, toCopy.AngleType, bCopyRings: true, isIntrinsic: toCopy.isIntrinsic)
        {
        }
        
        public EulerAngleRotation(float inYaw, float inPitch, float inRoll) : 
            this(inRoll, inYaw, inPitch, AngleType.Radian)
        {
        }
        
        public EulerAngleRotation(float inYaw, float inPitch, float inRoll, AngleType inAngleType) : //TODO: order of gimbleAxes
            this(new GimbleRing(EGimbleAxis.Yaw, inYaw, inAngleType, null), 
                new GimbleRing(EGimbleAxis.Pitch, inPitch, inAngleType, null), 
                new GimbleRing(EGimbleAxis.Roll, inRoll, inAngleType, null), 
                inAngleType)
        {
        }

        public EulerAngleRotation(
            float firstAngle, EGimbleAxis firstAxis, AngleType firstAngleType, 
            float secondAngle, EGimbleAxis secondAxis, AngleType secondAngleType, 
            float thirdAngle, EGimbleAxis thirdAxis, AngleType thirdAngleType, 
            AngleType inAngleType, bool bCopyRings = false, bool isIntrinsic = false, bool ringsInheritAngleType = true) : 
            this(new GimbleRing(firstAxis, firstAngle, firstAngleType, null), 
                new GimbleRing(secondAxis, secondAngle, secondAngleType, null), 
                new GimbleRing(thirdAxis, thirdAngle, thirdAngleType, null), 
                inAngleType, bCopyRings, isIntrinsic, ringsInheritAngleType)
        {
        }

        public EulerAngleRotation(GimbleRing firstRing, GimbleRing secondRing, GimbleRing thirdRing,
            AngleType inAngleType, bool bCopyRings = false, bool isIntrinsic = true, bool ringsInheritAngleType = true)
        {
            if (bCopyRings)
            {
                firstGimbleRing = new GimbleRing(firstRing); 
                secondGimbleRing = new GimbleRing(secondRing); 
                thirdGimbleRing = new GimbleRing(thirdRing); 
            }
            else
            {
                firstGimbleRing = firstRing;
                secondGimbleRing = secondRing;
                thirdGimbleRing = thirdRing;
            }
            
            if (AngleType is null && inAngleType is null)
            {
                Debug.LogError("EulerAngleConstructor: angleType is null, setting angleType to Radian");
                AngleType = AngleType.Radian;
            }
            else
            {
                AngleType = inAngleType;
            }

            this.isIntrinsic = isIntrinsic;
            gimble = new GimbleRing[]
            {
                firstGimbleRing, secondGimbleRing, thirdGimbleRing
            }; 
        }
        
        public void SetAngleType(AngleType value)
        {
            if (value is null)
            {
                Debug.LogError("EulerAngleRotation.SetAngleType error: value is null"); 
                return; 
            }
            
            angleType = value; 
        }

        public void SwitchGimbleOrder(int firstIndex, int secondIndex)
        {
            (gimble[firstIndex], gimble[secondIndex]) = (gimble[secondIndex], gimble[firstIndex]); 
        }
        
        public void SwitchGimbleOrder(GimbleRing firstRing, GimbleRing secondRing)
        {
            (firstRing, secondRing) = (secondRing, firstRing); //TODO: test this function
        }

        public bool IsGimbleValid()
        {
            return GetGimbleType() != EGimbleType.Invalid; 
        }

        public EGimbleType GetGimbleType()
        {
            HashSet<EGimbleAxis> gimbleAxisSet = new HashSet<EGimbleAxis>();
            gimbleAxisSet.Add(gimble[0].eAxis); 
            
            if (gimbleAxisSet.Contains(gimble[1].eAxis))
            {
                Debug.Log("Gimble Invalid because: FirstGimbleRing.eAxis == SecondGimbleRing.eAxis");
                return EGimbleType.Invalid; 
            }
            else
            {
                gimbleAxisSet.Add(gimble[1].eAxis); 
            }

            if (gimbleAxisSet.Contains(gimble[2].eAxis))
            {
                if (gimble[2].eAxis == gimble[0].eAxis)
                {
                    return EGimbleType.TrueEulerAngle; 
                }
                else //if (gimble[2].eAxis == gimble[1].eAxis)
                {
                    Debug.Log("Gimble Invalid because: SecondGimbleRings.eAxis == ThirdGimbleRing.eAxis"); 
                    return EGimbleType.Invalid; 
                }
            }
            else
            {
                return EGimbleType.TaitBryanAngle; 
            }
        }

        public override EulerAngleRotation ToEulerAngleRotation()
        {
            return new EulerAngleRotation(this); 
        }

        public override QuaternionRotation ToQuaternionRotation() //TODO: adjust for IsIntrinsic
        {
            QuaternionRotation result = new QuaternionRotation();
            foreach (GimbleRing rotation in gimble)
            {
                result = result * rotation.toQuaternionRotation(AngleType) * result.Inverse(); 
            }
            return result; 
        }

        public void GetValuesFromQuaternion(QuaternionRotation quaternionRotation) //TODO: test
        {
            if (GetGimbleType() == EGimbleType.Invalid)
            {
                Debug.LogError("EulerAngleRotation.GetValuesFromQuaternion() error: GimbleType is Invalid");
                return; 
            }

            if (GetGimbleType() == EGimbleType.TrueEulerAngle)
            {
                Debug.LogError("EulerAngleRotation.GetValuesFromQuaternion() error: Conversion from Quaternion to TrueEulerAngles not implemented");
                return; 
            }

            
            QuaternionRotation quaternionRotationCopy = new QuaternionRotation(quaternionRotation); 
            EulerAngleRotation incompleteEulerAngleRotation = new EulerAngleRotation(this); 
            
            for (int i = 0; i < gimble.Length; i++)
            {
                gimble[i].ExtractValueFromQuaternion(quaternionRotationCopy);  
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
        
        //ToQuaternionRotation.ToMatrixRotation is cheaper, because converting from EulerAngles to another RotationType requires combining and inversing multiple rotation, which is cheaper in Quaternions
        public override MatrixRotation ToMatrixRotation() //TODO: test this function
        {
            MatrixRotation result = new MatrixRotation(MatrixRotation.RotationIdentity());
            foreach (GimbleRing rotation in gimble)
            {
                result = result * rotation.toMatrixRotation(AngleType) * result.Inverse(); 
            }
            return result; 
        }

        public void GetValuesFromMatrix(MatrixRotation matrixRotation) //TODO: test this function
        {
            if (GetGimbleType() == EGimbleType.Invalid)
            {
                Debug.LogError("EulerAngleRotation.GetValuesFromMatrix() error: GimbleType is Invalid");
                return; 
            }

            if (GetGimbleType() == EGimbleType.TrueEulerAngle)
            {
                Debug.LogError("EulerAngleRotation.GetValuesFromMatrix() error: Conversion from Matrix to TrueEulerAngles not implemented");
                return; 
            }
            
            MatrixRotation matrixRotationCopy = new MatrixRotation(matrixRotation); 
            
            for (int i = 0; i < gimble.Length; i++)
            {
                gimble[i].ExtractValueFromMatrix(matrixRotationCopy); 
                MatrixRotation inverseRotation = gimble[i].toMatrixRotation(AngleType).Inverse();
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
