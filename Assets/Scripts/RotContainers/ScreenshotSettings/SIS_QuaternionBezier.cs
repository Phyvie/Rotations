using RotContainers.ScreenshotSettings;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "SIS_QuaternionBezier", menuName = "Scriptable Objects/ScreenshotInterpolationSettings/QuaternionBezier")]
public class SIS_QuaternionBezier : ScreenshotInterpolationSettings
{
    [SerializeField] private RotParams_Quaternion q0;
    [SerializeField] private RotParams_Quaternion q1;
    [SerializeField] private RotParams_Quaternion outQ0;
    [SerializeField] private RotParams_Quaternion inQ1;

    public override RotParams.RotParams Interpolate(float t)
    {
        return RotParams_Quaternion.BezierCurve(q0, q1, outQ0, inQ1, t);
    }
}