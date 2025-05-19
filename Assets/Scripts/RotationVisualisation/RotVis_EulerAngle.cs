using System;
using RotationTypes;
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
        
        public void VisUpdate()
        {
            if (!RotParams_EulerAngles.AreAxesMatching(_rotParams, _previousRotParamAxes))
            {
                VisReset(); 
                VisUpdateRailsForUnrotatedGimbal();
                _previousRotParamAxes = new RotParams_EulerAngles(_rotParams);
            }
            VisUpdateRings();
            VisUpdatePlaneArcs(); 
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
        
        private void VisUpdateRings()
        {
            outer.ring.transform.localRotation = new Quaternion(Mathf.Sin(_rotParams.outer.Angle/2), 0, 0, Mathf.Cos(_rotParams.outer.Angle/2)); 
            middle.ring.transform.localRotation = new Quaternion(Mathf.Sin(_rotParams.middle.Angle/2), 0, 0, Mathf.Cos(_rotParams.middle.Angle/2)); 
            inner.ring.transform.localRotation = new Quaternion(Mathf.Sin(_rotParams.inner.Angle/2), 0, 0, Mathf.Cos(_rotParams.inner.Angle/2));
        }

        private void VisUpdateRailsForUnrotatedGimbal()
        {
            outer.rail.transform.rotation = Quaternion.FromToRotation(Vector3.right, _rotParams.outer.RotationAxis);
            middle.rail.transform.rotation = Quaternion.FromToRotation(Vector3.right, _rotParams.middle.RotationAxis);
            inner.rail.transform.rotation = Quaternion.FromToRotation(Vector3.right, _rotParams.inner.RotationAxis);
        }

        private void VisFullUpdateRotations()
        {
            Quaternion outerRailRotation = Quaternion.FromToRotation(Vector3.right, _rotParams.outer.RotationAxis);
            outer.rail.transform.rotation = outerRailRotation;
            
            Quaternion outerRingRotation = Quaternion.AngleAxis(_rotParams.outer.Angle * Mathf.Rad2Deg, _rotParams.outer.RotationAxis);
            outer.ring.transform.rotation = outerRingRotation * outerRailRotation;
            
            Quaternion middleRailRotation = Quaternion.FromToRotation(Vector3.right, _rotParams.middle.RotationAxis);
            middle.rail.transform.rotation = middleRailRotation * outerRingRotation;
            
            Quaternion middleRingRotation = Quaternion.AngleAxis(_rotParams.middle.Angle * Mathf.Rad2Deg, _rotParams.middle.RotationAxis);
            middle.ring.transform.rotation = outerRingRotation * middleRingRotation * middleRailRotation; 
            
            Quaternion innerRailRotation = Quaternion.FromToRotation(Vector3.right, _rotParams.inner.RotationAxis);
            inner.rail.transform.rotation = outerRingRotation * middleRingRotation * innerRailRotation;
            
            Quaternion innerRingRotation = Quaternion.AngleAxis(_rotParams.inner.Angle * Mathf.Rad2Deg, _rotParams.inner.RotationAxis);
            inner.ring.transform.rotation = outerRingRotation * middleRingRotation * innerRingRotation * innerRailRotation; 
        }

        private void VisUpdatePlaneArcs()
        {
            //!ZyKa Improve the PlaneArcs, so that they either set or don't set the rotation of the object
            if (outer.vis_planeArc != null)
            {
                outer.vis_planeArc.EndingAngle = _rotParams.outer.Angle;
            }

            if (middle.vis_planeArc != null)
            {
                middle.vis_planeArc.EndingAngle = _rotParams.middle.Angle;
            }
            
            if (inner.vis_planeArc != null)
            {
                inner.vis_planeArc.EndingAngle = _rotParams.inner.Angle;
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