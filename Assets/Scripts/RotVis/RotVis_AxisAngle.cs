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

        public override void SetRotParams(RotParams.RotParams newRotParams)
        {
            if (newRotParams.GetType().IsSubclassOf(typeof(RotParams_AxisAngle)))
            {
                rotParams = (RotParams_AxisAngle)newRotParams;
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

        public Vector3 Axis
        {
            get => RotationVector.normalized;
            set => RotationVector = RotationVector.magnitude * value.normalized;
        }

        public float Angle
        {
            get => RotationVector.magnitude; 
            set => RotationVector = RotationVector.normalized * Angle;
        }

        public override void VisUpdate()
        {
            vis_rotationVector.Value = RotationVector;
            vis_PlaneArc.LocalRotationAxis = Axis; 
            vis_PlaneArc.StartingAngle = 0; 
            vis_PlaneArc.EndingAngle = RotationVector.magnitude;
        }

        private void OnValidate()
        {
            RotationVector = rotParams.RotationVector;
        }
    }
}