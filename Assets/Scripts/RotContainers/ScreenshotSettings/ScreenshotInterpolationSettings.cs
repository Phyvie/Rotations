using UnityEngine;

namespace Editor
{
    public abstract class ScreenshotInterpolationSettings : ScriptableObject
    {
        [Header("Output Settings")]
        public string path = "D:/CGL/Bachelor-Thesis/RotationImages/";
        public int imageWidth = 1000;
        public int imageHeight = 1000;
        public int imageWidthOffset = 20;
        public int imageHeightOffset = 20;

        [Header("Interpolation Settings")]
        public float[] interpolationAlphas = { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f };

        // Abstract method to perform interpolation logic specific to subclass
        // 't' is the interpolation parameter between 0 and 1
        public abstract void Interpolate(ref RotParams.RotParams rotParams, float t);
    }
}