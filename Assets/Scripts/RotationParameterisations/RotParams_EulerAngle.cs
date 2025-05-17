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
    public class RotParams_EulerAngle : RotationParameterisation
    {
        public EulerAngleGimbleRing firstGimbleRing; 
        public EulerAngleGimbleRing secondGimbleRing; 
        public EulerAngleGimbleRing thirdGimbleRing;
        private EulerAngleGimbleRing[] gimble => new[] { firstGimbleRing, secondGimbleRing, thirdGimbleRing }; 
        private EulerAngleGimbleRing Yaw => GetRingForAxis(EGimbleAxis.Yaw); 
        private EulerAngleGimbleRing Pitch => GetRingForAxis(EGimbleAxis.Pitch); 
        private EulerAngleGimbleRing Roll => GetRingForAxis(EGimbleAxis.Roll); 
        
        private EulerAngleGimbleRing GetRingForAxis(EGimbleAxis eAxis)
        {
            EulerAngleGimbleRing result = null;
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
        
        public RotParams_EulerAngle() : this (0, 0, 0, AngleType.Radian)
        {
        }

        public RotParams_EulerAngle(RotParams_EulerAngle toCopy) : 
            this(toCopy.firstGimbleRing, toCopy.secondGimbleRing, toCopy.thirdGimbleRing, toCopy.AngleType, bCopyRings: true, isIntrinsic: toCopy.isIntrinsic)
        {
        }
        
        public RotParams_EulerAngle(float inYaw, float inPitch, float inRoll) : 
            this(inRoll, inYaw, inPitch, AngleType.Radian)
        {
        }
        
        public RotParams_EulerAngle(float inYaw, float inPitch, float inRoll, AngleType inAngleType) : //TODO: order of gimbleAxes
            this(new EulerAngleGimbleRing(EGimbleAxis.Yaw, inYaw, inAngleType, null), 
                new EulerAngleGimbleRing(EGimbleAxis.Pitch, inPitch, inAngleType, null), 
                new EulerAngleGimbleRing(EGimbleAxis.Roll, inRoll, inAngleType, null), 
                inAngleType)
        {
        }

        public RotParams_EulerAngle(
            float firstAngle, EGimbleAxis firstAxis, AngleType firstAngleType, 
            float secondAngle, EGimbleAxis secondAxis, AngleType secondAngleType, 
            float thirdAngle, EGimbleAxis thirdAxis, AngleType thirdAngleType, 
            AngleType inAngleType, bool bCopyRings = false, bool isIntrinsic = false, bool ringsInheritAngleType = true) : 
            this(new EulerAngleGimbleRing(firstAxis, firstAngle, firstAngleType, null), 
                new EulerAngleGimbleRing(secondAxis, secondAngle, secondAngleType, null), 
                new EulerAngleGimbleRing(thirdAxis, thirdAngle, thirdAngleType, null), 
                inAngleType, bCopyRings, isIntrinsic, ringsInheritAngleType)
        {
        }

        public RotParams_EulerAngle(EulerAngleGimbleRing firstRing, EulerAngleGimbleRing secondRing, EulerAngleGimbleRing thirdRing,
            AngleType inAngleType, bool bCopyRings = false, bool isIntrinsic = true, bool ringsInheritAngleType = true)
        {
            if (bCopyRings)
            {
                firstGimbleRing = new EulerAngleGimbleRing(firstRing); 
                secondGimbleRing = new EulerAngleGimbleRing(secondRing); 
                thirdGimbleRing = new EulerAngleGimbleRing(thirdRing); 
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
        
        public void SwitchGimbleOrder(EulerAngleGimbleRing firstRing, EulerAngleGimbleRing secondRing)
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

        public override RotParams_EulerAngle ToEulerAngleRotation()
        {
            return new RotParams_EulerAngle(this); 
        }

        public override RotParams_Quaternion ToQuaternionRotation() //TODO: adjust for IsIntrinsic
        {
            RotParams_Quaternion result = new RotParams_Quaternion();
            foreach (EulerAngleGimbleRing rotation in gimble)
            {
                result = result * rotation.toQuaternionRotation(AngleType) * result.Inverse(); 
            }
            return result; 
        }

        public void GetValuesFromQuaternion(RotParams_Quaternion rotParamsQuaternion) //TODO: test
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

            
            RotParams_Quaternion rotParamsQuaternionCopy = new RotParams_Quaternion(rotParamsQuaternion); 
            RotParams_EulerAngle incompleteRotParamsEulerAngle = new RotParams_EulerAngle(this); 
            
            for (int i = 0; i < gimble.Length; i++)
            {
                gimble[i].ExtractValueFromQuaternion(rotParamsQuaternionCopy);  
                RotParams_Quaternion inverse = incompleteRotParamsEulerAngle.ToQuaternionRotation().Inverse();
                if (isIntrinsic || !isIntrinsic)
                {
                    rotParamsQuaternionCopy = inverse * rotParamsQuaternionCopy; 
                }

                if (rotParamsQuaternionCopy == RotParams_Quaternion.GetIdentity())
                {
                    return; 
                }
            }
        }
        
        //ToQuaternionRotation.ToMatrixRotation is cheaper, because converting from EulerAngles to another RotationParent requires combining and inversing multiple rotation, which is cheaper in Quaternions
        public override RotParams_Matrix ToMatrixRotation() //TODO: test this function
        {
            RotParams_Matrix result = new RotParams_Matrix(RotParams_Matrix.RotationIdentity());
            foreach (EulerAngleGimbleRing rotation in gimble)
            {
                result = result * rotation.toMatrixRotation(AngleType) * result.Inverse(); 
            }
            return result; 
        }

        public void GetValuesFromMatrix(RotParams_Matrix rotParamsMatrix) //TODO: test this function
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
            
            RotParams_Matrix rotParamsMatrixCopy = new RotParams_Matrix(rotParamsMatrix); 
            
            for (int i = 0; i < gimble.Length; i++)
            {
                gimble[i].ExtractValueFromMatrix(rotParamsMatrixCopy); 
                RotParams_Matrix inverse = gimble[i].toMatrixRotation(AngleType).Inverse();
                if (isIntrinsic || !isIntrinsic)
                {
                    rotParamsMatrixCopy = inverse * rotParamsMatrixCopy; 
                }

                if (rotParamsMatrixCopy == RotParams_Matrix.RotationIdentity())
                {
                    return; 
                }
            }
        }
        
        public override RotParams_AxisAngle ToAxisAngleRotation()
        {
            return ToQuaternionRotation().ToAxisAngleRotation(); 
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            throw new Exception("Rotating by an EulerAngle is useless, because the mathematics is the same as applying multiple MatrixRotations");
        }
    }
}
