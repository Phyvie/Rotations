using RotContainers;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_EulerBezier", menuName = "Scriptable Objects/InterpolationSettings/EulerBezier")]
public class IS_EulerBezier : InterpolationSettings
{
    [SerializeField] private RotParams_EulerAngles a;
    [SerializeField] private RotParams_EulerAngles b;
    [SerializeField] private RotParams_EulerAngles outA;
    [SerializeField] private RotParams_EulerAngles inB;

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_EulerAngles.BezierCurve(a, b, outA, inB, t);
    }
}