using System;
using BaseClasses;
using MathExtensions;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_EulerAngle : RotVis_Base
    {
        private RotParams_EulerAngles _previousRotParamAxes = new RotParams_EulerAngles();

        [Serializable]
        private class RailRingPair
        {
            [SerializeField] public GameObject rail;
            [SerializeField] public GameObject ring; 
            [SerializeField] public Vis_PlaneArc vis_planeArc; 
        }
        [SerializeField] private RailRingPair outer;
        [SerializeField] private RailRingPair middle;
        [SerializeField] private RailRingPair inner;
        
        [SerializeField] private Color PosYawColor; 
        [SerializeField] private Color NegYawColor;
        
        [SerializeField] private Color PosPitchColor;
        [SerializeField] private Color NegPitchColor;
        
        [SerializeField] private Color PosRollColor;
        [SerializeField] private Color NegRollColor;
        
        public RotVis_EulerAngle(RotParams_EulerAngles rotParams) : base(rotParams)
        {
            
        }
        
        [SerializeField] private RotParams_EulerAngles rotParams;
        public override RotParams_Base GetRotParams()
        {
            return rotParams; 
        }

        public override void SetRotParamsByRef(RotParams_Base newRotParams)
        {
            base.SetRotParamsByRef(newRotParams);
            if (newRotParams is RotParams_EulerAngles rotParamsEulerAngles)
            {
                rotParams = rotParamsEulerAngles;
                VisUpdate();
            }
        }
        
        public override void VisUpdate()
        {
            if (_previousRotParamAxes == null || !RotParams_EulerAngles.AreAxesMatching(rotParams, _previousRotParamAxes))
            {
                VisReset(); 
                VisSetupRails();
                VisUpdateShaderPlaneArcsColours();
                _previousRotParamAxes = new RotParams_EulerAngles(rotParams);
            }
            VisUpdateRingRotations();
            VisUpdateShaderPlaneArcsValues();
            VisUpdateRotationObject(); 
        }

        private void VisReset()
        {
            outer.rail.transform.rotation = Quaternion.identity;
            outer.ring.transform.rotation = Quaternion.identity;
            
            middle.rail.transform.rotation = Quaternion.identity;
            middle.ring.transform.rotation = Quaternion.identity;
            
            inner.rail.transform.rotation = Quaternion.identity;
            inner.ring.transform.rotation = Quaternion.identity;
        }
        
        private void VisUpdateRingRotations()
        {
            outer.ring.transform.localRotation = new Quaternion(0, Mathf.Sin(rotParams.Outer.AngleInRadian/2), 0, Mathf.Cos(rotParams.Outer.AngleInRadian/2)); 
            middle.ring.transform.localRotation = new Quaternion(0, Mathf.Sin(rotParams.Middle.AngleInRadian/2), 0, Mathf.Cos(rotParams.Middle.AngleInRadian/2)); 
            inner.ring.transform.localRotation = new Quaternion(0, Mathf.Sin(rotParams.Inner.AngleInRadian/2), 0, Mathf.Cos(rotParams.Inner.AngleInRadian/2));
        }

        private void VisSetupRails()
        {
            Vector3 outerUp = rotParams.Outer.RotationAxis;
            Vector3 middleUp = rotParams.Middle.RotationAxis;
            Vector3 innerUp = rotParams.Inner.RotationAxis;
            Vector3 outerForward = outerUp.CyclicAxisRotation().CyclicAxisRotation(); 
            Vector3 middleForward = middleUp.CyclicAxisRotation().CyclicAxisRotation();
            Vector3 innerForward = innerUp.CyclicAxisRotation().CyclicAxisRotation();
            
            outer.rail.transform.rotation = Quaternion.LookRotation(outerForward, outerUp); 
            middle.rail.transform.rotation = Quaternion.LookRotation(middleForward, middleUp); 
            inner.rail.transform.rotation = Quaternion.LookRotation(innerForward, innerUp);
        }

        private void VisFullUpdateRotations()
        {
            Quaternion outerRailRotation = Quaternion.LookRotation(rotParams.Outer.RotationAxis, Vector3.Cross(rotParams.Middle.RotationAxis, rotParams.Outer.RotationAxis)); 
            outer.rail.transform.rotation = outerRailRotation;
            
            Quaternion outerRingRotation = Quaternion.AngleAxis(rotParams.Outer.AngleInRadian * Mathf.Rad2Deg, rotParams.Outer.RotationAxis);
            outer.ring.transform.rotation = outerRingRotation * outerRailRotation;
            
            Quaternion middleRailRotation = Quaternion.LookRotation(rotParams.Middle.RotationAxis, Vector3.Cross(rotParams.Outer.RotationAxis, rotParams.Middle.RotationAxis)); 
            middle.rail.transform.rotation = middleRailRotation * outerRingRotation;
            
            Quaternion middleRingRotation = Quaternion.AngleAxis(rotParams.Middle.AngleInRadian * Mathf.Rad2Deg, rotParams.Middle.RotationAxis);
            middle.ring.transform.rotation = outerRingRotation * middleRingRotation * middleRailRotation; 
            
            Quaternion innerRailRotation = Quaternion.LookRotation(rotParams.Inner.RotationAxis, Vector3.Cross(rotParams.Middle.RotationAxis, rotParams.Inner.RotationAxis)); 
            inner.rail.transform.rotation = outerRingRotation * middleRingRotation * innerRailRotation;
            
            Quaternion innerRingRotation = Quaternion.AngleAxis(rotParams.Inner.AngleInRadian * Mathf.Rad2Deg, rotParams.Inner.RotationAxis);
            inner.ring.transform.rotation = outerRingRotation * middleRingRotation * innerRingRotation * innerRailRotation; 
        }

        private void VisUpdateShaderPlaneArcsColours()
        {
            VisUpdatePlaneArcShaderColourSingle(rotParams.Outer, outer.vis_planeArc);
            VisUpdatePlaneArcShaderColourSingle(rotParams.Middle, middle.vis_planeArc);
            VisUpdatePlaneArcShaderColourSingle(rotParams.Inner, inner.vis_planeArc);
        }

        private void VisUpdatePlaneArcShaderColourSingle(_RotParams_EulerAngleGimbalRing gimbalRing, Vis_PlaneArc visPlaneArc)
        {
            switch (gimbalRing.EAxis)
            {
                case EGimbalAxis.Yaw:
                    visPlaneArc.PositiveAngleColor = PosYawColor; 
                    visPlaneArc.NegativeAngleColor = NegYawColor;
                    break;
                case EGimbalAxis.Pitch:
                    visPlaneArc.PositiveAngleColor = PosPitchColor;
                    visPlaneArc.NegativeAngleColor = NegPitchColor;
                    break;
                case EGimbalAxis.Roll:
                    visPlaneArc.PositiveAngleColor = PosRollColor;
                    visPlaneArc.NegativeAngleColor = NegRollColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void VisUpdateShaderPlaneArcsValues()
        {
            if (outer.vis_planeArc != null)
            {
                outer.vis_planeArc.BeginAngle = -rotParams.Outer.AngleInRadian; //The Gimbal itself gets rotated there the BeginAngle must be set to the negative of the AngleInRadian
            }

            if (middle.vis_planeArc != null)
            {
                middle.vis_planeArc.BeginAngle = -rotParams.Middle.AngleInRadian;
            }
            
            if (inner.vis_planeArc != null)
            {
                inner.vis_planeArc.BeginAngle = -rotParams.Inner.AngleInRadian;
            }
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (rotParams.GetGimbalType() == EGimbalType.InvalidGimbalOrder)
            {
                Debug.LogWarning("{gameObject.name} is set to an invalid GimbalType");
            }

            try
            {
                VisUpdate();
            }
            catch (Exception e)
            {
                Debug.Log($"{name} OnValidateError {e.Message}");
            }
        }
        #endif
    }
}