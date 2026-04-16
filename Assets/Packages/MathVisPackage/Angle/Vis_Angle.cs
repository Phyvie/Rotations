using System;
using MathExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Visualisation
{
    public class Vis_Angle : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float beginAngle;
        [SerializeField] private float endingAngle;
        [FormerlySerializedAs("_positiveAngleColor")] [SerializeField] private Color _positiveAngleColorOverwrite;
        [FormerlySerializedAs("_negativeAngleColor")] [SerializeField] private Color _negativeAngleColorOverwrite;
        [SerializeField] private M_CircleSector torusMaterial;
        [SerializeField] private M_CircleSector circleMaterial;

        [SerializeField] private bool autoAdjustForwardForUp = true; 
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
            if (!autoAdjustForwardForUp)
            {
                return; 
            }
            if (Vector3.Dot(GlobalUpAxis, Vector3.forward) > 0.9 || Vector3.Dot(GlobalUpAxis, Vector3.up) > 0.9 || Vector3.Dot(GlobalUpAxis, Vector3.right) > 0.9)
            {
                Vector3 rightCross = Vector3.Cross(GlobalUpAxis, GlobalUpAxis.CyclicAxisRotation());
                GlobalForwardAxis = Vector3.Cross(rightCross, GlobalUpAxis); 
            }
        }
        
        public void OnValidate()
        {
            VisUpdate();
            PositiveAngleColor = _positiveAngleColorOverwrite; 
            NegativeAngleColor = _negativeAngleColorOverwrite;
        }
    }
}