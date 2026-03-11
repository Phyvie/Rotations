using System;
using BaseClasses;
using MathVisualisation;
using RotParams;
using UnityEngine;
using UnityEngine.Serialization;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_Quaternion : RotVis_TemplateBase<RotParams_Quaternion>
    {
        //LaterZyKa RotVis_Quat: RotVis update this to be able to take in non-unit quaternions
        [SerializeField] private Vis_Vector vis_rotationVector;
        [SerializeField] private Vis_Axis vis_Axis; 
        [FormerlySerializedAs("vis_PlaneArc")] [SerializeField] private Vis_Angle visAngle;
        
        public RotVis_Quaternion(RotParams_Quaternion rotParams_Quaternion) : base(rotParams_Quaternion)
        {
            
        }
        
        public Vector3 Axis => rotParams.NormalizedAxis;

        public float Angle => rotParams.AngleInRadian;

        public override void VisUpdate()
        {
            vis_rotationVector.Value = Axis * Angle;

            vis_Axis.Value = Axis; 
            
            visAngle.LocalUpAxis = Axis; 
            visAngle.BeginAngle = 0; 
            visAngle.EndingAngle = Angle;
            
            vis_rotationVector.Color = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(Axis,
                    rotParams.AngleInRadian > 0);
            vis_Axis.Color = ColorPalette.RotationPalette.InterpColorForAxisAndSign(Axis,
                rotParams.AngleInRadian > 0);
            visAngle.PositiveAngleColor = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(Axis, true);
            visAngle.NegativeAngleColor = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(Axis, false);
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