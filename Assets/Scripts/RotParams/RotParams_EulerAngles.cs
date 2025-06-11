using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotParams
{
    public enum EGimbalType
    {
        InvalidGimbalOrder, 
        TrueEulerAngle, 
        TaitBryanAngle, 
    }
    
    [Serializable]
    public class RotParams_EulerAngles : RotParams
    {
        public _RotParams_EulerAngleGimbalRing outer = new _RotParams_EulerAngleGimbalRing(EGimbleAxis.Yaw, 0); 
        public _RotParams_EulerAngleGimbalRing middle = new _RotParams_EulerAngleGimbalRing(EGimbleAxis.Pitch, 0); 
        public _RotParams_EulerAngleGimbalRing inner = new _RotParams_EulerAngleGimbalRing(EGimbleAxis.Roll, 0); 
        private _RotParams_EulerAngleGimbalRing[] gimbal => new[] { outer, middle, inner }; 
        private _RotParams_EulerAngleGimbalRing Yaw => GetRingForAxis(EGimbleAxis.Yaw); 
        private _RotParams_EulerAngleGimbalRing Pitch => GetRingForAxis(EGimbleAxis.Pitch); 
        private _RotParams_EulerAngleGimbalRing Roll => GetRingForAxis(EGimbleAxis.Roll);
        [CreateProperty]
        private string GimbalType => Enum.GetName(typeof(EGimbalType), GetGimbalType()); 
        
        private _RotParams_EulerAngleGimbalRing GetRingForAxis(EGimbleAxis eAxis)
        {
            _RotParams_EulerAngleGimbalRing result = null;
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
        public RotParams_EulerAngles()
        {
        }

        public RotParams_EulerAngles(RotParams_EulerAngles toCopy) : 
            this(toCopy.outer, toCopy.middle, toCopy.inner, bCopyRings: true)
        {
        }
        
        public RotParams_EulerAngles(float inYaw, float inPitch, float inRoll) : 
            this(new _RotParams_EulerAngleGimbalRing(EGimbleAxis.Yaw, inYaw), 
                new _RotParams_EulerAngleGimbalRing(EGimbleAxis.Pitch, inPitch), 
                new _RotParams_EulerAngleGimbalRing(EGimbleAxis.Roll, inRoll))
        {
        }
        
        public RotParams_EulerAngles(
            float firstAngle, EGimbleAxis firstAxis,  
            float secondAngle, EGimbleAxis secondAxis,  
            float thirdAngle, EGimbleAxis thirdAxis,  
            bool bCopyRings = false) : 
            this(new _RotParams_EulerAngleGimbalRing(firstAxis, firstAngle), 
                new _RotParams_EulerAngleGimbalRing(secondAxis, secondAngle), 
                new _RotParams_EulerAngleGimbalRing(thirdAxis, thirdAngle), 
                bCopyRings)
        {
        }

        public RotParams_EulerAngles(_RotParams_EulerAngleGimbalRing firstRing, _RotParams_EulerAngleGimbalRing secondRing, _RotParams_EulerAngleGimbalRing thirdRing, bool bCopyRings = false)
        {
            if (bCopyRings)
            {
                outer = new _RotParams_EulerAngleGimbalRing(firstRing); 
                middle = new _RotParams_EulerAngleGimbalRing(secondRing); 
                inner = new _RotParams_EulerAngleGimbalRing(thirdRing); 
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
        
        public void SwitchGimbleOrder(_RotParams_EulerAngleGimbalRing firstRing, _RotParams_EulerAngleGimbalRing secondRing)
        {
            (firstRing, secondRing) = (secondRing, firstRing); //TODO: test this function
        }

        public bool IsGimbleValid()
        {
            return GetGimbalType() != EGimbalType.InvalidGimbalOrder; 
        }

        public EGimbalType GetGimbalType()
        {
            HashSet<EGimbleAxis> gimbalAxisSet = new HashSet<EGimbleAxis>();
            gimbalAxisSet.Add(gimbal[0].eAxis); 
            
            if (gimbalAxisSet.Contains(gimbal[1].eAxis))
            {
                return EGimbalType.InvalidGimbalOrder; 
            }
            else
            {
                gimbalAxisSet.Add(gimbal[1].eAxis); 
            }

            if (gimbalAxisSet.Contains(gimbal[2].eAxis))
            {
                if (gimbal[2].eAxis == gimbal[0].eAxis)
                {
                    return EGimbalType.TrueEulerAngle; 
                }
                else //if (gimbal[2].eAxis == gimbal[1].eAxis)
                {
                    return EGimbalType.InvalidGimbalOrder; 
                }
            }
            else
            {
                return EGimbalType.TaitBryanAngle; 
            }
        }

        public static bool AreAxesMatching(RotParams_EulerAngles a, RotParams_EulerAngles b)
        {
            return a.outer.eAxis == b.outer.eAxis && a.middle.eAxis == b.middle.eAxis && a.inner.eAxis == b.inner.eAxis;
        }

        public override RotParams_EulerAngles ToEulerAngleRotation()
        {
            return new RotParams_EulerAngles(this); 
        }

        public override RotParams_Quaternion ToQuaternionRotation()
        {
            RotParams_Quaternion result = new RotParams_Quaternion();
            foreach (_RotParams_EulerAngleGimbalRing rotation in gimbal.Reverse())
            {
                RotParams_Quaternion asQuat = rotation.toQuaternionRotation();
                result = asQuat * result; 
            }
            return result; 
        }

        public void GetValuesFromQuaternion(RotParams_Quaternion rotParamsQuaternion) //TODO: test
        {
            if (GetGimbalType() == EGimbalType.InvalidGimbalOrder)
            {
                Debug.LogError("EulerAngleRotation.GetValuesFromQuaternion() error: GimbleType is Invalid");
                return; 
            }

            if (GetGimbalType() == EGimbalType.TrueEulerAngle)
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
            foreach (_RotParams_EulerAngleGimbalRing rotation in gimbal)
            {
                result = result * rotation.toMatrixRotation() * result.Inverse(); 
            }
            return result; 
        }

        public void GetValuesFromMatrix(RotParams_Matrix rotParamsMatrix) //TODO: test this function
        {
            if (GetGimbalType() == EGimbalType.InvalidGimbalOrder)
            {
                Debug.LogError("EulerAngleRotation.GetValuesFromMatrix() error: GimbleType is Invalid");
                return; 
            }

            if (GetGimbalType() == EGimbalType.TrueEulerAngle)
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

        public override void ResetToIdentity()
        {
            outer.AngleInRadian = 0; 
            middle.AngleInRadian = 0;
            inner.AngleInRadian = 0;
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            throw new Exception("Rotating by an EulerAngle is useless, because the mathematics is the same as applying multiple MatrixRotations");
        }
    }
}
