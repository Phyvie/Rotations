using System;
using BaseClasses;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    //-ZyKa check which MonoBehaviours can be reduced to Behaviour or Component
    public class RotVis_AxisAngle : RotVis
    {
        [SerializeField] private Vis_Vector vis_rotationVector;
        [SerializeField] private Vis_PlaneArc vis_PlaneArc;

        public RotVis_AxisAngle(RotParams_AxisAngle rotParams) : base(rotParams)
        {
            
        }

        [SerializeField] private RotParams_AxisAngle rotParams;
        public override RotParams.RotParams GetRotParams()
        {
            return rotParams; 
        }

        public override void SetRotParamsByRef(ref RotParams.RotParams newRotParams)
        {
            if (newRotParams is RotParams_AxisAngle rotParamsAxisAngle)
            {
                rotParams = rotParamsAxisAngle;
                VisUpdate();
            }
        }
        
        public Vector3 RotationVector
        {
            get => rotParams.RotationVector;
            set
            {
                rotParams.RotationVector = value;
                VisUpdate();
            }
        }

        public Vector3 NormalisedAxis
        {
            get => rotParams.NormalisedAxis;
            set => rotParams.NormalisedAxis = value.normalized;
        }

        public float AngleInRadian
        {
            get => rotParams.AngleInRadian; 
            set => rotParams.AngleInRadian = value;
        }

        public override void VisUpdate()
        {
            vis_rotationVector.Value = RotationVector;
            vis_PlaneArc.LocalRotationAxis = NormalisedAxis; 
            vis_PlaneArc.StartingAngle = 0; 
            vis_PlaneArc.EndingAngle = RotationVector.magnitude;
            VisUpdateRotationObject(); 
        }

        private void OnValidate()
        {
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