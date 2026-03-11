using System;
using BaseClasses;
using MathVisualisation;
using RotParams;
using UnityEngine;
using UnityEngine.Serialization;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_AxisAngle : RotVis_TemplateBase<RotParams_AxisAngle>
    {
        [SerializeField] private Vis_Vector vis_rotationVector;
        [SerializeField] private Vis_Axis vis_axis;
        [FormerlySerializedAs("vis_PlaneArc")] [SerializeField] private Vis_Angle visAngle;

        public RotVis_AxisAngle(RotParams_AxisAngle rotParams) : base(rotParams) { }

        public Vector3 RotationVectorInRadian => rotParams.RotationVectorInRadian;

        public Vector3 NormalisedAxis => rotParams.NormalisedAxis;

        public float AngleInRadian => rotParams.AngleInRadian; 

        public override void VisUpdate()
        {
            vis_rotationVector.Value = NormalisedAxis * Mathf.Abs(AngleInRadian);
            
            vis_axis.Value = NormalisedAxis;
            
            visAngle.LocalUpAxis = NormalisedAxis; 
            visAngle.BeginAngle = 0; 
            visAngle.EndingAngle = AngleInRadian;
 
            vis_rotationVector.Color = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis,
                rotParams.AngleInCurrentUnit > 0);
            vis_axis.Color = ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis,
                rotParams.AngleInCurrentUnit > 0);
            visAngle.PositiveAngleColor = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis, true);
            visAngle.NegativeAngleColor = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(NormalisedAxis, false);
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