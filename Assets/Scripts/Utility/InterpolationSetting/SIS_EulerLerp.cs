using RotContainers;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_EulerLerp", menuName = "Scriptable Objects/InterpolationSettings/EulerLerp")]
public class IS_EulerLerp : InterpolationSettings
{
    [SerializeField] private RotParams_EulerAngles a;
    [SerializeField] private RotParams_EulerAngles b;

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_EulerAngles.Lerp(a, b, t);
    }
}