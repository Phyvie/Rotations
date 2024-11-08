using RotationTypes;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotationVisualisation
{
    public class MB_Rotation3D : MonoBehaviour
    {
        #region RotationDataTypes
        #region Variables
        [FormerlySerializedAs("eulerAngleRotation")] [SerializeField] private EulerAngleRotationDeprecated eulerAngleRotationDeprecated = new EulerAngleRotationDeprecated(); 
        [SerializeField] private QuaternionRotation quaternionRotation = new QuaternionRotation();
        [SerializeField] private AxisAngleRotation axisAngleRotation = new AxisAngleRotation();
        [SerializeField] private MatrixRotation matrixRotation = new MatrixRotation();
        #endregion
        #region ConverterFunction

        void UpdateRotationsBasedOn(RotationType rotationType)
        {
            eulerAngleRotationDeprecated = eulerAngleRotationDeprecated.ToEulerAngleRotation(); 
            quaternionRotation = eulerAngleRotationDeprecated.ToQuaternionRotation();
            axisAngleRotation = eulerAngleRotationDeprecated.ToAxisAngleRotation();
            matrixRotation = eulerAngleRotationDeprecated.ToMatrixRotation(); 
        }
        
        [ContextMenu("UpdateBasedOnEuler")]
        void UpdateBasedOnEuler()
        {
            UpdateRotationsBasedOn(eulerAngleRotationDeprecated);
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
