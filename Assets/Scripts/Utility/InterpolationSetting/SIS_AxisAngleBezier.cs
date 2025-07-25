using RotContainers;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_AxisAngleBezier", menuName = "Scriptable Objects/InterpolationSettings/AxisAngleBezier")]
public class IS_AxisAngleBezier : InterpolationSettings
{
    [SerializeField] private RotParams_AxisAngle a = new RotParams_AxisAngle();
    [SerializeField] private RotParams_AxisAngle b = new RotParams_AxisAngle();
    [SerializeField] private RotParams_AxisAngle outA = new RotParams_AxisAngle();
    [SerializeField] private RotParams_AxisAngle inB = new RotParams_AxisAngle();

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_AxisAngle.BezierCurve(a, b, outA, inB, t);
    }
}