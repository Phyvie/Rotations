using RotContainers; 
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_AxisAngleLerp", menuName = "Scriptable Objects/InterpolationSettings/AxisAngleLerp")]
public class IS_AxisAngleLerp : InterpolationSettings
{
    [SerializeField] private RotParams_AxisAngle a;
    [SerializeField] private RotParams_AxisAngle b;

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_AxisAngle.LerpAxisAngle(a, b, t);
    }
}
