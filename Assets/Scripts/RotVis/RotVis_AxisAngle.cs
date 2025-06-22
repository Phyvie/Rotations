using System;
using BaseClasses;
using MathVisualisation;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    //-ZyKa check which MonoBehaviours can be reduced to Behaviour or Component
    public class RotVis_AxisAngle : RotVis
    {
        [SerializeField] private Vis_Vector vis_rotationVector;
        [SerializeField] private Vis_Axis vis_axis;
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
            base.SetRotParamsByRef(ref newRotParams);
            if (newRotParams is RotParams_AxisAngle rotParamsAxisAngle)
            {
                rotParams = rotParamsAxisAngle;
                VisUpdate();
            }
        }
        
        public Vector3 RotationVectorInRadian
        {
            get => rotParams.RotationVectorInRadian;
            set
            {
                rotParams.RotationVectorInRadian = value;
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
            get => rotParams.AngleRadianInRadian; 
            set => rotParams.AngleRadianInRadian = value;
        }

        public override void VisUpdate()
        {
            vis_rotationVector.Value = NormalisedAxis * Mathf.Abs(AngleInRadian);
            
            vis_axis.Value = NormalisedAxis;
            
            vis_PlaneArc.LocalRotationAxis = NormalisedAxis; 
            vis_PlaneArc.StartingAngle = 0; 
            vis_PlaneArc.EndingAngle = RotationVectorInRadian.magnitude;
 
            vis_rotationVector.Color = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis,
                rotParams.AngleInCurrentUnit > 0);
            vis_axis.Color = ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis,
                rotParams.AngleInCurrentUnit > 0);
            vis_PlaneArc.PositiveAngleColor = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis, true);
            vis_PlaneArc.NegativeAngleColor = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis, false);
            
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