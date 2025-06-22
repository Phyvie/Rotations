using RotContainers;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_AxisAngleBezier", menuName = "Scriptable Objects/InterpolationSettings/AxisAngleBezier")]
public class IS_AxisAngleBezier : InterpolationSettings
{
    [SerializeField] private RotParams_AxisAngle a;
    [SerializeField] private RotParams_AxisAngle b;
    [SerializeField] private RotParams_AxisAngle outA;
    [SerializeField] private RotParams_AxisAngle inB;

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_AxisAngle.BezierCurve(a, b, outA, inB, t);
    }
}