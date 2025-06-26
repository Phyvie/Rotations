using RotationVisualisation;
using RotParams;
using UnityEngine;
using UnityEngine.Serialization;

public class QuaternionFunctionTest : MonoBehaviour
{
    [FormerlySerializedAs("rotVisBaseQuaternion")] [FormerlySerializedAs("rotVis_Quaternion")] [SerializeField] private RotVis_Quaternion rotVisQuaternion;
    RotParams_Quaternion quat => (RotParams_Quaternion)rotVisQuaternion.GetRotParams();
    
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
