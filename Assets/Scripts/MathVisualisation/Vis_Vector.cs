using System;
using UnityEngine;

namespace Visualisation
{
    [SelectionBase]
    public class Vis_Vector : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Vector3 value; 
        [SerializeField] private GameObject vectorLength;
        [SerializeField] private GameObject vectorHead; 
        #endregion Variables
        
        #region Properties
        public Vector3 Value
        {
            get => value;
            set
            {
                SetLength(value.magnitude); 
                SetDirectionFromCoordinates(value);
                SetScaling(value.magnitude);
                this.value = value;
            }
        }

        public Color Color
        {
            get => vectorHead.GetComponent<M_VectorColour>().Color;
            set
            {
                vectorHead.GetComponent<M_VectorColour>().Color = value;
                vectorLength.GetComponent<M_VectorColour>().Color = value;
            }
        }
        #endregion Properties
        
        #region GetSetFunctions
        public void SetLength(float length)
        {
            vectorLength.transform.localScale = new Vector3(length * _scalingData.lengthScale, 1, 1); 
            vectorHead.transform.localPosition = new Vector3(length * _scalingData.lengthScale, 0, 0);
        }

        public void SetDirectionFromQuaternion(Quaternion rotation)
        {
            transform.localRotation = rotation; 
        }
        
        public void SetDirectionFromCoordinates(Vector3 directionCoordinates)
        {
            SetDirectionFromQuaternion(Quaternion.FromToRotation(Vector3.right, directionCoordinates));
        }
        
        public void SetDirectionFromCoordinates(float x, float y, float z)
        {
            SetDirectionFromCoordinates(new Vector3(x, y, z));
        }
        
        public void SetDirectionFromAngles(float azimuth, float elevation)
        {
            SetDirectionFromQuaternion(Quaternion.Euler(azimuth, elevation, 0)); 
        }

        public void SetDirectionFromAngles(float yaw, float pitch, float roll)
        {
            SetDirectionFromQuaternion(Quaternion.Euler(pitch, yaw, roll));
        }
        #endregion GetSetFunctions

        [ExecuteAlways]
        private void Update()
        {
            VisUpdate(); 
        }

        private void VisUpdate()
        {
            Value = value; 
        }

        #region VisualScaling
        [System.Serializable]
        public class VectorScalingData
        {
            public float lengthScale; 
            
            public float scalingLowerClamp; 
            public float scalingUpperClamp;
            
            public float minThickness;
            public float maxThickness;

            public Vector3 minHeadSize; 
            public Vector3 maxHeadSize;
        }
        [SerializeField] private VectorScalingData _scalingData;
        
        public Vector3 HeadSize
        {
            get => vectorHead.transform.localScale;
            set => vectorHead.transform.localScale = value;
        }
        
        public float LengthThickness
        {
            get => vectorLength.transform.localScale.y;
            set
            {
                vectorLength.transform.localScale = new Vector3(vectorLength.transform.localScale.x, value, value);
            }
        }
        
        public void SetScaling(float alpha)
        {
            if (Mathf.Abs(_scalingData.scalingUpperClamp) + Mathf.Abs(_scalingData.scalingLowerClamp) > 0.001)
            {
                LengthThickness = Mathf.Lerp(_scalingData.minThickness, _scalingData.maxThickness,
                    (alpha - _scalingData.scalingLowerClamp) / (_scalingData.scalingUpperClamp - _scalingData.scalingLowerClamp));
                HeadSize = Vector3.Lerp(_scalingData.minHeadSize, _scalingData.maxHeadSize,
                    (alpha - _scalingData.scalingLowerClamp) / (_scalingData.scalingUpperClamp - _scalingData.scalingLowerClamp)); 
            }
        }
        #endregion VisualScaling
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            Value = value; 
        }
        #endif
    }
}