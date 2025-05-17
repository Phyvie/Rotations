using UnityEngine;

namespace Visualisation
{
    public class Vis_PlaneArc : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationAxis; 
        [SerializeField] private float beginAngle; 
        [SerializeField] private float endingAngle; 
        [SerializeField] private M_CircleSector torusMaterial; 
        [SerializeField] private M_CircleSector circleMaterial;
        
        public float StartingAngle
        {
            get => beginAngle;
            set
            {
                beginAngle = value;
                UpdateVisualisation();
            }
        }

        public float EndingAngle
        {
            get => endingAngle;
            set
            {
                endingAngle = value;
                UpdateVisualisation();
            }
        }

        public Vector3 RotationAxis
        {
            get => rotationAxis;
            set
            {
                rotationAxis = value;
                transform.rotation = Quaternion.FromToRotation(Vector3.right, rotationAxis);
                UpdateVisualisation();
            }
        }

        public void UpdateVisualisation()
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.right, rotationAxis);
            
            torusMaterial.BeginAngle = beginAngle;
            torusMaterial.EndAngle = endingAngle; 
            
            circleMaterial.BeginAngle = beginAngle;
            circleMaterial.EndAngle = endingAngle;
        }
        
        public void OnValidate()
        {
            StartingAngle = beginAngle;
            EndingAngle = endingAngle;
            RotationAxis = rotationAxis;
        }
    }
}