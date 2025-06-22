using RotContainers; 
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_QuaternionSlerp", menuName = "Scriptable Objects/InterpolationSettings/QuaternionSlerp")]
public class IS_QuaternionSlerp : InterpolationSettings
{
    [SerializeField] private RotParams_Quaternion q0;
    [SerializeField] private RotParams_Quaternion q1;
    [SerializeField] private bool shortestPathCorrection = false;

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_Quaternion.Slerp(q0, q1, t, shortestPathCorrection);
    }
}