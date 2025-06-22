using UnityEngine;

namespace RotContainers
{
    [CreateAssetMenu(fileName = "ScreenshotSettings", menuName = "Scriptable Objects/ScreenshotSettings", order = 0)]
    public class ScreenshotSettings : ScriptableObject
    {
        [Header("Output Settings")]
        public string path = "D:/CGL/Bachelor-Thesis/RotationImages/";
        public int columns = 4; 
        public int imageWidth = 1000;
        public int imageHeight = 1000;
        public int imageWidthOffset = 20;
        public int imageHeightOffset = 20;


    }
}