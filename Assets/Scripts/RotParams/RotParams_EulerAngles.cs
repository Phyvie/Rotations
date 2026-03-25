using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Packages.MathExtensions;
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
    public class RotParams_EulerAngles : RotParams_Base
    { 
        #region Variables   
        [SerializeField] private _RotParams_EulerAngleGimbalRing outer; 
        [SerializeField] private _RotParams_EulerAngleGimbalRing middle; 
        [SerializeField] private _RotParams_EulerAngleGimbalRing inner;
        #endregion Variables
        
        #region Properties
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
        public EAngleType OuterAngleType
        {
            get => outer.TypedAngle.AngleType;
            set
            {
                outer.TypedAngle.AngleType = value;
                OnPropertyChanged(nameof(OuterAngleType));
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
        public EAngleType MiddleAngleType
        {
            get => middle.TypedAngle.AngleType;
            set
            {
                middle.TypedAngle.AngleType = value;
                OnPropertyChanged(nameof(MiddleAngleType));
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
        public EAngleType InnerAngleType
        {
            get => inner.TypedAngle.AngleType;
            set
            {
                inner.TypedAngle.AngleType = value;
                OnPropertyChanged(nameof(InnerAngleType));
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
        #endregion Properties
        
        #region Constructors
        public override RotParams_Base GetIdentity()
        {
            return new RotParams_EulerAngles(); 
        }
        
        public override void CopyValues(RotParams_Base toCopy)
        {
            if (toCopy is RotParams_EulerAngles rotParams)
            {
                this.Outer = new _RotParams_EulerAngleGimbalRing(rotParams.Outer); 
                this.Middle = new _RotParams_EulerAngleGimbalRing(rotParams.Middle); 
                this.Inner = new _RotParams_EulerAngleGimbalRing(rotParams.Inner); 
            }
            else
            {
                CopyValues(toCopy.ToEulerParams());
            }
        }
        
        public RotParams_EulerAngles(RotParams_EulerAngles toCopy) : 
            this(toCopy.outer, toCopy.middle, toCopy.inner, bCopyRings: true)
        {
        }
        
        public RotParams_EulerAngles() : this(
            new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Yaw, EAngleType.Degree, 0), 
            new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Pitch, EAngleType.Degree, 0), 
            new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Roll, EAngleType.Degree, 0))
        {
        }
        
        public RotParams_EulerAngles(float inYawRadian, float inPitchRadian, float inRollRadian) : 
            this(new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Yaw, inYawRadian), 
                new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Pitch, inPitchRadian), 
                new _RotParams_EulerAngleGimbalRing(EGimbalAxis.Roll, inRollRadian))
        {
        }

        public RotParams_EulerAngles(
            EGimbalAxis outerAxis, float inOuterRadian,
            EGimbalAxis middleAxis, float inMiddleRadian,
            EGimbalAxis innerAxis, float inInnerRadian) :
            this(new _RotParams_EulerAngleGimbalRing(outerAxis, inOuterRadian),
                new _RotParams_EulerAngleGimbalRing(middleAxis, inMiddleRadian),
                new _RotParams_EulerAngleGimbalRing(innerAxis, inInnerRadian))
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
        public void SwitchGimbalOrder(int firstIndex, int secondIndex) //-TODO: Test this function
        {
            (gimbal[firstIndex], gimbal[secondIndex]) = (gimbal[secondIndex], gimbal[firstIndex]); 
        }
        
        public void SwitchGimbalOrder(_RotParams_EulerAngleGimbalRing firstRing, _RotParams_EulerAngleGimbalRing secondRing) //-TODO: Test this function
        {
            (firstRing, secondRing) = (secondRing, firstRing); 
        }
        #endregion GimbalOperations

        #region Converters

        public override RotParams_Base ToSelfTypeCopy(RotParams_Base toConvert)
        {
            return toConvert.ToEulerParams(); 
        }

        public override void ToSelfType(RotParams_Base toConvert)
        {
            toConvert.ToEulerParams(this); 
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_EulerAngles ToEulerParams()
        {
            return ToEulerParams(new RotParams_EulerAngles());
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_Quaternion ToQuaternionParams()
        {
            return ToQuaternionParams(new RotParams_Quaternion());
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_Matrix ToMatrixParams()
        {
            return ToMatrixParams(new RotParams_Matrix(new float[3, 3]));
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_AxisAngle ToAxisAngleParams()
        {
            return ToAxisAngleParams(new RotParams_AxisAngle(Vector3.right, 0));
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_EulerAngles ToEulerParams(RotParams_EulerAngles eulerParams)
        {
            eulerParams.CopyValues(this);
            return eulerParams;
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_Quaternion ToQuaternionParams(RotParams_Quaternion quaternionParams)
        {
            RotParams_Quaternion result = new RotParams_Quaternion();
            foreach (_RotParams_EulerAngleGimbalRing rotation in gimbal.Reverse())
            {
                RotParams_Quaternion asQuat = rotation.toQuaternionRotation();
                result = asQuat * result; 
            }
            quaternionParams.CopyValues(result);
            return quaternionParams;
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_Matrix ToMatrixParams(RotParams_Matrix matrixParams)
        {
            RotParams_Matrix result = new RotParams_Matrix(RotParams_Matrix.RotationIdentity());
            foreach (_RotParams_EulerAngleGimbalRing rotation in gimbal)
            {
                result = result * rotation.toMatrixRotation() * result.Inverse(); 
            }
            matrixParams.CopyValues(result);
            return matrixParams;
        }

        // TodoZyKa RotParams_Conversion: Reread & Test
        public override RotParams_AxisAngle ToAxisAngleParams(RotParams_AxisAngle axisAngleParams)
        {
            ToQuaternionParams().ToAxisAngleParams(axisAngleParams);
            return axisAngleParams;
        }
        
        #region comparison
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            RotParams_EulerAngles other = (RotParams_EulerAngles)obj;
            return this == other;
        }

        //!!!ZyKa need to overwork this equality comparison, because it currently only returns true when two EulerAngles are exactly the same including order of rotation. 
        public static bool operator ==(RotParams_EulerAngles a, RotParams_EulerAngles b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (((object)a == null) || ((object)b == null)) return false;

            return a.OuterAxis == b.OuterAxis &&
                   Mathf.Abs(a.OuterAngle - b.OuterAngle) < 0.0001f &&
                   a.MiddleAxis == b.MiddleAxis &&
                   Mathf.Abs(a.MiddleAngle - b.MiddleAngle) < 0.0001f &&
                   a.InnerAxis == b.InnerAxis &&
                   Mathf.Abs(a.InnerAngle - b.InnerAngle) < 0.0001f; 
        }

        public static bool operator !=(RotParams_EulerAngles a, RotParams_EulerAngles b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (OuterAngle, MiddleAngle, InnerAngle, OuterAxis, MiddleAxis, InnerAxis).GetHashCode();
        }
        #endregion comparison

        public override RotParams_Base GetInverse()
        {
            Debug.LogWarning("Cannot get inverse of RotParams_EulerAngles, need to convert to RotParams_Matrix");
            
            RotParams_Matrix asMatrix = ToMatrixParams();

            return asMatrix.Transpose().ToEulerParams(); 
        }

        #endregion Converters

        #region operators
        public static RotParams_EulerAngles operator+(RotParams_EulerAngles a, RotParams_EulerAngles b)
        {
            if (a.OuterAxis != b.OuterAxis)
            {
                throw new Exception($"RotParamsSum not possible, because OuterAxis differs: {a.OuterAxis} != {b.OuterAxis}"); 
            }

            if (a.MiddleAxis != b.MiddleAxis)
            {
                throw new Exception($"RotParamsSum not possible, because MiddleAxis differs: {a.MiddleAxis} != {b.MiddleAxis}"); 
            }

            if (a.InnerAxis != b.InnerAxis)
            {
                throw new Exception($"RotParamsSum not possible, because InnerAxis differs: {a.InnerAxis} != {b.InnerAxis}"); 
            }
            
            return new RotParams_EulerAngles(
                a.OuterAxis, a.Outer.AngleInRadian + b.Outer.AngleInRadian, 
                a.MiddleAxis, a.Middle.AngleInRadian + b.Middle.AngleInRadian, 
                a.InnerAxis, a.Inner.AngleInRadian + b.Inner.AngleInRadian);
        }

        public static RotParams_EulerAngles operator *(float alpha, RotParams_EulerAngles a)
        {
            return a * alpha; 
        }
        
        public static RotParams_EulerAngles operator *(RotParams_EulerAngles a, float alpha)
        {
            return new RotParams_EulerAngles(
                a.OuterAxis, a.Outer.AngleInRadian * alpha, 
                a.MiddleAxis, a.Middle.AngleInRadian * alpha, 
                a.InnerAxis, a.Inner.AngleInRadian * alpha);
        }
        #endregion operators
        
        #region functions

        protected override RotParams_Base Concatenate_Implementation(RotParams_Base otherRotation, bool otherFirst = false)
        {
            Debug.LogWarning("Cannot concatenate RotParams_EulerAngles, converting to RotationMatrix");
            
            RotParams_Matrix thisAsMatrix = ToMatrixParams();
            RotParams_Matrix otherAsMatrix = otherRotation.ToMatrixParams();
            
            return thisAsMatrix.Concatenate(otherAsMatrix, otherFirst).ToEulerParams();
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

        public override void GetValuesFromUnityQuaternion(Quaternion unityQuaternion)
        {
            //TodoZyKa RotParams_EulerAngle: missing function 
            throw new NotImplementedException("GetValuesFromUnityQuaternion is not implemented");
        }

        public override string ToString()
        {
            return $"({Outer.GetRotationName()}, {Outer.TypedAngle.AngleInDegree}), " +
                   $"({Middle.GetRotationName()}, {Middle.TypedAngle.AngleInDegree}), " +
                   $"({Inner.GetRotationName()}, {Inner.TypedAngle.AngleInDegree})"; 
        }

        public static RotParams_EulerAngles Lerp(RotParams_EulerAngles a, RotParams_EulerAngles b, float alpha)
        {
            return a*(1-alpha) + b*alpha;
        }
        
        public static RotParams_EulerAngles BezierCurve(RotParams_EulerAngles rotParamsA, RotParams_EulerAngles rotParamsB,
            RotParams_EulerAngles outA, RotParams_EulerAngles inB, float alpha)
        {
            float oneMinusAlpha = 1 - alpha;
            
            return 
                1*oneMinusAlpha*oneMinusAlpha*oneMinusAlpha * rotParamsA + 
                3*oneMinusAlpha*oneMinusAlpha*alpha * outA + 
                3*oneMinusAlpha*alpha*alpha * inB + 
                1*alpha*alpha*alpha * rotParamsB
                ; 
        }
        #endregion functions
    }
}
