using UnityEngine;

namespace MathVisualisation
{
    public class Vis_Axis : MonoBehaviour
    {
        [SerializeField] private Vector3 value; 
        
        public Vector3 Value
        {
            get => value;
            set
            {
                SetDirectionFromCoordinates(value);
                SetScaling(value.magnitude);
                this.value = value;
            }
        }

        public Color Color
        {
            get => GetComponent<M_VectorColour>().Color;
            set => GetComponent<M_VectorColour>().Color = value;
        }

        [System.Serializable]
        public class AxisScalingData
        {
            public float scalingLowerClamp; 
            public float scalingUpperClamp;
            
            public float minThickness;
            public float maxThickness;
        }
        [SerializeField] private AxisScalingData _scalingData;
        
        public float Thickness
        {
            get => transform.localScale.y;
            set => transform.localScale = new Vector3(transform.localScale.x, value, value);
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

        public void SetScaling(float alpha)
        {
            if (Mathf.Abs(_scalingData.scalingUpperClamp) + Mathf.Abs(_scalingData.scalingLowerClamp) > 0.001)
            {
                Thickness = Mathf.Lerp(_scalingData.minThickness, _scalingData.maxThickness,
                    (alpha - _scalingData.scalingLowerClamp) / (_scalingData.scalingUpperClamp - _scalingData.scalingLowerClamp));
            }
        }

        private void OnValidate()
        {
            Value = value; 
        }
    }
}
