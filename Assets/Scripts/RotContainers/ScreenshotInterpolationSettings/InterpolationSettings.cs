using RotParams;
using UnityEngine;

namespace RotContainers
{
    public abstract class InterpolationSettings : ScriptableObject
    {
        [Header("Interpolation Settings")] 
        public bool useLinearAlphaDistance = false;
        private int interpolationCount = 5; 
        [SerializeField] private float[] interpolationAlphas = { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f };

        public float getInterpolationAlpha(int i) =>
            useLinearAlphaDistance ? (float) i / interpolationCount : interpolationAlphas[i]; 
        public int InterpolationCount => useLinearAlphaDistance ? interpolationCount : interpolationAlphas.Length;
        
        public abstract RotParams_Base Interpolate(float alpha);
    }
}