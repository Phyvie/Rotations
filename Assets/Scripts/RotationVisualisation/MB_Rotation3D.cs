using RotationTypes;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotationVisualisation
{
    public class MB_Rotation3D : MonoBehaviour
    {
        #region RotationDataTypes
        #region Variables
        [FormerlySerializedAs("eulerAngleRotationDeprecated")] [SerializeField] private EulerAngleRotation eulerAngleRotation = new EulerAngleRotation(); 
        [SerializeField] private QuaternionRotation quaternionRotation = new QuaternionRotation();
        [SerializeField] private AxisAngleRotation axisAngleRotation = new AxisAngleRotation();
        [SerializeField] private MatrixRotation matrixRotation = new MatrixRotation();
        #endregion
        #region ConverterFunction

        void UpdateRotationsBasedOn(RotationType rotationType)
        {
            eulerAngleRotation = eulerAngleRotation.ToEulerAngleRotation(); 
            quaternionRotation = eulerAngleRotation.ToQuaternionRotation();
            axisAngleRotation = eulerAngleRotation.ToAxisAngleRotation();
            matrixRotation = eulerAngleRotation.ToMatrixRotation(); 
        }
        
        [ContextMenu("UpdateBasedOnEuler")]
        void UpdateBasedOnEuler()
        {
            UpdateRotationsBasedOn(eulerAngleRotation);
        }
        [ContextMenu("UpdateBasedOnQuaternion")]
        void UpdateBasedOnQuaternion()
        {
            UpdateRotationsBasedOn(quaternionRotation);
        }
        [ContextMenu("UpdateBasedOnAxisAngle")]
        void UpdateBasedOnAxisAngle()
        {
            UpdateRotationsBasedOn(axisAngleRotation);
        }
        [ContextMenu("UpdateBasedOnMatrix")]
        void UpdateBasedOnMatrix()
        {
            UpdateRotationsBasedOn(matrixRotation);
        }
        #endregion
        #endregion
        
        #region RotationMonoBehaviourTypes   
        [SerializeField] private MB_Matrix _mbMatrix;

        [ContextMenu("ConnectMBMatrix")]
        public void ConnectMBMatrix()
        {
            _mbMatrix.RefMatrix = matrixRotation.InternalMatrix; 
        }
        #endregion
    }
}
