using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
public class EulerMatrixRotation : MonoBehaviour
{
    [SerializeField] private float3x3 yawMatrix; 
    [SerializeField] private float3x3 pitchMatrix; 
    [SerializeField] private float3x3 rollMatrix; 
    
    private Matrix4x4 _YawMatrix; 
    private Matrix4x4 _PitchMatrix; 
    private Matrix4x4 _RollMatrix;

    [SerializeField] private GameObject UnrotatedPoint; 
    [SerializeField] private GameObject UnrotatedAxis; 
    [SerializeField] private GameObject RotatedPoint; 
    [SerializeField] private GameObject RotatedAxis; 
    
    [SerializeField] private float YawAngle;
    [SerializeField] private float PitchAngle;
    [SerializeField] private float RollAngle;

    [ContextMenu("CalculateMatrices")]
    private void CalculateMatrices()
    {
        _YawMatrix = Matrix4x4.Rotate(Quaternion.Euler(RollAngle, 0, 0));
        _PitchMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, YawAngle, 0));
        _RollMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, PitchAngle));
        
        yawMatrix = _YawMatrix.ToFloat3x3();
        pitchMatrix = _PitchMatrix.ToFloat3x3();
        rollMatrix = _RollMatrix.ToFloat3x3();
    }

    [ContextMenu("RotatePoint")]
    private void RotatePoint()
    {
        CalculateMatrices();

        if (UnrotatedPoint is null || RotatedPoint is null)
        {
            Debug.LogWarning(name + ": can't rotate, because References are not set");
            return; 
        }
        
        Vector3 position = UnrotatedPoint.transform.position; 
        position = _YawMatrix.MultiplyVector(position); 
        position = _PitchMatrix.MultiplyVector(position); 
        position = _RollMatrix.MultiplyVector(position);
        RotatedPoint.transform.position = position;
        RotatedAxis.transform.eulerAngles = UnrotatedAxis.transform.eulerAngles + new Vector3(RollAngle, YawAngle, PitchAngle); 
    }

    private void Update()
    {
        RotatePoint();         
    }

    private void OnValidate()
    {
        //RotatePoint(); 
    }
}
