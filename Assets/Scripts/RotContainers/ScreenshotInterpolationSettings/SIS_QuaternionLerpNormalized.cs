using RotContainers; 
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_QuaternionLerpNormalized", menuName = "Scriptable Objects/InterpolationSettings/QuaternionLerpNormalized")]
public class IS_QuaternionLerpNormalized : InterpolationSettings
{
    [SerializeField] private RotParams_Quaternion q0;
    [SerializeField] private RotParams_Quaternion q1;
    [SerializeField] private bool shortestPathCorrection = false;

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_Quaternion.LerpNormalised(q0, q1, t, shortestPathCorrection);
    }
}