using RotContainers; 
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_QuaternionLogInterpolation", menuName = "Scriptable Objects/InterpolationSettings/QuaternionLogInterpolation")]
public class IS_QuaternionLogarithmicInterpolation : InterpolationSettings
{
    [SerializeField] private RotParams_Quaternion q0;
    [SerializeField] private RotParams_Quaternion q1;
    [SerializeField] private bool shortestPathCorrection = false;

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_Quaternion.LogarithmicInterpolation(q0, q1, t, shortestPathCorrection);
    }
}