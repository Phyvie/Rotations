using RotParams;
using UnityEngine;
using UnityEngine.Serialization;

namespace RotContainers
{
    public abstract class InterpolationSettings : ScriptableObject
    {
        [Header("Interpolation Settings")] 
        public bool useLinearAlphaDistance = false;
        [SerializeField] private float[] interpolationAlphas = { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f };
        [SerializeField] private float interpolationTime;
        [SerializeField] public bool pingPongInterpolation;
        [SerializeField] public float orientationHoldTime = 1.0f;

        [SerializeField] public bool visPath = false; 
        
        public float InterpolationStepTime
        {
            get => interpolationTime / InterpolationCount; 
            set => interpolationTime = value * InterpolationCount;
        }
        
        public float GetInterpolationAlpha(int i) =>
            useLinearAlphaDistance ? (float) i / (InterpolationCount-1) : interpolationAlphas[i]; 
        public int InterpolationCount => interpolationAlphas.Length;

        public abstract RotParams_Base Interpolate(float alpha);
    }
}