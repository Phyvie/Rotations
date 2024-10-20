using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]
public class UnrotatedAxis : MonoBehaviour
{
    private Vector3 _lastRotation;
    private Vector3 _lastPosition;

    [SerializeField] private GameObject UnrotatedPoint; 
    
    private void Update()
    {
        if (_lastRotation != transform.rotation.eulerAngles)
        {
            UnrotatedPoint.transform.position = transform.right;
            _lastRotation = transform.rotation.eulerAngles;
            _lastPosition = UnrotatedPoint.transform.position; 
        }
        else if (_lastPosition != UnrotatedPoint.transform.position)
        {
            UnrotatedPoint.transform.position = UnrotatedPoint.transform.position.normalized;
            transform.LookAt(UnrotatedPoint.transform.position);
            _lastPosition = UnrotatedPoint.transform.position;
            _lastRotation = transform.eulerAngles; 
        }
    }
}
