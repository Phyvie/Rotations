using MathExtensions;
using Unity.Mathematics;
using UnityEngine;

public class _Test : MonoBehaviour
{
    private void Awake()
    {
        // Matrix4x4 matrix = Matrix4x4.LookAt(
        //     new Vector3(0, -1, 1), 
        //     new Vector3(-1, 1, 0), 
        //     Vector3.up);
        // float3x3 floatMatrix = matrix.ToFloat3x3();
        //
        // Quaternion fromMatrix = matrix.ToQuaternion();
        // Quaternion altFromMatrix = floatMatrix.ToQuaternion();
        // Quaternion trueFromMatrix =
        //     Quaternion.LookRotation(
        //         matrix.GetColumn(0),
        //         matrix.GetColumn(1)
        //     ); 
        //
        // Vector3 eulerAngles = altFromMatrix.eulerAngles;
        // Vector3 rotationVector = altFromMatrix.ToRotationVector(); 
    }
}
