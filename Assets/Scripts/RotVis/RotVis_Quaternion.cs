using System;
using BaseClasses;
using MathVisualisation;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_Quaternion : RotVis_Base
    {
        //-ZyKa Quaternion -ZyKa RotVis update this to be able to take in non-unit quaternions
        [SerializeField] private Vis_Vector vis_rotationVector;
        [SerializeField] private Vis_Axis vis_Axis; 
        [SerializeField] private Vis_PlaneArc vis_PlaneArc;
        
        public RotVis_Quaternion(RotParams_Quaternion rotParams_Quaternion) : base(rotParams_Quaternion)
        {
            
        }

        [SerializeField] private RotParams_Quaternion rotParams;
        public override RotParams_Base GetRotParams()
        {
            return rotParams; 
        }

        public override void SetRotParamsByRef(RotParams_Base newRotParams)
        {
            base.SetRotParamsByRef(newRotParams);
            if (newRotParams is RotParams_Quaternion rotParamsQuaternion)
            {
                rotParams = rotParamsQuaternion;
                VisUpdate();
            }
        }
        
        public Vector3 Axis
        {
            get => rotParams.NormalizedAxis;
            set
            {
                rotParams.NormalizedAxis = value;
                VisUpdate();
            }
        }

        public float Angle
        {
            get => rotParams.Angle;
            set
            {
                rotParams.Angle = value; 
                VisUpdate();
            }
        }

        public override void VisUpdate()
        {
            vis_rotationVector.Value = Axis * Angle;

            vis_Axis.Value = Axis; 
            
            vis_PlaneArc.LocalUpAxis = Axis; 
            vis_PlaneArc.BeginAngle = 0; 
            vis_PlaneArc.EndingAngle = Angle;
            
            vis_rotationVector.Color = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(Axis,
                    rotParams.Angle > 0);
            vis_Axis.Color = ColorPalette.RotationPalette.InterpColorForAxisAndSign(Axis,
                rotParams.Angle > 0);
            vis_PlaneArc.PositiveAngleColor = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(Axis, true);
            vis_PlaneArc.NegativeAngleColor = 
                ColorPalette.RotationPalette.InterpColorForAxisAndSign(Axis, false);
            
            VisUpdateRotationObject();
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

        [ContextMenu("ResetToIdentity")]
        private void ResetToIdentity()
        {
            rotParams = new RotParams_Quaternion();
        }
    }
}