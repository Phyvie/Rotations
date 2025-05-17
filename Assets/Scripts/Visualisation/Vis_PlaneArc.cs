using UnityEngine;

namespace Visualisation
{
    public class Vis_PlaneArc : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationAxis; 
        [SerializeField] private float startingAngle; //+ZyKa create shader for arcs && make the shaders subsribe to this; maybe install unity-atoms 
        [SerializeField] private float endingAngle; 
        [SerializeField] private GameObject torus; 
        [SerializeField] private GameObject circle;
        
        public float StartingAngle
        {
            get => startingAngle;
            set => startingAngle = value;
        }

        public float EndingAngle
        {
            get => endingAngle;
            set => endingAngle = value;
        }

        public Vector3 RotationAxis
        {
            get => rotationAxis;
            set
            {
                transform.rotation = Quaternion.FromToRotation(Vector3.right, rotationAxis);
            }
        }

        public void OnValidate()
        {
            StartingAngle = startingAngle;
            EndingAngle = endingAngle;
            RotationAxis = rotationAxis;
        }
    }
}