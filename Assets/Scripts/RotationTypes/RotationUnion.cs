using System;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public enum ERotationType
    {
        eulerAngle, 
        quaternion, 
        axisAngle, 
        matrix
    }
    
    [Serializable]
    public struct RotationUnion
    {
        [SerializeField] public ERotationType zyKaEnum;

        public ERotationType ZyKaEnum
        {
            get => zyKaEnum;
            set
            {
                Debug.Log($"NewZyKaEnumValue = {value}");
                zyKaEnum = value;
            }
        }

        public void SetZyKaEnum(ERotationType newType)
        {
            Debug.Log($"ZyKaEnum = {newType}");
            Debug.Log($"eulerAngleRotation.firstGimbleRing.angle: {eulerAngleRotation.firstGimbleRing.angle}");
            zyKaEnum = newType; 
        }
        
        [SerializeField] private ERotationType eRotationType;
        [SerializeField] private RotationType activeRotation; 
        [SerializeField] private EulerAngleRotation eulerAngleRotation;
        [SerializeField] private QuaternionRotation quaternionRotation;
        [SerializeField] private AxisAngleRotation axisAngleRotation;
        [SerializeField] private MatrixRotation matrixRotation;

        public void SetType(ERotationType newType)
        {
            if (newType == eRotationType)
            {
                return; 
            }
            
            if (activeRotation is null)
            {
                activeRotation = newType switch
                {
                    ERotationType.eulerAngle => new EulerAngleRotation(), 
                    ERotationType.quaternion => new QuaternionRotation(), 
                    //missing AxisAngle and Matrix
                    _ => throw new NotImplementedException()
                };
            }
            else
            {
                activeRotation = newType switch
                {
                    ERotationType.eulerAngle => (eulerAngleRotation = activeRotation.ToEulerAngleRotation()), 
                    ERotationType.quaternion => (quaternionRotation = activeRotation.ToQuaternionRotation()), 
                    //missing AxisAngle and Matrix
                    _ => throw new NotImplementedException()
                }; 
            }

            eRotationType = newType; 
        }

        public dynamic GetActiveValueDynamic()
        {
            return activeRotation; 
        }

        public TRotationType GetValueOfTypeT<TRotationType>() where TRotationType : RotationType
        {
            return (TRotationType) activeRotation; 
        }
    }
}