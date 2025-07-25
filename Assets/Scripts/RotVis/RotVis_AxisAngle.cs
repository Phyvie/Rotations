using System;
using BaseClasses;
using MathVisualisation;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_AxisAngle : RotVis_TemplateBase<RotParams_AxisAngle>
    {
        [SerializeField] private Vis_Vector vis_rotationVector;
        [SerializeField] private Vis_Axis vis_axis;
        [SerializeField] private Vis_PlaneArc vis_PlaneArc;

        public RotVis_AxisAngle(RotParams_AxisAngle rotParams) : base(rotParams)
        {
            
        }

        public Vector3 RotationVectorInRadian
        {
            get => rotParams.RotationVectorInRadian;
            set => rotParams.RotationVectorInRadian = value; 
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
            vis_rotationVector.Value = NormalisedAxis * Mathf.Abs(AngleInRadian);
            
            vis_axis.Value = NormalisedAxis;
            
            vis_PlaneArc.LocalUpAxis = NormalisedAxis; 
            vis_PlaneArc.BeginAngle = 0; 
            vis_PlaneArc.EndingAngle = AngleInRadian;
 
            vis_rotationVector.Color = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis,
                rotParams.AngleInCurrentUnit > 0);
            vis_axis.Color = ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis,
                rotParams.AngleInCurrentUnit > 0);
            vis_PlaneArc.PositiveAngleColor = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis, true);
            vis_PlaneArc.NegativeAngleColor = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis, false);
            
            Debug.Log($"!ZyKa: {nameof(RotVis_AxisAngle)}.{nameof(VisUpdate)}");
        }

        #if UNITY_EDITOR
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
        #endif
    }
}