using MathExtensions;
using UnityEngine;

namespace Visualisation
{
    public class Vis_PlaneArc : MonoBehaviour
    {
        [SerializeField] private Vector3 localRotationAxis;
        [SerializeField] private Vector3 localForwardVector;
        [SerializeField] private float beginAngle;
        [SerializeField] private float endingAngle;
        [SerializeField] private M_CircleSector torusMaterial;
        [SerializeField] private M_CircleSector circleMaterial;
        
        public Vector3 LocalRotationAxis
        {
            get => localRotationAxis;
            set
            {
                localRotationAxis = value;
                VisUpdateRotationAxis();
            }
        }

        public Vector3 GlobalRotationAxis
        {
            get => transform.parent != null ? transform.parent.rotation * localRotationAxis : localRotationAxis;
            set
            {
                LocalRotationAxis = transform.parent != null ? 
                    Quaternion.Inverse(transform.parent.rotation) * value : 
                    value; 
            }
        }
        
        public Vector3 LocalForwardVector
        {
            get => localForwardVector;
            set
            {
                localForwardVector = value;
                VisUpdateRotationForward(); 
            }
        }

        public Vector3 GlobalForwardVector
        {
            get => transform.parent != null ? transform.parent.rotation * localForwardVector : localForwardVector;
            set
            {
                LocalForwardVector = transform.parent != null ? 
                    Quaternion.Inverse(transform.parent.rotation) * value : 
                    value; 
            }
        }
        
        public float StartingAngle
        {
            get => beginAngle;
            set
            {
                beginAngle = value;
                VisUpdateShader();
            }
        }

        public float EndingAngle
        {
            get => endingAngle;
            set
            {
                endingAngle = value;
                VisUpdateShader();
            }
        }
        
        public void VisUpdate()
        {
            VisUpdateRotation();
            VisUpdateShader(); 
        }

        private void VisUpdateRotation()
        {
            VisUpdateRotationViaRotatingFromPreviousToCurrent(); 
        }

        private void VisUpdateShader()
        {
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
            // AdjustForwardToUp();
            VisUpdate();
        }
        
        #region VisUpdateRotationByOrientationInterpolation
        //-ZyKa Good example why Rotation Vector Interpolation does not work
        //This function does not work properly because the distance between the quaternion is too big; e.g. interpolating between (0.5, 0.5, 0.5, -0.5f) and (0.5, 0.5, 0.5, 0.5) results in 0
        private void VisUpdateRotationByOrientationInterpolation()
        {
            Debug.Log($"OrientationNormalUp: {OrientationNormalUp}");
            Debug.Log($"OrientationNormalForward: {OrientationNormalForward}");
            Debug.Log($"OrientationNormalRight: {OrientationNormalRight}");
            Debug.Log($"OrientationNormalDown: {OrientationNormalDown}");
            Debug.Log($"OrientationNormalBackward: {OrientationNormalBackward}");
            Debug.Log($"OrientationNormalLeft: {OrientationNormalLeft}");

            float[] weights = new float[3];
            Quaternion[] orientations = new Quaternion[3];

            orientations[0] = LocalRotationAxis.x >= 0 ? OrientationNormalRight : OrientationNormalLeft;
            weights[0] = LocalRotationAxis.x * LocalRotationAxis.x;

            orientations[1] = LocalRotationAxis.y >= 0 ? OrientationNormalUp : OrientationNormalDown;
            weights[1] = LocalRotationAxis.y * LocalRotationAxis.y;

            orientations[2] = LocalRotationAxis.z >= 0 ? OrientationNormalForward : OrientationNormalBackward;
            weights[2] = LocalRotationAxis.z * LocalRotationAxis.z;

            transform.localRotation =
                Math.QuaternionInterpolateAsRotationVectors(orientations, weights);
        }

        static readonly Quaternion OrientationNormalUp = new Quaternion(0, 0, 0, 1);
        public static readonly Quaternion OrientationNormalForward = new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);
        public static readonly Quaternion OrientationNormalRight = new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f);
        public static readonly Quaternion OrientationNormalDown = new Quaternion(0, 0, 1, 0);
        public static readonly Quaternion OrientationNormalBackward = new Quaternion(-0.5f, 0.5f, -0.5f, 0.5f);
        public static readonly Quaternion OrientationNormalLeft = new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f);
        #endregion //VisUpdateRotationByOrientationInterpolation

        #region VisUpdateRotationByForwardVectorInterpolation
        public static readonly Vector3 ForwardForNormalUpDown = Vector3.forward;
        public static readonly Vector3 ForwardForNormalLeftRight = Vector3.up;
        public static readonly Vector3 ForwardForNormalForwardBackward = Vector3.right;

        //-ZyKa Interpolating this with UpVector = (1, 1, 1) is a good example why Linear Matrix Interpolation doesn't work for rotations
        public void VisUpdateRotationByForwardVectorInterpolation()
        {
            transform.rotation =
                Quaternion.LookRotation(LerpInterpolationViaUpVector(LocalRotationAxis), LocalRotationAxis);
        }

        public Vector3 LerpInterpolationViaUpVector(Vector3 normalVector)
        {
            normalVector.Normalize();
            Vector3 interpolatedForward = ((normalVector.y * normalVector.y * ForwardForNormalUpDown) +
                                           (normalVector.x * normalVector.x * ForwardForNormalLeftRight) +
                                           (normalVector.z * normalVector.z * ForwardForNormalForwardBackward))
                .normalized;
            Vector3 orthogonalisedForward =
                Vector3.Cross(Vector3.Cross(normalVector, interpolatedForward), normalVector);
            return orthogonalisedForward;
        }
        #endregion //VisUpdateRotationByForwardVectorInterpolation
        
        #region VisUpdateRotationViaRotatingFromPreviousToCurrent
        //This requires the original setup to be perfect, which doesn't quite work out, because as soon as a little numerical error comes into play everything explodes
        private void VisUpdateRotationViaRotatingFromPreviousToCurrent()
        {
            VisUpdateRotationAxis();
            VisUpdateRotationForward(); 
        }
        
        private void VisUpdateRotationAxis()
        {
            Quaternion rotation = Quaternion.FromToRotation(transform.up, GlobalRotationAxis);
            transform.rotation = rotation * transform.rotation; 
        }

        private void VisUpdateRotationForward()
        {
            localForwardVector = localRotationAxis.GetOrthogonalisedVector(localForwardVector).normalized; 
            Quaternion rotation = Quaternion.FromToRotation(transform.forward, localForwardVector);
            transform.rotation = rotation * transform.rotation;
        }
        #endregion VisUpdateRotationViaRotatingFromPreviousToCurrent
    }
}