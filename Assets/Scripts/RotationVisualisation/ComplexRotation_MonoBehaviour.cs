using RotationTypes;
using UnityEngine;

namespace RotationVisualisation
{
    public class ComplexRotation_MonoBehaviour : MonoBehaviour
    {
        [SerializeField] private Matrix4x4 zyKa; 

        [SerializeField] private Vector3 UnrotatedVector; 
        [SerializeField] private Vector3 RotatedVector; 
        
        [SerializeField] private ComplexRotation _rotation = new ComplexRotation();

        public ComplexRotation Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }
    }
}
