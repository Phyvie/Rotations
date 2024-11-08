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
    public class EulerAngleRotationDeprecated : RotationType
    {
        [FormerlySerializedAs("firstGimbleRing")] public GimbleRingDeprecated firstGimbleRingDeprecated; 
        [FormerlySerializedAs("secondGimbleRing")] public GimbleRingDeprecated secondGimbleRingDeprecated; 
        [FormerlySerializedAs("thirdGimbleRing")] public GimbleRingDeprecated thirdGimbleRingDeprecated;
        private GimbleRingDeprecated[] gimble; 
        private GimbleRingDeprecated Yaw => GetRingForAxis(EGimbleAxis.Yaw); 
        private GimbleRingDeprecated Pitch => GetRingForAxis(EGimbleAxis.Pitch); 
        private GimbleRingDeprecated Roll => GetRingForAxis(EGimbleAxis.Roll); 
        
        private GimbleRingDeprecated GetRingForAxis(EGimbleAxis eAxis)
        {
            GimbleRingDeprecated result = null;
            if (firstGimbleRingDeprecated.eAxis == eAxis)
            {
                result = firstGimbleRingDeprecated; 
            }
            if (secondGimbleRingDeprecated.eAxis == eAxis)
            {
                if (result is not null)
                {
                    Debug.LogWarning($"EulerAngleRotationDeprecated has multiple axis of type: {eAxis.ToString()}; returning the first rotation of given axis"); 
                }
                else
                {
                    result = secondGimbleRingDeprecated; 
                }
            }
            if (thirdGimbleRingDeprecated.eAxis == eAxis)
            {
                if (result is not null)
                {
                    Debug.LogWarning($"EulerAngleRotationDeprecated has multiple axis of type: {eAxis.ToString()}; returning the first rotation of given axis"); 
                }
                else
                {
                    result = thirdGimbleRingDeprecated; 
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

        [SerializeField] private bool gimbleRingsInheritAngleType;

        public bool GimbleRingsInheritAngleType
        {
            get => gimbleRingsInheritAngleType;
            set => gimbleRingsInheritAngleType = value;
        }

        [SerializeField] protected AngleType _angleType = AngleType.Radian;

        public AngleType angleType
        {
            get => _angleType;
            set => SetAngleType(value);
        }
        
        public EulerAngleRotationDeprecated() : this (0, 0, 0, AngleType.Radian)
        {
        }

        public EulerAngleRotationDeprecated(EulerAngleRotationDeprecated toCopy) : 
            this(toCopy.firstGimbleRingDeprecated, toCopy.secondGimbleRingDeprecated, toCopy.thirdGimbleRingDeprecated, toCopy.angleType, bCopyRings: true, isIntrinsic: toCopy.isIntrinsic, ringsInheritAngleType: toCopy.gimbleRingsInheritAngleType)
        {
        }
        
        public EulerAngleRotationDeprecated(float inYaw, float inPitch, float inRoll) : 
            this(inRoll, inYaw, inPitch, AngleType.Radian)
        {
        }
        
        public EulerAngleRotationDeprecated(float inYaw, float inPitch, float inRoll, AngleType inAngleType) : //TODO: order of gimbleAxes
            this(new GimbleRingDeprecated(EGimbleAxis.Yaw, inYaw, inAngleType, null), 
                new GimbleRingDeprecated(EGimbleAxis.Pitch, inPitch, inAngleType, null), 
                new GimbleRingDeprecated(EGimbleAxis.Roll, inRoll, inAngleType, null), 
                inAngleType)
        {
        }

        public EulerAngleRotationDeprecated(
            float firstAngle, EGimbleAxis firstAxis, AngleType firstAngleType, 
            float secondAngle, EGimbleAxis secondAxis, AngleType secondAngleType, 
            float thirdAngle, EGimbleAxis thirdAxis, AngleType thirdAngleType, 
            AngleType inAngleType, bool bCopyRings = false, bool isIntrinsic = false, bool ringsInheritAngleType = true) : 
            this(new GimbleRingDeprecated(firstAxis, firstAngle, firstAngleType, null), 
                new GimbleRingDeprecated(secondAxis, secondAngle, secondAngleType, null), 
                new GimbleRingDeprecated(thirdAxis, thirdAngle, thirdAngleType, null), 
                inAngleType, bCopyRings, isIntrinsic, ringsInheritAngleType)
        {
        }

        public EulerAngleRotationDeprecated(GimbleRingDeprecated firstRingDeprecated, GimbleRingDeprecated secondRingDeprecated, GimbleRingDeprecated thirdRingDeprecated,
            AngleType inAngleType, bool bCopyRings = false, bool isIntrinsic = true, bool ringsInheritAngleType = true)
        {
            if (bCopyRings)
            {
                firstGimbleRingDeprecated = new GimbleRingDeprecated(firstRingDeprecated); 
                secondGimbleRingDeprecated = new GimbleRingDeprecated(secondRingDeprecated); 
                thirdGimbleRingDeprecated = new GimbleRingDeprecated(thirdRingDeprecated); 
            }
            else
            {
                firstGimbleRingDeprecated = firstRingDeprecated;
                secondGimbleRingDeprecated = secondRingDeprecated;
                thirdGimbleRingDeprecated = thirdRingDeprecated;
            }
            firstGimbleRingDeprecated.parentEulerAngle = this; 
            secondGimbleRingDeprecated.parentEulerAngle = this; 
            thirdGimbleRingDeprecated.parentEulerAngle = this; 
            
            if (angleType is null && inAngleType is null)
            {
                Debug.LogError("EulerAngleConstructor: _angleType is null, setting _angleType to Radian");
                angleType = AngleType.Radian;
            }
            else
            {
                angleType = inAngleType;
            }

            gimbleRingsInheritAngleType = ringsInheritAngleType;
            this.isIntrinsic = isIntrinsic;
            gimble = new GimbleRingDeprecated[]
            {
                firstGimbleRingDeprecated, secondGimbleRingDeprecated, thirdGimbleRingDeprecated
            }; 
        }
        
        public void SetAngleType(AngleType value)
        {
            if (value is null)
            {
                Debug.LogError("EulerAngleRotationDeprecated.SetAngleType error: value is null"); 
                return; 
            }
            
            if (gimbleRingsInheritAngleType)
            {
                foreach (GimbleRingDeprecated gimbleRing in gimble)
                {
                    gimbleRing.angleType = value; 
                }
            }
            _angleType = value; 
        }

        public void SwitchGimbleOrder(int firstIndex, int secondIndex)
        {
            (gimble[firstIndex], gimble[secondIndex]) = (gimble[secondIndex], gimble[firstIndex]); 
        }
        
        public void SwitchGimbleOrder(GimbleRingDeprecated firstRingDeprecated, GimbleRingDeprecated secondRingDeprecated)
        {
            (firstRingDeprecated, secondRingDeprecated) = (secondRingDeprecated, firstRingDeprecated); //TODO: test this function
        }

        public bool IsGimbleValid()
        {
            return GetGimbleType() != EGimbleType.Invalid; 
        }

        public EGimbleType GetGimbleType()
        {
            HashSet<EGimbleAxis> gimbleAxisSet = new HashSet<EGimbleAxis>();

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

        public override EulerAngleRotationDeprecated ToEulerAngleRotation()
        {
            return new EulerAngleRotationDeprecated(this); 
        }

        public override QuaternionRotation ToQuaternionRotation() //TODO: adjust for IsIntrinsic
        {
            QuaternionRotation result = new QuaternionRotation();
            foreach (GimbleRingDeprecated rotation in gimble)
            {
                result = result * rotation.toQuaternionRotation() * result.Inverse(); 
            }
            return result; 
        }

        public void GetValuesFromQuaternion(QuaternionRotation quaternionRotation) //TODO: test
        {
            QuaternionRotation quaternionRotationCopy = new QuaternionRotation(quaternionRotation); 
            EulerAngleRotationDeprecated incompleteEulerAngleRotationDeprecated = new EulerAngleRotationDeprecated(this); 
            
            for (int i = 0; i < gimble.Length; i++)
            {
                gimble[i].ExtractValueFromQuaternion(quaternionRotationCopy);  
                QuaternionRotation inverseRotation = incompleteEulerAngleRotationDeprecated.ToQuaternionRotation().Inverse();
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
            foreach (GimbleRingDeprecated rotation in gimble)
            {
                result = result * rotation.toMatrixRotation() * result.Inverse(); 
            }
            return result; 
        }

        public void GetValuesFromMatrix(MatrixRotation matrixRotation) //TODO: test this function
        {
            MatrixRotation matrixRotationCopy = new MatrixRotation(matrixRotation); 
            
            for (int i = 0; i < gimble.Length; i++)
            {
                gimble[i].ExtractValueFromMatrix(matrixRotationCopy); 
                MatrixRotation inverseRotation = gimble[i].toMatrixRotation().Inverse();
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
