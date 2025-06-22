using RotContainers.ScreenshotSettings;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "SIS_EulerBezier", menuName = "Scriptable Objects/ScreenshotInterpolationSettings/EulerBezier")]
public class SIS_EulerBezier : ScreenshotInterpolationSettings
{
    [SerializeField] private RotParams_EulerAngles a;
    [SerializeField] private RotParams_EulerAngles b;
    [SerializeField] private RotParams_EulerAngles outA;
    [SerializeField] private RotParams_EulerAngles inB;

    public override RotParams.RotParams Interpolate(float t)
    {
        return RotParams_EulerAngles.BezierCurve(a, b, outA, inB, t);
    }
}