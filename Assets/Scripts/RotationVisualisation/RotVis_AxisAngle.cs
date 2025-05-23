using RotParams;
using UnityEngine;
using UnityEngine.Serialization;
using Visualisation;

namespace RotationVisualisation
{
    //-ZyKa check which MonoBehaviours can be reduced to Behaviour or Component
    public class RotVis_AxisAngle : MonoBehaviour
    {
        [SerializeField] private RotParams_AxisAngle rotParams_AxisAngle; 
        [SerializeField] private Vis_Vector vis_rotationVector;
        [SerializeField] private Vis_PlaneArc vis_PlaneArc; 
        
        public RotParams_AxisAngle RotParams
        {
            get => rotParams_AxisAngle;
            set => rotParams_AxisAngle = value;
        }
        
        public Vector3 RotationVector
        {
            get => RotParams.RotationVector;
            set
            {
                RotParams.RotationVector = value;
                UpdateVisualisation();
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

        public void UpdateVisualisation()
        {
            vis_rotationVector.Value = RotationVector;
            vis_PlaneArc.LocalRotationAxis = Axis; 
            vis_PlaneArc.StartingAngle = 0; 
            vis_PlaneArc.EndingAngle = RotationVector.magnitude;
        }

        private void OnValidate()
        {
            RotationVector = RotParams.RotationVector;
        }
    }
}