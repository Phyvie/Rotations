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
    public class RotParams_EulerAngles : RotationParameterisation
    {
        public _RotParams_EulerAngleGimbleRing outer; 
        public _RotParams_EulerAngleGimbleRing middle; 
        public _RotParams_EulerAngleGimbleRing inner;
        private _RotParams_EulerAngleGimbleRing[] gimbal => new[] { outer, middle, inner }; 
        private _RotParams_EulerAngleGimbleRing Yaw => GetRingForAxis(EGimbleAxis.Yaw); 
        private _RotParams_EulerAngleGimbleRing Pitch => GetRingForAxis(EGimbleAxis.Pitch); 
        private _RotParams_EulerAngleGimbleRing Roll => GetRingForAxis(EGimbleAxis.Roll); 
        
        private _RotParams_EulerAngleGimbleRing GetRingForAxis(EGimbleAxis eAxis)
        {
            _RotParams_EulerAngleGimbleRing result = null;
            if (outer.eAxis == eAxis)
            {
                result = outer; 
            }
            if (middle.eAxis == eAxis)
            {
                if (result is not null)
                {
                    Debug.LogWarning($"EulerAngleRotation has multiple axis of type: {eAxis.ToString()}; returning the first rotation of given axis"); 
                }
                else
                {
                    result = middle; 
                }
            }
            if (inner.eAxis == eAxis)
            {
                if (result is not null)
                {
                    Debug.LogWarning($"EulerAngleRotation has multiple axis of type: {eAxis.ToString()}; returning the first rotation of given axis"); 
                }
                else
                {
                    result = inner; 
                }
            }

            return result; 
        }
        
        #region Constructors
        public RotParams_EulerAngles() : this (0, 0, 0)
        {
        }

        public RotParams_EulerAngles(RotParams_EulerAngles toCopy) : 
            this(toCopy.outer, toCopy.middle, toCopy.inner, bCopyRings: true)
        {
        }
        
        public RotParams_EulerAngles(float inYaw, float inPitch, float inRoll) : 
            this(new _RotParams_EulerAngleGimbleRing(EGimbleAxis.Yaw, inYaw), 
                new _RotParams_EulerAngleGimbleRing(EGimbleAxis.Pitch, inPitch), 
                new _RotParams_EulerAngleGimbleRing(EGimbleAxis.Roll, inRoll))
        {
        }
        
        public RotParams_EulerAngles(
            float firstAngle, EGimbleAxis firstAxis,  
            float secondAngle, EGimbleAxis secondAxis,  
            float thirdAngle, EGimbleAxis thirdAxis,  
            bool bCopyRings = false) : 
            this(new _RotParams_EulerAngleGimbleRing(firstAxis, firstAngle), 
                new _RotParams_EulerAngleGimbleRing(secondAxis, secondAngle), 
                new _RotParams_EulerAngleGimbleRing(thirdAxis, thirdAngle), 
                bCopyRings)
        {
        }

        public RotParams_EulerAngles(_RotParams_EulerAngleGimbleRing firstRing, _RotParams_EulerAngleGimbleRing secondRing, _RotParams_EulerAngleGimbleRing thirdRing, bool bCopyRings = false)
        {
            if (bCopyRings)
            {
                outer = new _RotParams_EulerAngleGimbleRing(firstRing); 
                middle = new _RotParams_EulerAngleGimbleRing(secondRing); 
                inner = new _RotParams_EulerAngleGimbleRing(thirdRing); 
            }
            else
            {
                outer = firstRing;
                middle = secondRing;
                inner = thirdRing;
            }
        }
        #endregion //Constructors
        
        public void SwitchGimbleOrder(int firstIndex, int secondIndex)
        {
            (gimbal[firstIndex], gimbal[secondIndex]) = (gimbal[secondIndex], gimbal[firstIndex]); 
        }
        
        public void SwitchGimbleOrder(_RotParams_EulerAngleGimbleRing firstRing, _RotParams_EulerAngleGimbleRing secondRing)
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
            gimbleAxisSet.Add(gimbal[0].eAxis); 
            
            if (gimbleAxisSet.Contains(gimbal[1].eAxis))
            {
                return EGimbleType.Invalid; 
            }
            else
            {
                gimbleAxisSet.Add(gimbal[1].eAxis); 
            }

            if (gimbleAxisSet.Contains(gimbal[2].eAxis))
            {
                if (gimbal[2].eAxis == gimbal[0].eAxis)
                {
                    return EGimbleType.TrueEulerAngle; 
                }
                else //if (gimble[2].eAxis == gimble[1].eAxis)
                {
                    return EGimbleType.Invalid; 
                }
            }
            else
            {
                return EGimbleType.TaitBryanAngle; 
            }
        }

        public override RotParams_EulerAngles ToEulerAngleRotation()
        {
            return new RotParams_EulerAngles(this); 
        }

        public override RotParams_Quaternion ToQuaternionRotation() //TODO: adjust for IsIntrinsic
        {
            RotParams_Quaternion result = new RotParams_Quaternion();
            foreach (_RotParams_EulerAngleGimbleRing rotation in gimbal)
            {
                result = result * rotation.toQuaternionRotation() * result.Inverse(); 
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
            RotParams_EulerAngles incompleteRotParamsEulerAngles = new RotParams_EulerAngles(this); 
            
            for (int i = 0; i < gimbal.Length; i++)
            {
                gimbal[i].ExtractValueFromQuaternion(rotParamsQuaternionCopy);  
                RotParams_Quaternion inverse = incompleteRotParamsEulerAngles.ToQuaternionRotation().Inverse();
                
                rotParamsQuaternionCopy = inverse * rotParamsQuaternionCopy; 

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
            foreach (_RotParams_EulerAngleGimbleRing rotation in gimbal)
            {
                result = result * rotation.toMatrixRotation() * result.Inverse(); 
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
            
            for (int i = 0; i < gimbal.Length; i++)
            {
                gimbal[i].ExtractValueFromMatrix(rotParamsMatrixCopy); 
                RotParams_Matrix inverse = gimbal[i].toMatrixRotation().Inverse();
                
                rotParamsMatrixCopy = inverse * rotParamsMatrixCopy; 

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
