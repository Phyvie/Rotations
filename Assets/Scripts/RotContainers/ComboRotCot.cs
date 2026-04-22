using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Packages.UnityExtensionMethods;
using RotObj;
using RotParams;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace RotContainers
{
    /*
     * TODO: Separate this class into multiple classes, because it currently manages everything from setting up UI to camera movement and Coordinate-Grid
     */
    public class ComboRotCot : MonoBehaviour
    {
        #region InitializeVariables
        [SerializeField] private MonoBehaviourFunctions initMode; 
        #endregion InitializeVariables
        
        #region TypedRotCots
        [SerializeField] private RotCotAxisAngle rotCotAxisAngle; 
        [SerializeField] private RotCotQuaternion rotCotQuaternion; 
        [SerializeField] private RotCotEuler rotCotEuler;
        [SerializeField] private RotCotMatrix rotCotMatrix;
        private List<RotCot_GenericBase> _rotCotsList;
        private List<RotCot_GenericBase> RotCotsList
        {
            get
            {
                if (_rotCotsList == null)
                {
                    _rotCotsList = new List<RotCot_GenericBase>() { rotCotAxisAngle, rotCotQuaternion, rotCotEuler, rotCotMatrix };
                }
                return _rotCotsList;
            }
        }
        
        private RotCot_GenericBase _activeRotCot;
        public RotParams_Base ActiveRotParams_Generic
        {
            get => _activeRotCot.GetRotParams_Generic(); 
            set => _activeRotCot.SetRotParams_Generic(value);
        }

        private bool _convertRotParamsOnSwitch = false;
        [CreateProperty]
        private bool ConvertRotParamsOnSwitch
        {
            get => _convertRotParamsOnSwitch; 
            set => _convertRotParamsOnSwitch = value;
        }
        
        #endregion TypedRotCots
        
        #region Cam
        [SerializeField] private GameObject cameraPrefab;
        private GameObject cameraRotationPivot;  
        [SerializeField] private Camera visCamera;
        public Camera VisCamera
        {
            get => visCamera;
            set
            {
                visCamera = value;
                if (visCamera != null && coordinateGrid != null)
                {
                    coordinateGrid.ViewCamera = VisCamera; 
                }
            }
        }

        #if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern int GetCanvasHeight();
        [DllImport("__Internal")]
        private static extern int GetCanvasWidth();
        #endif
        
        private void AdjustCameraAndUIRatio(GeometryChangedEvent e)
        {
            if (e.newRect.size == e.oldRect.size)
            {
                return; 
            }

            if (visCamera == null)
            {
                Debug.Log($"{nameof(AdjustCameraAndUIRatio)} visCamera is null");
                return; 
            }

            if (_splitscreen__CameraSpace_Element == null)
            {
                Debug.Log($"{nameof(AdjustCameraAndUIRatio)} {nameof(_splitscreen__CameraSpace_Element)} is null");
                return; 
            }

            #if UNITY_WEBGL && !UNITY_EDITOR
            float screenHeight = GetCanvasHeight(); //ZyKa!
            #else
            float screenHeight = Screen.height;
            #endif
            float cameraHeightFraction = _splitscreen__CameraSpace_Element.resolvedStyle.height / screenHeight;

            Debug.Log("ScreenHeight: " + screenHeight + "_splitscreen_CameraSpace.height" + _splitscreen__CameraSpace_Element.resolvedStyle.height + " CameraHeightFraction: " + cameraHeightFraction); 
            
            visCamera.rect = new Rect
            (
                0f,
                1f - cameraHeightFraction,
                1f,
                cameraHeightFraction
            ); 
        }
        
        #region CoordinateGrid
        [SerializeField] private Vis_CoordinateGrid coordinateGrid;
        #endregion CoordinateGrid
        
        #region orientedObject
        [SerializeField] private GameObject orientedObjectPrefab; 
        [SerializeField] private OrientedObject orientedObject; 
        #endregion orientedObject
        
        #region UISetup
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

        [Tooltip("This is only needed when the UIDocument is not set externally (e.g. by SplitscreenMultiRotationContainer); Thus the component must create a UIDocument itself")]
        [SerializeField] private VisualTreeAsset SplitscreenUIAsset;
        [SerializeField] private PanelSettings panelSettingsAsset;
        
        [Tooltip("This is not the object which contains uiMenu & UIRotSlot; it is the visualElement into which the uiRoot will be spawned")]
        [SerializeField] private string Splitscreen__UISpace_ElementName = "Splitscreen__UISpace";
        private VisualElement _splitscreen__UISpace_Element;  
        [SerializeField] private string Splitscreen__CameraSpace_ElementName = "Splitscreen__CameraSpace";
        private VisualElement _splitscreen__CameraSpace_Element; 
        
        [Tooltip("This is the VisualElement which actually contains uiMenuLine & RotParamsSlot")]
        [SerializeField] private VisualTreeAsset uiComboContainerRootAsset;
        private VisualElement _comboContainerRoot; //the actual root containing the menuLine + UIRotSlot
        
        [SerializeField] private VisualTreeAsset uiMenuLineAsset;
        [SerializeField] private string uiMenuLineName = "uiMenuLine"; 
        private VisualElement _uiMenuLine;

        [SerializeField] private string uiRotParamsSlot = "RotParamsSlot"; 
        private VisualElement _uiRotParamsSlot;
        #endregion UISetup
        
        [SerializeField] private Rect cameraScreenRect = new Rect(0, 0, 1, 1);
        [SerializeField] private bool cameraInputEnabled = false;

        public bool CameraMovementEnabled
        {
            get => cameraInputEnabled;
            set => cameraInputEnabled = value;
        }
        
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
        #endregion Cam
        
        #region Initialization
        private void Awake()
        {
            if (initMode == MonoBehaviourFunctions.Awake)
            {
                SelfInitialize();
            }
        }

        public void Start()
        {
            if (initMode == MonoBehaviourFunctions.Start)
            {
                SelfInitialize();
            }
        }

        private bool isInitialized = false; 
        [ContextMenu("Init Rotation Container")]
        private void SelfInitialize()
        {
            if (isInitialized)
            {
                Debug.LogWarning($"{name} is already initialized");
                return;
            }
            isInitialized = true;
            
            InitUI();
            InitVisCam();
            uiDocument?.rootVisualElement?.RegisterCallback<GeometryChangedEvent>(AdjustCameraAndUIRatio); 
            InitOrientedObject();
            InitCoordinateGrid(); 
            
            InitializeRotCots(); 
            SelectedTypeIndex = 0; 
        }

        private void OnDestroy()
        {
            if (isInitialized)
            {
                uiDocument?.rootVisualElement?.UnregisterCallback<GeometryChangedEvent>(AdjustCameraAndUIRatio);
            }
        }

        private void InitUI()
        {
            if (uiDocument == null)
            {
                if (!TryGetComponent<UIDocument>(out uiDocument))
                {
                    uiDocument = gameObject.AddComponent<UIDocument>();
                    uiDocument.visualTreeAsset = SplitscreenUIAsset; 
                    uiDocument.panelSettings = panelSettingsAsset;
                }
            }
            
            if (_splitscreen__UISpace_Element == null)
            {
                _splitscreen__UISpace_Element = 
                    string.IsNullOrEmpty(Splitscreen__UISpace_ElementName) ? 
                        null :  
                        uiDocument.rootVisualElement.Q<VisualElement>(Splitscreen__UISpace_ElementName);
                
                if (_splitscreen__UISpace_Element == null)
                {
                    Debug.LogError($"{name} could not find uiRotationRoot");
                    _splitscreen__UISpace_Element = uiDocument.rootVisualElement;
                }
            }

            if (_splitscreen__CameraSpace_Element == null)
            {
                _splitscreen__CameraSpace_Element = 
                    string.IsNullOrEmpty(Splitscreen__CameraSpace_ElementName) ? 
                        null : 
                        uiDocument.rootVisualElement.Q<VisualElement>(Splitscreen__CameraSpace_ElementName);
                
                if (_splitscreen__CameraSpace_Element == null)
                {
                    Debug.LogError($"{name} could not find uiCameraSpace");
                    _splitscreen__CameraSpace_Element = uiDocument.rootVisualElement;
                }
            }
            
            if (_comboContainerRoot == null)
            {
                _comboContainerRoot = uiComboContainerRootAsset.CloneTree(); 
                _splitscreen__UISpace_Element.Add(_comboContainerRoot);
                _comboContainerRoot.style.flexGrow = 1; 
                _comboContainerRoot.name = "RotationContainer"; 
            }
            
            _uiMenuLine = _comboContainerRoot.Q<VisualElement>(uiMenuLineName);
            if (_uiMenuLine == null)
            {
                Debug.LogError($"{name} could not find uiMenu");
                return; 
            }
            _uiMenuLine.dataSource = this;

            if (_uiRotParamsSlot == null)
            {
                _uiRotParamsSlot = _comboContainerRoot.Q<VisualElement>(uiRotParamsSlot);
                if (_uiRotParamsSlot == null)
                {
                    Debug.LogError($"{name} could not find _uiRotSlot");
                    return; 
                }
            }
        }

        private void InitVisCam()
        {
            if (VisCamera ==null)
            {
                if (cameraPrefab == null)
                {
                    Debug.LogError($"{name} VisCamera prefab is null");
                    return;
                }
                GameObject visCamGO = Instantiate(cameraPrefab, this.transform);
                VisCamera = visCamGO.GetComponentInChildren<Camera>();
                if (VisCamera == null)
                {
                    Debug.LogError($"{name} VisCamera prefab does not contain a Camera component");
                    return;
                }
                VisCamera.rect = cameraScreenRect; 
            }

            if (cameraRotationPivot == null)
            {
                cameraRotationPivot = VisCamera.transform.parent.gameObject; 
            }
        }
        
        private void InitOrientedObject()
        {
            if (orientedObject == null)
            {
                orientedObject = Instantiate(orientedObjectPrefab, this.transform).GetComponent<OrientedObject>(); 
            }
        }
        
        private void InitializeRotCots()
        {
            foreach (RotCot_GenericBase rotCot in RotCotsList)
            {
                if (rotCot != null)
                {
                    rotCot.Initialize(this.transform, _uiRotParamsSlot, orientedObject);
                    rotCot.enabled = false;
                }
            }
        }

        private void InitCoordinateGrid()
        {
            if (coordinateGrid == null)
            {
                Debug.LogWarning("No CoordinateGrid found; cannot initialize CoordinateGrid");
                return; 
            }
            coordinateGrid.ViewCamera = VisCamera; 
        }
        #endregion Initialization
        
        #region UITypeSelectionControls
        [CreateProperty]
        public List<string> SelectableRotationTypes => new List<string>
        {
            "AxisAngle",
            "Quaternion",
            "EulerAngles",
            "Matrix"
        };

        private int _selectedTypeIndex = 0;

        [CreateProperty]
        private int SelectedTypeIndex 
        {
            get => _selectedTypeIndex;
            set
            {
                SwitchActiveRotCot(RotCotsList[value]);
                _selectedTypeIndex = value;
            }
        }
        #endregion UITypeSelectionControls
        
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
            }
        }

        #region SwitchRotParamType
        /* accessor function, mostly for backwards compatibility */
        private void SwitchActiveRotCot(Type newType)
        {
            SwitchActiveRotCot(RotCotsList.Find((rotCot) => rotCot != null && rotCot.GetType().IsAssignableFrom(newType)));
        }

        private void SwitchActiveRotCot(RotCot_GenericBase newActiveRotCot)
        {
            if (_activeRotCot == newActiveRotCot)
            {
                return; 
            }

            if (_activeRotCot != null)
            {
                _activeRotCot.enabled = false;
            }

            if (ConvertRotParamsOnSwitch && _activeRotCot != null)
            {
                newActiveRotCot.GetRotParams_Generic().ConvertAndCopyValues(_activeRotCot.GetRotParams_Generic()); 
            }
            _activeRotCot = newActiveRotCot;
            
            if (newActiveRotCot == null)
            {
                Debug.LogWarning($"{nameof(SwitchActiveRotCot)} just switched ActiveRotCot to null");
                return; 
            }
            
            //OnEnable handles further interactions (event subscriptions, VisUpdate(), OrientedObject.SetRotation(), ...)
            _activeRotCot.enabled = true; 
        }
        #endregion SwitchRotParamType
        
        #region Reset

        /* LaterZyKa User Controls: Figure out how to create UI-Buttons that the user can click to access functions */
        [CreateProperty]
        public bool ParamsResetFunction
        {
            get => false;
            set
            {
                if (_activeRotCot != null)
                {
                    _activeRotCot.GetRotParams_Generic()?.ResetToIdentity();
                    /* LaterZyKa ResetRotCot: check whether you need to manually call a VisUpdate here */   
                    _activeRotCot.GetRotVis_Generic()?.VisUpdate();
                }
            }
        }
        
        public void Reset()
        {
            foreach (RotCot_GenericBase rotCot in RotCotsList)
            {
                if (rotCot != null)
                {
                    rotCot.ResetToIdentity();
                    rotCot.enabled = false;
                }
            }
            SelectedTypeIndex = 0; 
            ResetAppliedObjectRotation();
        }
        #endregion Reset

        /* LaterZyKa CameraControls: Move CameraControls into their own class with their own script */
        #region CameraInteraction
        private void RotateCamera(float deltaX, float deltaY)
        {
            if (cameraRotationPivot == null)
            {
                Debug.LogWarning("CameraRotationPivot is null");
                return;
            }
            cameraRotationPivot.transform.localEulerAngles += new Vector3(deltaY, deltaX, 0); 
        }

        private void ZoomCamera(float deltaZoom)
        {
            if (visCamera == null)
            {
                Debug.LogWarning("VisCamera is null");
                return;
            }
            visCamera.transform.localPosition += Vector3.back * deltaZoom; 
        }
        #endregion CameraInteraction
        
        #region ApplyRotation
        [ContextMenu("ApplyObjectRotation")]
        public void ApplyRotation()
        {
            orientedObject.ApplyObjectRotation();
        }

        public RotParams_Base GetAppliedRotation()
        {
            return new RotParams_Quaternion(orientedObject.GetAppliedRotation()); //TodoZyKa RotObjectHierarchy proper conversion functions 
        }
        
        [ContextMenu("ResetAppliedObjectRotation")]
        public void ResetAppliedObjectRotation()
        {
            orientedObject.ResetAppliedObjectRotation(); 
        }
        #endregion ApplyRotation
        
        #endregion userInteraction
    }
}