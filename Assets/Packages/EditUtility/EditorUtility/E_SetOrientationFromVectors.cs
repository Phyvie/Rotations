using UnityEngine;

public class SetOrientationFromVectors : MonoBehaviour
{
    [SerializeField] private Vector3 upVector;
    [SerializeField] private Vector3 forwardVector; 
    
    [ContextMenu("Set Orientation")]
    private void SetOrientation()
    {
        forwardVector = Vector3.ProjectOnPlane(forwardVector, upVector).normalized;
        transform.rotation = Quaternion.LookRotation(forwardVector, upVector);
    }
}
