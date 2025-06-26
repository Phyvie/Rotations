using RotationVisualisation;
using RotParams;
using UnityEngine;

public class QuaternionFunctionTest : MonoBehaviour
{
    [SerializeField] private RotVis_Quaternion rotVis_Quaternion;
    RotParams_Quaternion quat => (RotParams_Quaternion)rotVis_Quaternion.GetRotParams();
    
    //!ZyKa Debug
    [SerializeField] private float SetAngle = 0;
    [ContextMenu("Set Angle")]
    private void ZyKaSetAngle()
    {
        quat.SignedAngle = SetAngle; 
    }

    [ContextMenu("Get Angle")]
    private void ZyKaGetAngle()
    {
        SetAngle = quat.SignedAngle;
    }
    
    [SerializeField] private Vector3 SetAxis = Vector3.zero;
    [ContextMenu("Set Axis")]
    private void ZyKaSetAxis()
    {
        quat.NormalizedAxis = SetAxis;
    }

    [ContextMenu("Get Axis")]
    private void ZyKaGetAxis()
    {
        SetAxis = quat.NormalizedAxis;
    }
}
