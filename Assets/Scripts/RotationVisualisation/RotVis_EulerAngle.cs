using System;
using RotationTypes;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_EulerAngle : MonoBehaviour
    {
        [SerializeField] private RotParams_EulerAngles _rotParams;
        private RotParams_EulerAngles _previousRotParams; 

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

        public void Initialise()
        {
            outer.vis_planeArc.LocalRotationAxis = Vector3.right;
            middle.vis_planeArc.LocalRotationAxis = Vector3.right;
            inner.vis_planeArc.LocalRotationAxis = Vector3.right;
        }
        
        public void UpdateVisualisation()
        {
            if (!RotParams_EulerAngles.AreAxesMatching(_rotParams, _previousRotParams))
            {
                
            }

            Quaternion OuterRailRotation = Quaternion.FromToRotation(Vector3.right, _rotParams.outer.RotationAxis);
            Quaternion OuterRingRotation = Quaternion.AngleAxis(_rotParams.outer.Angle * Mathf.Rad2Deg, _rotParams.outer.RotationAxis);
            Quaternion MiddleRailRotation = Quaternion.FromToRotation(Vector3.right, _rotParams.middle.RotationAxis);
            Quaternion MiddleRingRotation = Quaternion.AngleAxis(_rotParams.middle.Angle * Mathf.Rad2Deg, _rotParams.middle.RotationAxis);
            Quaternion InnerRailRotation = Quaternion.FromToRotation(Vector3.right, _rotParams.inner.RotationAxis);
            Quaternion InnerRingRotation = Quaternion.AngleAxis(_rotParams.inner.Angle * Mathf.Rad2Deg, _rotParams.inner.RotationAxis);

            outer.rail.transform.rotation = OuterRailRotation;
            outer.ring.transform.rotation = OuterRingRotation * OuterRailRotation;
            middle.rail.transform.rotation = MiddleRailRotation * OuterRingRotation;
            middle.ring.transform.rotation = MiddleRingRotation * MiddleRailRotation * OuterRingRotation;
            inner.rail.transform.rotation = InnerRailRotation * MiddleRingRotation * OuterRingRotation;
            inner.ring.transform.rotation = InnerRingRotation * InnerRailRotation * MiddleRingRotation * OuterRingRotation; 
            
            /*
            //1. Set gimbal Up for unrotated axes
            //these calculations are only necessary, when the order of gimbal-rings is changed
            outer.rail.transform.rotation = Quaternion.FromToRotation(Vector3.right, _rotParams.outer.RotationAxis); 
            middle.rail.transform.rotation = middle.rail.transform.parent.rotation * Quaternion.FromToRotation(Vector3.right, _rotParams.middle.RotationAxis);
            inner.rail.transform.rotation = inner.rail.transform.parent.rotation * Quaternion.FromToRotation(Vector3.right, _rotParams.inner.RotationAxis); 
            
            //2. rotate the gimbal
            // outer.vis_planeArc.EndingAngle = _rotParams.outer.Angle;
            outer.ring.transform.localRotation = new Quaternion(Mathf.Cos(_rotParams.outer.Angle/2), 0, 0, Mathf.Sin(_rotParams.outer.Angle/2)); 
            // middle.vis_planeArc.EndingAngle = _rotParams.middle.Angle;
            middle.ring.transform.localRotation = new Quaternion(Mathf.Cos(_rotParams.middle.Angle/2), 0, 0, Mathf.Sin(_rotParams.middle.Angle/2));
            // inner.vis_planeArc.EndingAngle = _rotParams.inner.Angle; 
            inner.ring.transform.localRotation = new Quaternion(Mathf.Cos(_rotParams.inner.Angle/2), 0, 0, Mathf.Sin(_rotParams.inner.Angle/2)); 
            */
        }

        private void ResetToZero()
        {
            outer.rail.transform.rotation = Quaternion.identity; 
        }
        
        private void OnValidate()
        {
            if (_rotParams.GetGimbleType() == EGimbleType.Invalid)
            {
                Debug.LogWarning("{gameObject.name} is set to an invalid GimbalType");
            }

            UpdateVisualisation(); 
        }
    }
}