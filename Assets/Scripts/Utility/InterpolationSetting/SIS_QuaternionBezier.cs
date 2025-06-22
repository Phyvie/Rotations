using RotContainers;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_QuaternionBezier", menuName = "Scriptable Objects/InterpolationSettings/QuaternionBezier")]
public class IS_QuaternionBezier : InterpolationSettings
{
    [SerializeField] private RotParams_Quaternion q0;
    [SerializeField] private RotParams_Quaternion q1;
    [SerializeField] private RotParams_Quaternion outQ0;
    [SerializeField] private RotParams_Quaternion inQ1;

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_Quaternion.BezierCurve(q0, q1, outQ0, inQ1, t);
    }
}