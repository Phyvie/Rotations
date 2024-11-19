using System;
using MeshGenerator;
using RotationTypes;
using UnityEngine;

namespace RotationVisualisation
{
    [RequireComponent(typeof(TorusGenerator))]
    public class MB_Rotation3D : MonoBehaviour
    {
        [SerializeField] private RotationType activeRotationType; 
        [SerializeField] private TorusGenerator ref_TorusGenerator;
        [SerializeField] private GameObject ref_UnrotatedPoint;
        [SerializeField] private GameObject ref_RotatedPoint;

        private void Awake()
        {
            ref_TorusGenerator = GetComponent<TorusGenerator>(); 
        }

        [SerializeField]
        void UpdateTorusMesh()
        {
            
        }

        [SerializeField]
        void UpdatePointRotation()
        {
            ref_RotatedPoint.transform.position = activeRotationType.RotateVector(ref_UnrotatedPoint.transform.position); 
        }
    }
}
