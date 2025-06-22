using RotContainers.ScreenshotSettings;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "SIS_QuaternionSquad", menuName = "Scriptable Objects/ScreenshotInterpolationSettings/QuaternionSquad")]
public class SIS_QuaternionSquad : ScreenshotInterpolationSettings
{
    [SerializeField] private RotParams_Quaternion q0;
    [SerializeField] private RotParams_Quaternion q1;
    [SerializeField] private RotParams_Quaternion outQ1;
    [SerializeField] private RotParams_Quaternion inQ2;

    public override RotParams.RotParams Interpolate(float t)
    {
        return RotParams_Quaternion.Squad(q0, q1, outQ1, inQ2, t);
    }
}