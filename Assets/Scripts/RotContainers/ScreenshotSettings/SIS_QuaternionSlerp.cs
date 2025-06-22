using RotContainers.ScreenshotSettings;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "SIS_QuaternionSlerp", menuName = "Scriptable Objects/ScreenshotInterpolationSettings/QuaternionSlerp")]
public class SIS_QuaternionSlerp : ScreenshotInterpolationSettings
{
    [SerializeField] private RotParams_Quaternion q0;
    [SerializeField] private RotParams_Quaternion q1;
    [SerializeField] private bool shortestPathCorrection = false;

    public override RotParams.RotParams Interpolate(float t)
    {
        return RotParams_Quaternion.Slerp(q0, q1, t, shortestPathCorrection);
    }
}