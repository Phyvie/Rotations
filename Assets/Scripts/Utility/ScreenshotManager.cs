using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RotContainers;
using RotParams;
using UnityEngine;

namespace Utility
{
    public class ScreenshotManager : MonoBehaviour
    {
        [SerializeField] private GeneralRotationContainer generalRotationContainer;
        [SerializeField] private ScreenshotSettings screenshotSettings;
        [SerializeField] private InterpolationSettings interpolationSettings; 
        public Camera screenshotCamera => generalRotationContainer.VisCamera;

        [Serializable]
        public class ViewShot
        {
            public RenderTexture renderTexture; 
            public RotParams_Base rotParams;
            public string name;

            public ViewShot(RenderTexture renderTexture, RotParams_Base rotParams, string name = null)
            {
                this.renderTexture = renderTexture;
                this.rotParams = rotParams;
                this.name = String.IsNullOrEmpty(name) ? rotParams.ToString() : name;
            }
        }
        [SerializeField] private List<ViewShot> viewShots = new List<ViewShot>(); 
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                TakeAndSaveSingleViewshot();
            }
            
            if (Input.GetKeyDown(KeyCode.F9))
            {
                TakeViewshot();
            }
            
            if (Input.GetKeyDown(KeyCode.F10))
            {
                SaveViewshots();
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                ResetViewshots();
            }
            
            if (Input.GetKeyDown(KeyCode.F12))
            {
                StartScreenShotInterpolation();
            }
        }

        [ContextMenu("TakeViewshot")]
        private void TakeViewshot()
        {
            Rect screenshotCameraRectBuffer = screenshotCamera.rect;
            screenshotCamera.rect = new Rect(0, 0, 1, 1); 
            
            RenderTexture screenTexture = new RenderTexture(screenshotSettings.imageWidth, screenshotSettings.imageWidth, 16);
            screenshotCamera.targetTexture = screenTexture;
            RenderTexture.active = screenTexture;
            screenshotCamera.Render();
            
            viewShots.Add(new ViewShot(screenTexture, generalRotationContainer.RotParams));
            
            screenshotCamera.rect = screenshotCameraRectBuffer;
            screenshotCamera.targetTexture = null;
        }

        [ContextMenu("SaveViewshots")]
        private void SaveViewshots()
        {
            int imageWidth = screenshotSettings.imageWidth;
            int imageHeight = screenshotSettings.imageHeight;
            int imageWidthOffset = screenshotSettings.imageWidthOffset;
            int imageHeightOffset = screenshotSettings.imageHeightOffset;

            int totalImages = viewShots.Count;
            int columns = screenshotSettings.columns;
            int rows = Mathf.CeilToInt((float)totalImages / columns);

            int totalWidth = columns * imageWidth + (columns - 1) * imageWidthOffset;
            int totalHeight = rows * imageHeight + (rows - 1) * imageHeightOffset;
            
            Texture2D finalTexture = new Texture2D(totalWidth, totalHeight, TextureFormat.RGBA32, false);
            for (int i = 0; i < totalImages; i++)
            {
                Texture2D singleFrame = new Texture2D(imageWidth, imageHeight, TextureFormat.RGBA32, false);
                
                RenderTexture.active = viewShots[i].renderTexture;
                singleFrame.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
                singleFrame.Apply();
                
                int col = i % columns;
                int row = i / columns;
                int x = col * (imageWidth + imageWidthOffset);
                int y = (rows - 1 - row) * (imageHeight + imageHeightOffset);

                finalTexture.SetPixels(x, y, imageWidth, imageHeight, singleFrame.GetPixels());
                
                byte[] singleShotByteArray = singleFrame.EncodeToPNG();
                System.IO.File.WriteAllBytes($"{Path.Combine(screenshotSettings.path, viewShots[i].name)}.png", singleShotByteArray);
                
                Destroy(singleFrame);
            }
            
            byte[] byteArray = finalTexture.EncodeToPNG();
            string timestamp = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
            System.IO.File.WriteAllBytes($"{Path.Combine(screenshotSettings.path, timestamp)}.png", byteArray);
            
            RenderTexture.active = null; 
        }

        [ContextMenu("ResetViewshots")]
        private void ResetViewshots()
        {
            for (int i = 0; i < viewShots.Count; i++)
            {
                viewShots[i].renderTexture.Release();
                Destroy(viewShots[i].renderTexture);
            }
            viewShots.Clear();
        }
        
        [ContextMenu("TakeAndSaveCameraViewshot")]
        private void TakeAndSaveSingleViewshot()
        {
            string timestamp = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");

            Rect screenshotCameraRectBuffer = screenshotCamera.rect;
            screenshotCamera.rect = new Rect(0, 0, 1, 1); 
            RenderTexture screenTexture = new RenderTexture(screenshotSettings.imageWidth, screenshotSettings.imageHeight, 16);
            screenshotCamera.targetTexture = screenTexture;
            RenderTexture.active = screenTexture;
            screenshotCamera.Render();
            screenshotCamera.rect = screenshotCameraRectBuffer;

            Texture2D renderedTexture = new Texture2D(screenshotSettings.imageWidth, screenshotSettings.imageHeight, TextureFormat.RGBA32, false);
            renderedTexture.ReadPixels(new Rect(0, 0, screenshotSettings.imageWidth, screenshotSettings.imageHeight), 0, 0);
            RenderTexture.active = null;

            byte[] byteArray = renderedTexture.EncodeToPNG();
            File.WriteAllBytes($"{Path.Combine(screenshotSettings.path, generalRotationContainer.RotParams.ToString())}.png", byteArray);
        
            Destroy(screenTexture);
            Destroy(renderedTexture);
            
            screenshotCamera.targetTexture = null;
        }

        private Coroutine cor_Interpolation; 
        [ContextMenu("StartScreenShotInterpolation")]
        private void StartScreenShotInterpolation()
        {
            if (cor_Interpolation == null)
            {
                cor_Interpolation = StartCoroutine(ScreenShotInterpolation(interpolationSettings));
            }
        }
    
        private IEnumerator ScreenShotInterpolation(InterpolationSettings interpolationSettings)
        {
            ResetViewshots(); 
            for (int i = 0; i < interpolationSettings.InterpolationCount; i++)
            {
                generalRotationContainer.RotParams = interpolationSettings.Interpolate(interpolationSettings.getInterpolationAlpha(i));
                yield return new WaitForEndOfFrame();
                
                TakeViewshot();
                yield return new WaitForEndOfFrame();
            }
            SaveViewshots();
            cor_Interpolation = null; 
        }

        [SerializeField] private int interpolationStep; 
        [ContextMenu("SetToNextInterpolationStep")]
        private void SetToInterpolationStep()
        {
            float alpha = interpolationSettings.getInterpolationAlpha(interpolationStep); 
            generalRotationContainer.RotParams = interpolationSettings.Interpolate(alpha);
        }
    }
}
