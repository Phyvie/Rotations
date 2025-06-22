using RotContainers.ScreenshotSettings;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "SIS_QuaternionLogInterpolation", menuName = "Scriptable Objects/ScreenshotInterpolationSettings/QuaternionLogInterpolation")]
public class SIS_QuaternionLogarithmicInterpolation : ScreenshotInterpolationSettings
{
    [SerializeField] private RotParams_Quaternion q0;
    [SerializeField] private RotParams_Quaternion q1;
    [SerializeField] private bool shortestPathCorrection = false;

    public override RotParams.RotParams Interpolate(float t)
    {
        return RotParams_Quaternion.LogarithmicInterpolation(q0, q1, t, shortestPathCorrection);
    }
}