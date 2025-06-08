using BaseClasses;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_Quaternion : RotVis<RotParams_Quaternion>
    {
        
        [SerializeField] private RotParams_Quaternion rotParams_Quaternion; 
        [SerializeField] private Vis_Vector vis_rotationVector;
        [SerializeField] private Vis_PlaneArc vis_PlaneArc;

        public RotVis_Quaternion(RotParams_Quaternion rotParams_Quaternion) : base(rotParams_Quaternion)
        {
            
        }
        
        public RotParams_Quaternion RotParams
        {
            get => rotParams_Quaternion;
            set => rotParams_Quaternion = value;
        }
        
        public Vector3 Axis
        {
            get => RotParams.Axis;
            set
            {
                RotParams.Axis = value;
                VisUpdate();
            }
        }

        public float Angle
        {
            get => RotParams.Angle;
            set
            {
                RotParams.Angle = value; 
                VisUpdate();
            }
        }

        public override void VisUpdate()
        {
            vis_rotationVector.Value = Axis * Angle;
            vis_PlaneArc.LocalRotationAxis = Axis; 
            vis_PlaneArc.StartingAngle = 0; 
            vis_PlaneArc.EndingAngle = Angle;
        }

        private void OnValidate()
        {
            Angle = rotParams_Quaternion.Angle;
            Axis = rotParams_Quaternion.Axis;
        }
    }
}