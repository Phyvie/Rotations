using System;
using RotationTypes;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_EulerAngle : MonoBehaviour
    {
        [SerializeField] private RotParams_EulerAngles _rotParams;

        [System.Serializable]
        private class RailRingPair
        {
            [SerializeField] public GameObject rail;
            [SerializeField] public Vis_PlaneArc ring; 
        }
        [SerializeField] private RailRingPair outer;
        [SerializeField] private RailRingPair middle;
        [SerializeField] private RailRingPair inner;
        
        public void UpdateVisualisation()
        {
            
        }

        private void SetRailRingRotation(RailRingPair pair, _RotParams_EulerAngleGimbleRing ringParams)
        {
            pair.rail.transform.rotation = Quaternion.FromToRotation(Vector3.right, ringParams.GetLocalRotationAxis());
            pair.ring.EndingAngle = ringParams.angle; 
        }
        
        private void OnValidate()
        {
            if (_rotParams.GetGimbleType() == EGimbleType.Invalid)
            {
                Debug.LogWarning("{gameObject.name} is set to an invalid GimbalType");
            }
        }
    }
}