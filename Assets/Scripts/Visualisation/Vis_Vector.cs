using UnityEngine;

namespace Visualisation
{
    [SelectionBase]
    public class Vis_Vector : MonoBehaviour
    {
        [SerializeField] private Vector3 value; 
        [SerializeField] private GameObject vectorLength; 
        [SerializeField] private GameObject vectorHead;

        public Vector3 Value
        {
            get => value;
            set
            {
                SetLength(value.magnitude); 
                SetDirectionFromCoordinates(value);
                this.value = value;
            }
        }

        public void SetLength(float length)
        {
            vectorLength.transform.localScale = new Vector3(length, 1, 1); 
            vectorHead.transform.localPosition = new Vector3(length, 0, 0);
        }

        public void SetDirectionFromQuaternion(Quaternion rotation)
        {
            transform.rotation = rotation; 
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

        private void OnValidate()
        {
            Value = value; 
        }
    }
}