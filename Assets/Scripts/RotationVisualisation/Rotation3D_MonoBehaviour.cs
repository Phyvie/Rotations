using RotationTypes;
using Unity.VisualScripting;
using UnityEngine;

namespace RotationVisualisation
{
    public class Rotation3D_MonoBehaviour : MonoBehaviour
    {
        [SerializeField] private EulerAngleRotation eulerAngleRotation = new EulerAngleRotation(); 
        [SerializeField] private QuaternionRotation complexRotation = new QuaternionRotation();
        [SerializeField] private AxisAngleRotation axisAngleRotation = new AxisAngleRotation();
        [SerializeField] private MatrixRotation matrixRotation = new MatrixRotation(); 

    }
}
