using BaseClasses;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_Quaternion : RotVis
    {
        [SerializeField] private Vis_Vector vis_rotationVector;
        [SerializeField] private Vis_PlaneArc vis_PlaneArc;
        
        public RotVis_Quaternion(RotParams_Quaternion rotParams_Quaternion) : base(rotParams_Quaternion)
        {
            
        }

        [SerializeField] private RotParams_Quaternion rotParams;
        public override RotParams.RotParams GetRotParams()
        {
            return rotParams; 
        }

        public override void SetRotParams(ref RotParams.RotParams newRotParams)
        {
            if (newRotParams is RotParams_Quaternion rotParamsQuaternion)
            {
                rotParams = rotParamsQuaternion;
                VisUpdate();
            }
        }
        
        public Vector3 Axis
        {
            get => rotParams.Axis;
            set
            {
                rotParams.Axis = value;
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
            vis_PlaneArc.LocalRotationAxis = Axis; 
            vis_PlaneArc.StartingAngle = 0; 
            vis_PlaneArc.EndingAngle = Angle;
        }

        private void OnValidate()
        {
            Angle = rotParams.Angle;
            Axis = rotParams.Axis;
        }
    }
}