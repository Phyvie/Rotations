using RotContainers.ScreenshotSettings;
using RotParams;
using UnityEngine;

[CreateAssetMenu(fileName = "SIS_MatrixLerpThenToRotationByAxes", menuName = "Scriptable Objects/ScreenshotInterpolationSettings/MatrixLerpThenToRotationByAxes")]
public class SIS_MatrixLerpThenToRotationByAxes : ScreenshotInterpolationSettings
{
    [SerializeField] private RotParams_Matrix from;
    [SerializeField] private RotParams_Matrix to;
    [SerializeField] private int primaryAxisIndex = 0;
    [SerializeField] private int secondaryAxisIndex = 1;

    public override RotParams.RotParams Interpolate(float t)
    {
        return RotParams_Matrix.LerpThenToRotationByAxes(from, to, t, primaryAxisIndex, secondaryAxisIndex);
    }
}