using System;
using MathExtensions;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_EulerAngle : MonoBehaviour
    {
        [SerializeField] private RotParams_EulerAngles _rotParams;
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

        [SerializeField] private GameObject rotObj;

        [SerializeField] private Color PosYawColor; 
        [SerializeField] private Color NegYawColor;
        
        [SerializeField] private Color PosPitchColor;
        [SerializeField] private Color NegPitchColor;
        
        [SerializeField] private Color PosRollColor;
        [SerializeField] private Color NegRollColor;
        
        public void VisUpdate()
        {
            if (!RotParams_EulerAngles.AreAxesMatching(_rotParams, _previousRotParamAxes))
            {
                VisReset(); 
                VisUpdateRailsForUnrotatedGimbal();
                VisUpdateRotObjReset(); 
                VisUpdatePlaneArcShaderColors();
                _previousRotParamAxes = new RotParams_EulerAngles(_rotParams);
            }
            VisUpdateRingRotations();
            VisUpdatePlaneArcShaders(); 
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
            outer.ring.transform.localRotation = new Quaternion(0, Mathf.Sin(_rotParams.outer.Angle/2), 0, Mathf.Cos(_rotParams.outer.Angle/2)); 
            middle.ring.transform.localRotation = new Quaternion(0, Mathf.Sin(_rotParams.middle.Angle/2), 0, Mathf.Cos(_rotParams.middle.Angle/2)); 
            inner.ring.transform.localRotation = new Quaternion(0, Mathf.Sin(_rotParams.inner.Angle/2), 0, Mathf.Cos(_rotParams.inner.Angle/2));
        }

        private void VisUpdateRailsForUnrotatedGimbal()
        {
            Vector3 outerUp = _rotParams.outer.RotationAxis;
            Vector3 middleUp = _rotParams.middle.RotationAxis;
            Vector3 innerUp = _rotParams.inner.RotationAxis;
            Vector3 outerForward = Vector3.Dot(outerUp, middleUp) == 0 ? middleUp : outerUp.CyclicAxisRotation();
            Vector3 middleForward = Vector3.Dot(outerUp, middleUp) == 0 ? Vector3.Cross(middleUp, outerUp) : middleUp.CyclicAxisRotation();
            Vector3 innerForward = Vector3.Dot(middleUp, innerUp) == 0 ? Vector3.Cross(innerUp, middleUp) : innerUp.CyclicAxisRotation();
            
            outer.rail.transform.rotation = Quaternion.LookRotation(outerForward, outerUp); 
            middle.rail.transform.rotation = Quaternion.LookRotation(middleForward, middleUp); 
            inner.rail.transform.rotation = Quaternion.LookRotation(innerForward, innerUp);
        }

        private void VisUpdateRotObjReset()
        {
            if (rotObj != null)
            {
                rotObj.transform.rotation = Quaternion.identity; 
            }
        }

        private void VisFullUpdateRotations()
        {
            Quaternion outerRailRotation = Quaternion.LookRotation(_rotParams.outer.RotationAxis, Vector3.Cross(_rotParams.middle.RotationAxis, _rotParams.outer.RotationAxis)); 
            outer.rail.transform.rotation = outerRailRotation;
            
            Quaternion outerRingRotation = Quaternion.AngleAxis(_rotParams.outer.Angle * Mathf.Rad2Deg, _rotParams.outer.RotationAxis);
            outer.ring.transform.rotation = outerRingRotation * outerRailRotation;
            
            Quaternion middleRailRotation = Quaternion.LookRotation(_rotParams.middle.RotationAxis, Vector3.Cross(_rotParams.outer.RotationAxis, _rotParams.middle.RotationAxis)); 
            middle.rail.transform.rotation = middleRailRotation * outerRingRotation;
            
            Quaternion middleRingRotation = Quaternion.AngleAxis(_rotParams.middle.Angle * Mathf.Rad2Deg, _rotParams.middle.RotationAxis);
            middle.ring.transform.rotation = outerRingRotation * middleRingRotation * middleRailRotation; 
            
            Quaternion innerRailRotation = Quaternion.LookRotation(_rotParams.inner.RotationAxis, Vector3.Cross(_rotParams.middle.RotationAxis, _rotParams.inner.RotationAxis)); 
            inner.rail.transform.rotation = outerRingRotation * middleRingRotation * innerRailRotation;
            
            Quaternion innerRingRotation = Quaternion.AngleAxis(_rotParams.inner.Angle * Mathf.Rad2Deg, _rotParams.inner.RotationAxis);
            inner.ring.transform.rotation = outerRingRotation * middleRingRotation * innerRingRotation * innerRailRotation; 
        }

        private void VisUpdatePlaneArcShaderColors()
        {
            VisUpdatePlaneArcShaderColourSingle(_rotParams.outer, outer.vis_planeArc);
            VisUpdatePlaneArcShaderColourSingle(_rotParams.middle, middle.vis_planeArc);
            VisUpdatePlaneArcShaderColourSingle(_rotParams.inner, inner.vis_planeArc);
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
                outer.vis_planeArc.StartingAngle = -_rotParams.outer.Angle;
            }

            if (middle.vis_planeArc != null)
            {
                middle.vis_planeArc.StartingAngle = -_rotParams.middle.Angle;
            }
            
            if (inner.vis_planeArc != null)
            {
                inner.vis_planeArc.StartingAngle = -_rotParams.inner.Angle;
            }
        }
        
        private void OnValidate()
        {
            if (_rotParams.GetGimbleType() == EGimbleType.Invalid)
            {
                Debug.LogWarning("{gameObject.name} is set to an invalid GimbalType");
            }

            VisUpdate(); 
        }
    }
}