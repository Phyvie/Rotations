using RotationTypes;
using UnityEngine;

namespace RotationVisualisation
{
    public class MB_Rotation3D : MonoBehaviour
    {
        [ContextMenu("AddToZyKa")]
        public void AddToZyKa()
        {
            eulerAngleRotation.ZyKa.Add(AngleType.Radian);
        }
        [ContextMenu("RemoveFromZyKa")]
        public void RemoveZyKa()
        {
            eulerAngleRotation.ZyKa.RemoveAt(0); 
        }
        
        
        #region RotationDataTypes
        #region Variables
        [SerializeField] private EulerAngleRotation eulerAngleRotation = new EulerAngleRotation(); 
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
        //TODO: Check the conversion between Rotations
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
