using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [CreateProperty]
        public EGimbalAxis ZyKaGimbalAxis; 
        
        [SerializeField] private _RotParams_EulerAngleGimbalRing outer; 
        [SerializeField] private _RotParams_EulerAngleGimbalRing middle; 
        [SerializeField] private _RotParams_EulerAngleGimbalRing inner;

        #region GettersSetters
        [CreateProperty]
        public _RotParams_EulerAngleGimbalRing Outer
        {
            get => outer;
            set
            {
                if (outer != null) { outer.PropertyChanged -= ForwardPropertyChanged; }
                outer = value;
                OnPropertyChanged(nameof(Outer));
                outer.PropertyChanged += ForwardPropertyChanged; 

                void ForwardPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    OnPropertyChanged(nameof(Outer) + "." + e.PropertyName);
                }
            }
        }

        [CreateProperty]
        public _RotParams_EulerAngleGimbalRing Middle
        {
            get => middle;
            set
            {
                if (middle != null) {middle.PropertyChanged -= ForwardPropertyChanged; }
                middle = value;
                OnPropertyChanged(nameof(Middle));
                middle.PropertyChanged += ForwardPropertyChanged; 

                void ForwardPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    OnPropertyChanged(nameof(Middle) + "." + e.PropertyName);
                }
            }
        }

        [CreateProperty]
        public _RotParams_EulerAngleGimbalRing Inner
        {
            get => inner;
            set
            {
                if(inner != null) {inner.PropertyChanged -= ForwardPropertyChanged;} 
                inner = value;
                OnPropertyChanged(nameof(Inner));
                inner.PropertyChanged += ForwardPropertyChanged; 

                void ForwardPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    OnPropertyChanged(nameof(Inner) + "." + e.PropertyName);
                }
            }
        }
        
        private _RotParams_EulerAngleGimbalRing[] gimbal => new[] { Outer, Middle, Inner }; 
        private _RotParams_EulerAngleGimbalRing Yaw => GetRingForAxis(EGimbalAxis.Yaw); 
        private _RotParams_EulerAngleGimbalRing Pitch => GetRingForAxis(EGimbalAxis.Pitch); 
        private _RotParams_EulerAngleGimbalRing Roll => GetRingForAxis(EGimbalAxis.Roll);
        [CreateProperty]
        private string GimbalType => Enum.GetName(typeof(EGimbalType), GetGimbalType()); 
        
        private _RotParams_EulerAngleGimbalRing GetRingForAxis(EGimbalAxis eAxis)
        {
            _RotParams_EulerAngleGimbalRing result = null;
            if (outer.EAxis == eAxis)
            {
                result = outer; 
            }
            if (middle.EAxis == eAxis)
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
            if (inner.EAxis == eAxis)
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

        [CreateProperty]
        public float OuterAngle
        {
            get => outer.TypedAngle.AngleInCurrentUnit;
            set
            {
                outer.TypedAngle.AngleInCurrentUnit = value;
                OnPropertyChanged(nameof(OuterAngle));
            }
        }

        [CreateProperty]
        public EGimbalAxis OuterAxis
        {
            get => outer.EAxis;
            set
            {
                outer.EAxis = value;
                OnPropertyChanged(nameof(OuterAxis));
            }
        }

        [CreateProperty]
        public float MiddleAngle
        {
            get => middle.TypedAngle.AngleInCurrentUnit;
            set
            {
                middle.TypedAngle.AngleInCurrentUnit = value;
                OnPropertyChanged(nameof(MiddleAngle));
            }
        }

        [CreateProperty]
        public EGimbalAxis MiddleAxis
        {
            get => middle.EAxis;
            set
            {
                middle.EAxis = value;
                OnPropertyChanged(nameof(MiddleAxis));
            }
        }
        
        [CreateProperty]
        public float InnerAngle
        {
            get => inner.TypedAngle.AngleInCurrentUnit;
            set
            {
                inner.TypedAngle.AngleInCurrentUnit = value;
                OnPropertyChanged(nameof(InnerAngle));
            }
        }

        [CreateProperty]
        public EGimbalAxis InnerAxis
        {
            get => inner.EAxis;
            set
            {
                inner.EAxis = value;
                OnPropertyChanged(nameof(InnerAxis));
            }
        }
        
        #endregion GettersSetters
        
        #region Constructors
        public RotParams_EulerAngles() : this(
            new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Yaw, AngleType.Degree, 0), 
            new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Pitch, AngleType.Degree, 0), 
            new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Roll, AngleType.Degree, 0))
        {
        }

        public RotParams_EulerAngles(RotParams_EulerAngles toCopy) : 
            this(toCopy.outer, toCopy.middle, toCopy.inner, bCopyRings: true)
        {
        }
        
        public RotParams_EulerAngles(float inYaw, float inPitch, float inRoll) : 
            this(new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Yaw, inYaw), 
                new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Pitch, inPitch), 
                new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Roll, inRoll))
        {
        }

        public RotParams_EulerAngles(_RotParams_EulerAngleGimbalRing firstRing, _RotParams_EulerAngleGimbalRing secondRing, _RotParams_EulerAngleGimbalRing thirdRing, bool bCopyRings = false)
        {
            if (bCopyRings)
            {
                Outer = new _RotParams_EulerAngleGimbalRing(firstRing); 
                Middle = new _RotParams_EulerAngleGimbalRing(secondRing); 
                Inner = new _RotParams_EulerAngleGimbalRing(thirdRing); 
            }
            else
            {
                Outer = firstRing;
                Middle = secondRing;
                Inner = thirdRing;
            }
        }
        #endregion //Constructors
        
        #region GimbalProperties
        public bool IsGimbalValid()
        {
            return GetGimbalType() != EGimbalType.InvalidGimbalOrder; 
        }

        public EGimbalType GetGimbalType()
        {
            HashSet<EGimbalAxis> gimbalAxisSet = new HashSet<EGimbalAxis>();
            gimbalAxisSet.Add(gimbal[0].EAxis); 
            
            if (gimbalAxisSet.Contains(gimbal[1].EAxis))
            {
                return EGimbalType.InvalidGimbalOrder; 
            }
            else
            {
                gimbalAxisSet.Add(gimbal[1].EAxis); 
            }

            if (gimbalAxisSet.Contains(gimbal[2].EAxis))
            {
                if (gimbal[2].EAxis == gimbal[0].EAxis)
                {
                    return EGimbalType.TrueEulerAngle; 
                }
                else //if (gimbal[2].EAxis == gimbal[1].EAxis)
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
            return a.outer.EAxis == b.outer.EAxis && a.middle.EAxis == b.middle.EAxis && a.inner.EAxis == b.inner.EAxis;
        }
        #endregion GimbalProperties
        
        #region GimbalOperations
        public void SwitchGimbalOrder(int firstIndex, int secondIndex)
        {
            (gimbal[firstIndex], gimbal[secondIndex]) = (gimbal[secondIndex], gimbal[firstIndex]); 
        }
        
        public void SwitchGimbalOrder(_RotParams_EulerAngleGimbalRing firstRing, _RotParams_EulerAngleGimbalRing secondRing)
        {
            (firstRing, secondRing) = (secondRing, firstRing); //TODO: test this function
        }
        #endregion GimbalOperations

        #region Converters
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
                Debug.LogError("EulerAngleRotation.GetValuesFromQuaternion() error: GimbalType is Invalid");
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
                Debug.LogError("EulerAngleRotation.GetValuesFromMatrix() error: GimbalType is Invalid");
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
        #endregion Converters

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
