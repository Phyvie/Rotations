using System;
using System.Collections.Generic;
using UnityEngine;

namespace RotationTypes
{
    [Serializable]
    public class AxisAngleRotation : RotationType
    {
        [SerializeField] private Vector3 axisAngle;
        private Vector3 axis => axisAngle.normalized; 
        private float angle => axisAngle.magnitude;

        public AxisAngleRotation()
        {
            
        }
        
        public AxisAngleRotation(Vector3 inAxisAngle, AngleType inAngleType)
        {
            angleType = inAngleType; 
            axisAngle = inAxisAngle; 
        }

        public AxisAngleRotation(Vector3 inAxis, float inAngle, AngleType inAngleType)
        {
            angleType = inAngleType; 
            inAxis = inAxis.normalized;
            axisAngle = inAxis * inAngle;
        }
        
        public override  EulerAngleRotation ToEulerAngleRotation()
        {
            throw new System.NotImplementedException();
        }

        public override QuaternionRotation ToQuaternionRotation()
        {
            return new QuaternionRotation(axis, angle, angleType); 
        }

        public override MatrixRotation ToMatrixRotation()
        {
            return ToQuaternionRotation().ToMatrixRotation(); 
        }

        public override AxisAngleRotation ToAxisAngleRotation()
        {
            return new AxisAngleRotation(axisAngle, angleType); 
        }

        public override void SetAngleType(AngleType value)
        {
            axisAngle = axisAngle * (float) (value.fullCircleUnits / angleType.fullCircleUnits);
            _angleType = value; 
        }

        public override Vector3 RotateVector(Vector3 inVector)
        {
            throw new NotImplementedException();
        }
    }
}
