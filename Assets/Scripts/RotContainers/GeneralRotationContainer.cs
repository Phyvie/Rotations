using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Editor;
using RotParams;
using UI_Toolkit;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace RotContainers
{
    public class GeneralRotationContainer : MonoBehaviour
    {
        #region Variables
        [SerializeField] private bool InitializeOnAwake = false;
        [SerializeField] private bool InitializeOnStart = true; 
        
        [SerializeField] private UIDocument uiDocument;

        public UIDocument UIDocument
        {
            get => uiDocument;
            set
            {
                Destroy(uiDocument);
                uiDocument = value;
            }
        }

        [Tooltip("This is only needed when the UIDocument is not set externally (e.g. by FullScreenMultiRotationContainer); Thus the component must create a UIDocument itself")]
        [SerializeField] private VisualTreeAsset fullScreenUIAsset;
        [SerializeField] private PanelSettings panelSettingsAsset;

        [Tooltip("This is not the object which contains uiMenu & UIRotSlot, rather it is the visualElement into which the uiRoot will be spawned")]
        [SerializeField] private string uiParentName; 
        private VisualElement _uiParent; //the slot into which the _uiRoot will be spawned

        [Tooltip("This is the VisualElement which actually contains uiMenuLine & RotParamsSlot")]
        [SerializeField] private VisualTreeAsset uiFullContainerAsset;
        private VisualElement _uiRoot; //the actual root containing the menuLine + UIRotSlot
        
        [SerializeField] private VisualTreeAsset uiMenuAsset;
        [SerializeField] private string uiMenuName = "uiMenuLine"; 
        private VisualElement _uiMenu; 
        
        [SerializeField] private UISlotReference uiRotSlot;
        //Prefabs for UIRot are below in the #region UITypeSelectionControls
        
        private RotParams.RotParams _rotParams;
        [SerializeField] private TypedRotationContainer typedRotationContainer;
        
        [SerializeField] private GameObject cameraPrefab;
        private GameObject cameraRotationPivot;  
        [SerializeField] private Camera visCamera;
        public Camera VisCamera
        {
            get => visCamera;
            set
            {
                if (visCamera != null && visCamera != value)
                {
                    Destroy(visCamera);
                }
                visCamera = value;
            }
        }
        
        [SerializeField] private Rect cameraScreenRect = new Rect(0, 0, 1, 1);
        [FormerlySerializedAs("cameraMovementEnabled")] [SerializeField] private bool cameraInputEnabled = false;

        public bool CameraMovementEnabled
        {
            get => cameraInputEnabled;
            set => cameraInputEnabled = value;
        }
        
        #endregion Variables
        
        public Rect CameraScreenRect
        {
            get => cameraScreenRect;
            set
            {
                if (visCamera != null)
                {
                    visCamera.rect = value; 
                }
                cameraScreenRect = value;
            }
        }

        #region RotationTypeData
        [Serializable]
        public class TypedRotationContainerPrefab
        {
            public GameObject visPrefab;
            public VisualTreeAsset uiPrefab; 
        }

        [SerializeField] private TypedRotationContainerPrefab eulerPrefab;
        [SerializeField] private TypedRotationContainerPrefab quaternionPrefab;
        [SerializeField] private TypedRotationContainerPrefab axisAnglePrefab;
        [SerializeField] private TypedRotationContainerPrefab matrixPrefab;
        #endregion RotationTypeData
        
        #region UITypeSelectionControls
        [CreateProperty]
        public List<string> SelectableRotationTypes => new List<string>
        {
            "EulerAngles", 
            "Quaternion", 
            "AxisAngle", 
            "Matrix"
        };

        private int _selectedTypeIndex = 0;

        [CreateProperty]
        private int SelectedTypeIndex
        {
            get => _selectedTypeIndex;
            set
            {
                switch (value)
                {
                    case 0: 
                        GenerateNewEulerAngles();
                        break;
                    case 1:
                        GenerateNewQuaternion();
                        break;
                    case 2: 
                        GenerateNewAxisAngle();
                        break;
                    case 3:
                        GenerateNewMatrix();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _selectedTypeIndex = value;
            }
        }
        #endregion UITypeSelectionControls
        
        #region Initialization
        private void Awake()
        {
            if (InitializeOnAwake)
            {
                SelfInitialize();
            }
        }

        public void Start()
        {
            if (InitializeOnStart)
            {
                SelfInitialize();
            }
        }
        
        [ContextMenu("Init Rotation Container")]
        private void SelfInitialize()
        {
            if (uiDocument == null)
            {
                uiDocument = gameObject.AddComponent<UIDocument>();
                uiDocument.visualTreeAsset = fullScreenUIAsset; 
                uiDocument.panelSettings = panelSettingsAsset;
            }
            
            if (_uiParent == null)
            {
                _uiParent = string.IsNullOrEmpty(uiParentName) ? 
                    uiDocument.rootVisualElement :  
                    uiDocument.rootVisualElement.Q<VisualElement>(uiParentName);
                
                if (_uiParent == null)
                {
                    Debug.LogWarning($"{name} could not find uiRotationRoot");
                    _uiParent = uiDocument.rootVisualElement;
                }
            }
            
            if (_uiRoot == null)
            {
                _uiRoot = uiFullContainerAsset.CloneTree(); 
                _uiParent.Add(_uiRoot);
                _uiRoot.style.flexGrow = 1; 
                _uiRoot.name = "RotationContainer"; 
            }
            
            _uiMenu = _uiParent.Q<VisualElement>(uiMenuName);
            if (_uiMenu == null)
            {
                Debug.LogWarning($"{name} could not find uiMenu");
                return; 
            }
            _uiMenu.dataSource = this;
            
            uiRotSlot.Initialize(_uiRoot);
            uiRotSlot.UISlot.dataSource = _rotParams; 
            
            if (VisCamera ==null)
            {
                VisCamera = Instantiate(cameraPrefab, this.transform).transform.GetChild(0).GetComponent<Camera>();
                VisCamera.rect = cameraScreenRect; 
            }

            if (cameraRotationPivot == null)
            {
                cameraRotationPivot = VisCamera.transform.parent.gameObject; 
            }

            SelectedTypeIndex = SelectedTypeIndex; //-ZyKa RotationContainer this line ensures that the RotUI is properly initiated
        }

        public void InitializeExternally(UIDocument newUIDocument, VisualElement visualParent)
        {
            uiDocument = newUIDocument;
            SetUIParent(visualParent);
        }

        public void SetUIParent(VisualElement newUIParent)
        {
            _uiParent = newUIParent;
            uiParentName = newUIParent.name; 
        }
        #endregion Initialization

        #region ChangeRotationType
        public void GenerateNewRotation<RotParamsType>() where RotParamsType : RotParams.RotParams, new()
        {
            _rotParams = new RotParamsType();
            if (typedRotationContainer == null)
            {
                typedRotationContainer = gameObject.AddComponent<TypedRotationContainer>();
            }
            TypedRotationContainerPrefab prefab =
                typeof(RotParamsType) switch
                {
                    Type t when t == typeof(RotParams_EulerAngles) => eulerPrefab,
                    Type t when t == typeof(RotParams_Quaternion) => quaternionPrefab,
                    Type t when t == typeof(RotParams_AxisAngle) => axisAnglePrefab,
                    { } t when t == typeof(RotParams_Matrix) => matrixPrefab,
                    _ => null
                };
            
            if (prefab == null)
            {
                throw new NullReferenceException();
            }
            
            typedRotationContainer.SpawnTypedRotation(ref _rotParams, prefab.visPrefab, this.transform, prefab.uiPrefab, uiRotSlot); 
        }
        
        //accessor function for the templated version
        public void GenerateNewRotationGeneric(System.Type type)
        {
            if (!typeof(RotParams.RotParams).IsAssignableFrom(type))
            {
                Debug.LogError($"Type {type} is not a subclass of RotParams.RotParams.");
                return;
            }

            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                Debug.LogError($"Type {type} does not have a parameterless constructor.");
                return;
            }

            // Get a reference to the generic method definition
            var method = GetType()
                .GetMethod(nameof(GenerateNewRotation), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.MakeGenericMethod(type);

            if (method == null)
            {
                Debug.LogError($"Failed to resolve generic method for type {type}.");
                return;
            }

            // Invoke the generic method
            method.Invoke(this, null);
        }
        
        [ContextMenu("GenerateMatrix")]
        public void GenerateNewMatrix()
        {
            GenerateNewRotation<RotParams_Matrix>(); 
        }
        
        [ContextMenu("GenerateAxisAngle")]
        public void GenerateNewAxisAngle()
        {
            GenerateNewRotation<RotParams_AxisAngle>(); 
        }
        
        [ContextMenu("GenerateQuaternion")]
        public void GenerateNewQuaternion()
        {
            GenerateNewRotation<RotParams_Quaternion>(); 
        }
        
        [ContextMenu("GenerateEuler")]
        public void GenerateNewEulerAngles()
        {
            GenerateNewRotation<RotParams_EulerAngles>();
        }
        #endregion ChangeRotationType

        private void OnValidate()
        {
            /* !ZyKa OnValidate
            if (EditorApplication.isPlayingOrWillChangePlaymode && !Application.isPlaying)
            {
                return; 
            }
            Init();
            */ 
        }

        #region userInteraction
        private void Update()
        {
            if (cameraInputEnabled)
            {
                if (Input.GetMouseButton(1))
                {
                    RotateCamera(Input.GetAxis("Mouse X") * 2.5f, Input.GetAxis("Mouse Y") * -2.5f); 
                }
                ZoomCamera(Input.GetAxis("Mouse ScrollWheel") * -1.5f);
                
                if (Input.GetKeyDown(KeyCode.F12))
                {
                    FullScreenshot();
                }

                if (Input.GetKeyDown(KeyCode.F11))
                {
                    CameraViewshot();
                }
            }
        }

        private void RotateCamera(float deltaX, float deltaY)
        {
            cameraRotationPivot.transform.localEulerAngles += new Vector3(deltaY, deltaX, 0); 
        }

        private void ZoomCamera(float deltaZoom)
        {
            visCamera.transform.localPosition += Vector3.back * deltaZoom; 
        }
        #endregion userInteraction

        [ContextMenu("FullScreenshot")]
        private void FullScreenshot()
        {
            string path = "D:/CGL/Bachelor-Thesis/RotationImages/"; 
            string timestamp = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
            
            ScreenCapture.CaptureScreenshot($"{path}{timestamp}.png", 4);
        }

        [ContextMenu("CameraViewshot")]
        private void CameraViewshot()
        {
            string path = "D:/CGL/Bachelor-Thesis/RotationImages/"; 
            string timestamp = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");

            int imageWidth = 1000; 
            int imageHeight = 1000;

            Rect visCameraRectBuffer = visCamera.rect;
            visCamera.rect = new Rect(0, 0, 1, 1); 
            RenderTexture screenTexture = new RenderTexture(imageWidth, imageHeight, 16);
            visCamera.targetTexture = screenTexture;
            RenderTexture.active = screenTexture;
            visCamera.Render();
            visCamera.rect = visCameraRectBuffer;

            Texture2D renderedTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGBA32, false);
            renderedTexture.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
            RenderTexture.active = null;

            byte[] byteArray = renderedTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes($"{path}{timestamp}.png", byteArray);
            
            visCamera.targetTexture = null;
        }

        [ContextMenu("StartScreenShotInterpolation")]
        private void StartScreenShotInterpolation()
        {
            StartCoroutine(ScreenShotInterpolation(screenshotInterpolationSettings));
        }

        [SerializeField] private ScreenshotInterpolationSettings screenshotInterpolationSettings; 
        
        private IEnumerator ScreenShotInterpolation(ScreenshotInterpolationSettings settings)
        {
            string path = settings.path;
            string timestamp = System.DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");

            int imageWidth = settings.imageWidth;
            int imageHeight = settings.imageHeight;
            int imageWidthOffset = settings.imageWidthOffset;
            int imageHeightOffset = settings.imageHeightOffset;

            float[] interpolationAlphas = settings.interpolationAlphas;

            int totalImages = interpolationAlphas.Length;
            int columns = Mathf.CeilToInt(Mathf.Sqrt(totalImages));
            int rows = Mathf.CeilToInt((float)totalImages / columns);

            int totalWidth = columns * imageWidth + (columns - 1) * imageWidthOffset;
            int totalHeight = rows * imageHeight + (rows - 1) * imageHeightOffset;

            Texture2D renderedTexture = new Texture2D(totalWidth, totalHeight, TextureFormat.RGBA32, false);
            RenderTexture screenTexture = new RenderTexture(imageWidth, imageHeight, 16);

            visCamera.rect = new Rect(0, 0, 1, 1);
            visCamera.targetTexture = screenTexture;
            Rect visCameraRectBuffer = visCamera.rect;
            RenderTexture.active = screenTexture;

            for (int i = 0; i < totalImages; i++)
            {
                float t = interpolationAlphas[i];

                // Call abstract interpolation method implemented by child class
                settings.Interpolate(ref _rotParams, t);

                yield return new WaitForEndOfFrame();

                visCamera.Render();

                Texture2D singleFrame = new Texture2D(imageWidth, imageHeight, TextureFormat.RGBA32, false);
                singleFrame.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
                singleFrame.Apply();

                // Save individual frame
                string singlePath = $"{path}{timestamp}_t{t:F2}.png";
                System.IO.File.WriteAllBytes(singlePath, singleFrame.EncodeToPNG());

                // Composite grid placement
                int col = i % columns;
                int row = i / columns;
                int x = col * (imageWidth + imageWidthOffset);
                int y = (rows - 1 - row) * (imageHeight + imageHeightOffset);

                renderedTexture.SetPixels(x, y, imageWidth, imageHeight, singleFrame.GetPixels());

                Destroy(singleFrame);
            }

            renderedTexture.Apply();

            // Cleanup
            visCamera.rect = visCameraRectBuffer;
            visCamera.targetTexture = null;
            RenderTexture.active = null;
            screenTexture.Release();
            Destroy(screenTexture);

            // Save composite grid image
            byte[] gridBytes = renderedTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes($"{path}{timestamp}_grid.png", gridBytes);

            Debug.Log($"✅ Screenshot interpolation complete: saved {totalImages} frames and grid at:\n{path}{timestamp}_grid.png");
        }

    }
}
