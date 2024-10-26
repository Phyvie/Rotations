using RotationTypes;
using UnityEngine;

namespace RotationVisualisation
{
    public class MB_Rotation3D : MonoBehaviour
    {
        [SerializeField] private EulerAngleRotation eulerAngleRotation = new EulerAngleRotation(); 
        [SerializeField] private QuaternionRotation complexRotation = new QuaternionRotation();
        [SerializeField] private AxisAngleRotation axisAngleRotation = new AxisAngleRotation();
        [SerializeField] private MatrixRotation matrixRotation = new MatrixRotation();

        [SerializeField] private MB_Matrix _mbMatrix;

        [ContextMenu("ConnectMBMatrix")]
        public void ConnectMBMatrix()
        {
            _mbMatrix.RefMatrix = matrixRotation.InternalMatrix; 
        }
    }
}
