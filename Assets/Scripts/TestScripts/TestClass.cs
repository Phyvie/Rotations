using System;
using RotationTypes;
using UnityEngine;

namespace TestScripts
{
    [Serializable]
    public class TestClass
    {
        [SerializeField] private AngleType angleTypeA = null;
        [SerializeField] private AngleType angleTypeB = AngleType.Radian; 
        
        [SerializeField] private AngleWithType angleWithType; 
        [SerializeField] private EulerAngleRotation eulerAngleRotation; 
    }
}
