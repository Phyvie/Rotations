using RotContainers.ScreenshotSettings;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "SIS_AxisAngleBezier", menuName = "Scriptable Objects/ScreenshotInterpolationSettings/AxisAngleBezier")]
public class SIS_AxisAngleBezier : ScreenshotInterpolationSettings
{
    [SerializeField] private RotParams_AxisAngle a;
    [SerializeField] private RotParams_AxisAngle b;
    [SerializeField] private RotParams_AxisAngle outA;
    [SerializeField] private RotParams_AxisAngle inB;

    public override RotParams.RotParams Interpolate(float t)
    {
        return RotParams_AxisAngle.BezierCurve(a, b, outA, inB, t);
    }
}