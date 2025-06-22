using RotContainers.ScreenshotSettings;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "SIS_EulerLerp", menuName = "Scriptable Objects/ScreenshotInterpolationSettings/EulerLerp")]
public class SIS_EulerLerp : ScreenshotInterpolationSettings
{
    [SerializeField] private RotParams_EulerAngles a;
    [SerializeField] private RotParams_EulerAngles b;

    public override RotParams.RotParams Interpolate(float t)
    {
        return RotParams_EulerAngles.Lerp(a, b, t);
    }
}