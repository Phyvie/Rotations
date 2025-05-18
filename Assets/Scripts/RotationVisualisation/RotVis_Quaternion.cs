using RotationTypes;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_Quaternion : MonoBehaviour
    {
        
        [SerializeField] private RotParams_Quaternion rotParams_Quaternion; 
        [SerializeField] private Vis_Vector vis_rotationVector;
        [SerializeField] private Vis_PlaneArc vis_PlaneArc; 
        
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
                UpdateVisualisation();
            }
        }

        public float Angle
        {
            get => RotParams.Angle;
            set
            {
                RotParams.Angle = value; 
                UpdateVisualisation();
            }
        }

        public void UpdateVisualisation()
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