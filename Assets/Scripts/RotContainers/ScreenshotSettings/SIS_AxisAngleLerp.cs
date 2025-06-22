using RotContainers.ScreenshotSettings;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "SIS_AxisAngleLerp", menuName = "Scriptable Objects/ScreenshotInterpolationSettings/AxisAngleLerp")]
public class SIS_AxisAngleLerp : ScreenshotInterpolationSettings
{
    [SerializeField] private RotParams_AxisAngle a;
    [SerializeField] private RotParams_AxisAngle b;

    public override RotParams.RotParams Interpolate(float t)
    {
        return RotParams_AxisAngle.LerpAxisAngle(a, b, t);
    }
}
