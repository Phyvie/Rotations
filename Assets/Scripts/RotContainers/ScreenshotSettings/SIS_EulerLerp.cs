using RotParams;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(fileName = "SIS_EulerLerp", menuName = "ScriptableObject/ScreenshotInterpolationSettings")]
    public class SIS_EulerLerp : ScreenshotInterpolationSettings
    {
        [SerializeField] private RotParams_EulerAngles a;
        [SerializeField] private RotParams_EulerAngles b; 
        
        public override void Interpolate(ref RotParams.RotParams rotParams, float t)
        {
            rotParams = RotParams_EulerAngles.Lerp(a, b, t);
        }
    }
}