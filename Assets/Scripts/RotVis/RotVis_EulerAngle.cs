using System;
using BaseClasses;
using MathExtensions;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_EulerAngle : RotVis
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
        public override RotParams.RotParams GetRotParams()
        {
            return rotParams; 
        }

        public override void SetRotParamsByRef(ref RotParams.RotParams newRotParams)
        {
            if (newRotParams is RotParams_EulerAngles rotParamsEulerAngles)
            {
                rotParams = rotParamsEulerAngles;
                VisUpdate();
            }
        }
        
        public override void VisUpdate()
        {
            Debug.LogWarning("ZyKaEulerRotVisUpdate");
            if (_previousRotParamAxes == null || !RotParams_EulerAngles.AreAxesMatching(rotParams, _previousRotParamAxes))
            {
                VisReset(); 
                VisUpdateRailsForUnrotatedGimbal();
                VisUpdatePlaneArcShaderColors();
                _previousRotParamAxes = new RotParams_EulerAngles(rotParams);
            }
            VisUpdateRingRotations();
            VisUpdatePlaneArcShaders();
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
            outer.ring.transform.localRotation = new Quaternion(0, Mathf.Sin(rotParams.outer.AngleInRadian/2), 0, Mathf.Cos(rotParams.outer.AngleInRadian/2)); 
            middle.ring.transform.localRotation = new Quaternion(0, Mathf.Sin(rotParams.middle.AngleInRadian/2), 0, Mathf.Cos(rotParams.middle.AngleInRadian/2)); 
            inner.ring.transform.localRotation = new Quaternion(0, Mathf.Sin(rotParams.inner.AngleInRadian/2), 0, Mathf.Cos(rotParams.inner.AngleInRadian/2));
        }

        private void VisUpdateRailsForUnrotatedGimbal()
        {
            Vector3 outerUp = rotParams.outer.RotationAxis;
            Vector3 middleUp = rotParams.middle.RotationAxis;
            Vector3 innerUp = rotParams.inner.RotationAxis;
            Vector3 outerForward = Vector3.Dot(outerUp, middleUp) == 0 ? middleUp : outerUp.CyclicAxisRotation();
            Vector3 middleForward = Vector3.Dot(outerUp, middleUp) == 0 ? Vector3.Cross(middleUp, outerUp) : middleUp.CyclicAxisRotation();
            Vector3 innerForward = Vector3.Dot(middleUp, innerUp) == 0 ? Vector3.Cross(innerUp, middleUp) : innerUp.CyclicAxisRotation();
            
            outer.rail.transform.rotation = Quaternion.LookRotation(outerForward, outerUp); 
            middle.rail.transform.rotation = Quaternion.LookRotation(middleForward, middleUp); 
            inner.rail.transform.rotation = Quaternion.LookRotation(innerForward, innerUp);
        }

        private void VisFullUpdateRotations()
        {
            Quaternion outerRailRotation = Quaternion.LookRotation(rotParams.outer.RotationAxis, Vector3.Cross(rotParams.middle.RotationAxis, rotParams.outer.RotationAxis)); 
            outer.rail.transform.rotation = outerRailRotation;
            
            Quaternion outerRingRotation = Quaternion.AngleAxis(rotParams.outer.AngleInRadian * Mathf.Rad2Deg, rotParams.outer.RotationAxis);
            outer.ring.transform.rotation = outerRingRotation * outerRailRotation;
            
            Quaternion middleRailRotation = Quaternion.LookRotation(rotParams.middle.RotationAxis, Vector3.Cross(rotParams.outer.RotationAxis, rotParams.middle.RotationAxis)); 
            middle.rail.transform.rotation = middleRailRotation * outerRingRotation;
            
            Quaternion middleRingRotation = Quaternion.AngleAxis(rotParams.middle.AngleInRadian * Mathf.Rad2Deg, rotParams.middle.RotationAxis);
            middle.ring.transform.rotation = outerRingRotation * middleRingRotation * middleRailRotation; 
            
            Quaternion innerRailRotation = Quaternion.LookRotation(rotParams.inner.RotationAxis, Vector3.Cross(rotParams.middle.RotationAxis, rotParams.inner.RotationAxis)); 
            inner.rail.transform.rotation = outerRingRotation * middleRingRotation * innerRailRotation;
            
            Quaternion innerRingRotation = Quaternion.AngleAxis(rotParams.inner.AngleInRadian * Mathf.Rad2Deg, rotParams.inner.RotationAxis);
            inner.ring.transform.rotation = outerRingRotation * middleRingRotation * innerRingRotation * innerRailRotation; 
        }

        private void VisUpdatePlaneArcShaderColors()
        {
            VisUpdatePlaneArcShaderColourSingle(rotParams.outer, outer.vis_planeArc);
            VisUpdatePlaneArcShaderColourSingle(rotParams.middle, middle.vis_planeArc);
            VisUpdatePlaneArcShaderColourSingle(rotParams.inner, inner.vis_planeArc);
        }

        private void VisUpdatePlaneArcShaderColourSingle(_RotParams_EulerAngleGimbalRing gimbalRing, Vis_PlaneArc visPlaneArc)
        {
            switch (gimbalRing.eAxis)
            {
                case EGimbleAxis.Yaw:
                    visPlaneArc.PositiveAngleColor = PosYawColor; 
                    visPlaneArc.NegativeAngleColor = NegYawColor;
                    break;
                case EGimbleAxis.Pitch:
                    visPlaneArc.PositiveAngleColor = PosPitchColor;
                    visPlaneArc.NegativeAngleColor = NegPitchColor;
                    break;
                case EGimbleAxis.Roll:
                    visPlaneArc.PositiveAngleColor = PosRollColor;
                    visPlaneArc.NegativeAngleColor = NegRollColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void VisUpdatePlaneArcShaders()
        {
            if (outer.vis_planeArc != null)
            {
                outer.vis_planeArc.StartingAngle = -rotParams.outer.AngleInRadian;
            }

            if (middle.vis_planeArc != null)
            {
                middle.vis_planeArc.StartingAngle = -rotParams.middle.AngleInRadian;
            }
            
            if (inner.vis_planeArc != null)
            {
                inner.vis_planeArc.StartingAngle = -rotParams.inner.AngleInRadian;
            }
        }
        
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
    }
}