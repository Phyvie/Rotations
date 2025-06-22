using RotContainers.ScreenshotSettings;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "SIS_QuaternionLerpNormalized", menuName = "Scriptable Objects/ScreenshotInterpolationSettings/QuaternionLerpNormalized")]
public class SIS_QuaternionLerpNormalized : ScreenshotInterpolationSettings
{
    [SerializeField] private RotParams_Quaternion q0;
    [SerializeField] private RotParams_Quaternion q1;
    [SerializeField] private bool shortestPathCorrection = false;

    public override RotParams.RotParams Interpolate(float t)
    {
        return RotParams_Quaternion.LerpNormalised(q0, q1, t, shortestPathCorrection);
    }
}