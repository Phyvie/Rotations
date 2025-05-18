using UnityEngine;
using UnityEngine.Serialization;

namespace Visualisation
{
    public class Vis_PlaneArc : MonoBehaviour
    {
        [SerializeField] private Vector3 localRotationAxis; 
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

        public Vector3 LocalRotationAxis
        {
            get => localRotationAxis;
            set
            {
                localRotationAxis = value;
                transform.localRotation = Quaternion.FromToRotation(Vector3.right, localRotationAxis);
                UpdateVisualisation();
            }
        }
        
        public void UpdateVisualisation()
        {
            transform.localRotation = Quaternion.FromToRotation(Vector3.right, localRotationAxis);

            if (torusMaterial != null)
            {
                torusMaterial.BeginAngle = beginAngle;
                torusMaterial.EndAngle = endingAngle; 
            }

            if (circleMaterial != null)
            {
                circleMaterial.BeginAngle = beginAngle;
                circleMaterial.EndAngle = endingAngle;
            }
        }
        
        public void OnValidate()
        {
            StartingAngle = beginAngle;
            EndingAngle = endingAngle;
            LocalRotationAxis = localRotationAxis;
        }
    }
}