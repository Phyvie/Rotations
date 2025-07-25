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
        [SerializeField] private ComboRotCot comboRotCot;
        [SerializeField] private ScreenshotSettings screenshotSettings;
        [SerializeField] private InterpolationSettings interpolationSettings; 
        public Camera screenshotCamera => comboRotCot.VisCamera;

        [Serializable]
        public class ViewShot
        {
            public Texture2D texture2D; 
            public RotParams_Base rotParams;
            public string name;

            public ViewShot(Texture2D texture2D, RotParams_Base rotParams, string name = null)
            {
                this.texture2D = texture2D;
                this.rotParams = rotParams;
                this.name = string.IsNullOrEmpty(name) ? rotParams.ToString() : name;
            }
        }
        [SerializeField] private List<ViewShot> viewShots = new List<ViewShot>(); 
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                TakeAndSaveSingleScreenshot();
            }
            
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
            
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                StartInterpolationOverTime(); 
            }
        }

        [ContextMenu("TakeViewshot")]
        private void TakeViewshot()
        {
            Rect screenshotCameraRectBuffer = screenshotCamera.rect;
            screenshotCamera.rect = new Rect(0, 0, 1, 1);

            // Render to temporary RT
            RenderTexture rt = new RenderTexture(screenshotSettings.imageWidth, screenshotSettings.imageHeight, 16);
            screenshotCamera.targetTexture = rt;
            RenderTexture.active = rt;

            screenshotCamera.Render();

            // Read into Texture2D
            Texture2D tex = new Texture2D(screenshotSettings.imageWidth, screenshotSettings.imageHeight, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, screenshotSettings.imageWidth, screenshotSettings.imageHeight), 0, 0);
            tex.Apply();

            // Store the Texture2D
            viewShots.Add(new ViewShot(tex, comboRotCot.ActiveRotParams_Generic));

            // Cleanup
            screenshotCamera.targetTexture = null;
            RenderTexture.active = null;
            rt.Release();
            Destroy(rt);

            screenshotCamera.rect = screenshotCameraRectBuffer;
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
                Texture2D singleFrame = viewShots[i].texture2D;
                
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
                Destroy(viewShots[i].texture2D);
            }
            viewShots.Clear();
        }

        [ContextMenu("TakeAndSaveSingleScreenshot")]
        private void TakeAndSaveSingleScreenshot()
        {
            ScreenCapture.CaptureScreenshot(screenshotSettings.path + "/" + comboRotCot.ActiveRotParams_Generic.ToString() + ".png");
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
            File.WriteAllBytes($"{Path.Combine(screenshotSettings.path, comboRotCot.ActiveRotParams_Generic.ToString())}.png", byteArray);
        
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
            //+ZyKa there probably is a memory leak somewhere in this function, so only use it in playInEditor (because that can easily be stopped and thus memory cleaned)
            // ResetViewshots(); 
            for (int i = 0; i < interpolationSettings.InterpolationCount; i++)
            {
                comboRotCot.ActiveRotParams_Generic = interpolationSettings.Interpolate(interpolationSettings.GetInterpolationAlpha(i));
                yield return new WaitForEndOfFrame();
                
                TakeViewshot();
                yield return new WaitForEndOfFrame();
            }
            // SaveViewshots();
            cor_Interpolation = null; 
        }

        [ContextMenu("InterpolateOverTime")]
        private void StartInterpolationOverTime()
        {
            if (cor_Interpolation == null)
            {
                cor_Interpolation = StartCoroutine(InterpolationOverTime(interpolationSettings));
            }
        }

        private IEnumerator InterpolationOverTime(InterpolationSettings interpolationSettings)
        {
            RotParams_Base storedAppliedRotation = comboRotCot.GetAppliedRotation();
            
            RotParams_Base startingParams = interpolationSettings.Interpolate(interpolationSettings.GetInterpolationAlpha(0));
            RotParams_Base startingInverse = startingParams.GetInverse(); 
            
            if (interpolationSettings.visPath)
            {
                comboRotCot.ActiveRotParams_Generic = startingParams; 
                comboRotCot.ApplyRotation();
                
                //+ZyKa this is a bad hack to ensure that the Axis is setup correctly
                RotParams_Base interpolated = interpolationSettings.Interpolate(0.0001f);
                if (interpolationSettings.visPath)
                {
                    interpolated = interpolated.Concatenate(startingInverse, true); 
                }
                comboRotCot.ActiveRotParams_Generic = interpolated;
            }

            yield return new WaitForSeconds(interpolationSettings.orientationHoldTime); 
            
            for (int i = 0; i < interpolationSettings.InterpolationCount; i++)
            {
                RotParams_Base interpolated = interpolationSettings.Interpolate(interpolationSettings.GetInterpolationAlpha(i));
                if (interpolationSettings.visPath)
                {
                    interpolated = interpolated.Concatenate(startingInverse, true); 
                }
                comboRotCot.ActiveRotParams_Generic = interpolated; 
                yield return new WaitForSeconds(interpolationSettings.InterpolationStepTime); 
            }

            if (interpolationSettings.pingPongInterpolation)
            {
                yield return new WaitForSeconds(interpolationSettings.orientationHoldTime);
                
                for (int i = interpolationSettings.InterpolationCount - 1; i > 0; i--) //+ZyKa it should end at i = 0, but I need this bad hack to ensure that the axis-ring is oriented correctly
                {
                    RotParams_Base interpolated = interpolationSettings.Interpolate(interpolationSettings.GetInterpolationAlpha(i));
                    if (interpolationSettings.visPath)
                    {
                        interpolated = interpolated.Concatenate(startingInverse, true); 
                    }
                    comboRotCot.ActiveRotParams_Generic = interpolated; 
                    yield return new WaitForSeconds(interpolationSettings.InterpolationStepTime);
                }
            }

            yield return new WaitForSeconds(interpolationSettings.orientationHoldTime); 
            
            if (interpolationSettings.visPath)
            {
                comboRotCot.ResetAppliedObjectRotation(); 
                comboRotCot.ActiveRotParams_Generic = storedAppliedRotation;
                comboRotCot.ApplyRotation(); 
            }
            
            cor_Interpolation = null; 
        }

        [SerializeField] private int interpolationStep; 
        [ContextMenu("SetToNextInterpolationStep")]
        private void SetToInterpolationStep()
        {
            float alpha = interpolationSettings.GetInterpolationAlpha(interpolationStep); 
            comboRotCot.ActiveRotParams_Generic = interpolationSettings.Interpolate(alpha);
        }
    }
}
