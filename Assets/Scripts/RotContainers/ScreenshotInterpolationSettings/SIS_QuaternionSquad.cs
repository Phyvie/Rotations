using RotContainers; 
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_QuaternionSquad", menuName = "Scriptable Objects/InterpolationSettings/QuaternionSquad")]
public class IS_QuaternionSquad : InterpolationSettings
{
    [SerializeField] private RotParams_Quaternion q0;
    [SerializeField] private RotParams_Quaternion q1;
    [SerializeField] private RotParams_Quaternion outQ1;
    [SerializeField] private RotParams_Quaternion inQ2;

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_Quaternion.Squad(q0, q1, outQ1, inQ2, t);
    }
}