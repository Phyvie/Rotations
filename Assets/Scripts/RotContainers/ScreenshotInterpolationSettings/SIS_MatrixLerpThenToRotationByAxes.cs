using RotContainers;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "IS_MatrixLerpThenToRotationByAxes", menuName = "Scriptable Objects/InterpolationSettings/MatrixLerpThenToRotationByAxes")]
public class IS_MatrixLerpThenToRotationByAxes : InterpolationSettings
{
    [SerializeField] private RotParams_Matrix from;
    [SerializeField] private RotParams_Matrix to;
    [SerializeField] private int primaryAxisIndex = 0;
    [SerializeField] private int secondaryAxisIndex = 1;

    public override RotParams_Base Interpolate(float t)
    {
        return RotParams_Matrix.LerpThenToRotationByAxes(from, to, t, primaryAxisIndex, secondaryAxisIndex);
    }
}