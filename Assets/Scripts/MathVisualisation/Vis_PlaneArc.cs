using System;
using MathExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Visualisation
{
    public class Vis_PlaneArc : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float beginAngle;
        [SerializeField] private float endingAngle;
        [FormerlySerializedAs("_positiveAngleColor")] [SerializeField] private Color _positiveAngleColorOverwrite;
        [FormerlySerializedAs("_negativeAngleColor")] [SerializeField] private Color _negativeAngleColorOverwrite;
        [SerializeField] private M_CircleSector torusMaterial;
        [SerializeField] private M_CircleSector circleMaterial;
        #endregion Variables
        
        #region Properties
        public Vector3 LocalUpAxis
        {
            get => transform.parent is not null ? transform.parent.InverseTransformDirection(transform.up) : transform.up;
            set
            {
                GlobalUpAxis = transform.parent is not null ? transform.parent.TransformDirection(value) : value;
            }
        }

        public Vector3 GlobalUpAxis
        {
            get => transform.up;
            set 
            {
                Quaternion rotation = Quaternion.FromToRotation(GlobalUpAxis, value);
                transform.rotation = rotation * transform.rotation;
                AutoAdjustForwardAxisForUpAxis();
                VisUpdateShader();
            }
        }
        
        public Vector3 LocalForwardAxis
        {
            get => transform.parent != null ? transform.parent.InverseTransformDirection(transform.forward) : transform.forward;
            set => GlobalForwardAxis = transform.parent != null ? transform.parent.TransformDirection(value) : value;
        }

        public Vector3 GlobalForwardAxis
        {
            get => transform.forward;
            set
            {
                if (value == Vector3.zero) return;
                if (Vector3.Angle(transform.forward, value) < 0.001f) return;

                Quaternion rotation = Quaternion.FromToRotation(transform.forward, value);
                transform.rotation = rotation * transform.rotation;
                VisUpdateShader();
            }
        }
        
        public float BeginAngle
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
        
        public Color PositiveAngleColor
        {
            get => _positiveAngleColorOverwrite;
            set
            {
                _positiveAngleColorOverwrite = value;
                if (torusMaterial != null)
                {
                    torusMaterial.PositiveAngleColor = value;
                }
                if (circleMaterial != null)
                {
                    circleMaterial.PositiveAngleColor = value;
                }
                VisUpdateShader();
            }
        }

        public Color NegativeAngleColor
        {
            get => _negativeAngleColorOverwrite;
            set
            {
                _negativeAngleColorOverwrite = value;
                if (torusMaterial != null)
                {
                    torusMaterial.NegativeAngleColor = value;
                }
                if (circleMaterial != null)
                {
                    circleMaterial.NegativeAngleColor = value;
                }
                VisUpdateShader();
            }
        }

        public Color FullRotationsColor
        {
            get => (endingAngle - beginAngle) > 0 ? PositiveAngleColor : NegativeAngleColor;
        }
        #endregion Properties

        #region ScreenshotUtility & Update
        [SerializeField] private float manualSelfRotationSpeed = 5.0f; 
        public void Update()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                RotateVisualisation(Time.deltaTime * manualSelfRotationSpeed);
            }

            if (Input.GetKey(KeyCode.E))
            {
                RotateVisualisation(Time.deltaTime * -manualSelfRotationSpeed);
            }
        }

        private void RotateVisualisation(float amount)
        {
            GlobalForwardAxis = Quaternion.AngleAxis(amount, GlobalUpAxis) * GlobalForwardAxis;
            /*Implemented but not tested
            if (canSnap && isSnapped)
            {
                overSnapRotation += amount;
                CheckForUnsnap();
            }
            else
            {
                GlobalForwardAxis = Quaternion.AngleAxis(amount, GlobalUpAxis) * GlobalForwardAxis;
                if (canSnap)
                {
                    CheckForSnap();
                }
            }
            */
        }
        
        #endregion ScreenshotUtility

        public void VisUpdate()
        {
            VisUpdateShader(); 
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
                circleMaterial.FullRotationColor = FullRotationsColor;
            }
        }

        private void AutoAdjustForwardAxisForUpAxis()
        {
            if (Mathf.Abs(GlobalUpAxis.x) < 0.1 || Mathf.Abs(GlobalUpAxis.y) < 0.1 || Mathf.Abs(GlobalUpAxis.z) < 0.1)
            {
                Vector3 rightCross = Vector3.Cross(GlobalUpAxis, GlobalUpAxis.CyclicAxisRotation());
                GlobalForwardAxis = Vector3.Cross(rightCross, GlobalUpAxis); 
            }
        }
        
        public void OnValidate()
        {
            VisUpdate();
        }
        
        #region Deprecated
        /*
        #region VisUpdateRotationByOrientationInterpolation
        //Good example why Rotation Vector Lerp is not ideal
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

            orientations[0] = LocalUpAxis.x >= 0 ? OrientationNormalRight : OrientationNormalLeft;
            weights[0] = LocalUpAxis.x * LocalUpAxis.x;

            orientations[1] = LocalUpAxis.y >= 0 ? OrientationNormalUp : OrientationNormalDown;
            weights[1] = LocalUpAxis.y * LocalUpAxis.y;

            orientations[2] = LocalUpAxis.z >= 0 ? OrientationNormalForward : OrientationNormalBackward;
            weights[2] = LocalUpAxis.z * LocalUpAxis.z;

            transform.localRotation =
                MathFunctions.QuaternionInterpolateAsRotationVectors(orientations, weights);
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

        //Interpolating this with UpVector = (1, 1, 1) is a good example why Linear Matrix Interpolation doesn't work for rotations
        public void VisUpdateRotationByForwardVectorInterpolation()
        {
            transform.rotation =
                Quaternion.LookRotation(LerpInterpolationViaUpVector(LocalUpAxis), LocalUpAxis);
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
        private void VisUpdateRotationForward()
        {
            LocalForwardAxis = LocalForwardAxis.GetOrthogonalisedVector(LocalForwardAxis).normalized; 
            Quaternion rotation = Quaternion.FromToRotation(transform.forward, LocalForwardAxis);
            transform.rotation = rotation * transform.rotation;
        }
        #endregion VisUpdateRotationViaRotatingFromPreviousToCurrent
        
        #region SnapForwardVector
        private bool isSnapped;
        private float overSnapRotation; 
        private void CheckForSnap(float tolerance = 0.1f)
        {
            throw new NotImplementedException("Implemented but not tested"); 
            if (Mathf.Approximately(GlobalForwardAxis.x, 1))
            {
                GlobalForwardAxis = GlobalUpAxis.GetOrthogonalWithSpecifiedXValue(1);
                isSnapped = true;
            }
            if (Mathf.Approximately(GlobalForwardAxis.x, -1))
            {
                GlobalForwardAxis = GlobalUpAxis.GetOrthogonalWithSpecifiedXValue(-1);
                isSnapped = true;
            }

            if (Mathf.Approximately(GlobalForwardAxis.y, 1))
            {
                GlobalForwardAxis = GlobalUpAxis.GetOrthogonalWithSpecifiedYValue(1);
                isSnapped = true;
            }
            if (Mathf.Approximately(GlobalForwardAxis.y, -1))
            {
                GlobalForwardAxis = GlobalUpAxis.GetOrthogonalWithSpecifiedYValue(-1);
                isSnapped = true;
            }

            if (Mathf.Approximately(GlobalForwardAxis.z, 1))
            {
                GlobalForwardAxis = GlobalUpAxis.GetOrthogonalWithSpecifiedZValue(1);
                isSnapped = true;
            }
            if (Mathf.Approximately(GlobalForwardAxis.z, -1))
            {
                GlobalForwardAxis = GlobalUpAxis.GetOrthogonalWithSpecifiedZValue(-1);
                isSnapped = true;
            }
        }
        
        private void CheckForUnsnap(float tolerance = 0.1f)
        {
            throw new NotImplementedException("Implemented but not tested"); 
            if (Mathf.Abs(overSnapRotation) > tolerance)
            {
                GlobalForwardAxis = Quaternion.AngleAxis(overSnapRotation, GlobalUpAxis) * GlobalForwardAxis;
                overSnapRotation = 0; 
                isSnapped = false; 
            }
        }
        #endregion SnapForwardVector
        */
        #endregion Deprecated
    }
}