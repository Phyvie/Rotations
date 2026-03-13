using System;
using System.Collections.Generic;
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
        
        #endregion TypedRotCots
        
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

        [Tooltip("This is only needed when the UIDocument is not set externally (e.g. by FullScreenMultiRotationContainer); Thus the component must create a UIDocument itself")]
        [SerializeField] private VisualTreeAsset fullScreenUIAsset;
        [SerializeField] private PanelSettings panelSettingsAsset;
        
        [Tooltip("This is not the object which contains uiMenu & UIRotSlot; it is the visualElement into which the uiRoot will be spawned")]
        [SerializeField] private string uiComboContainerParentName; 
        private VisualElement _comboContainerUIParent; //the slot into which the _uiRoot will be spawned
        
        [Tooltip("This is the VisualElement which actually contains uiMenuLine & RotParamsSlot")]
        [SerializeField] private VisualTreeAsset uiComboContainerRootAsset;
        private VisualElement _comboContainerRoot; //the actual root containing the menuLine + UIRotSlot
        
        [SerializeField] private VisualTreeAsset uiMenuLineAsset;
        [SerializeField] private string uiMenuLineName = "uiMenuLine"; 
        private VisualElement _uiMenuLine;

        [SerializeField] private string uiRotParamsSlot = "RotParamsSlot"; 
        private VisualElement _uiRotParamsSlot;
        #endregion UISetup
        
        #region Cam
        [SerializeField] private GameObject cameraPrefab;
        private GameObject cameraRotationPivot;  
        [SerializeField] private Camera visCamera;
        public Camera VisCamera
        {
            get => visCamera;
            set => visCamera = value;
        }
        
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
            InitOrientedObject();
            InitCoordinateGrid(); 
            
            InitializeRotCots(); 
            SelectedTypeIndex = 0; 
        }

        private void InitUI()
        {
            if (uiDocument == null)
            {
                if (!TryGetComponent<UIDocument>(out uiDocument))
                {
                    uiDocument = gameObject.AddComponent<UIDocument>();
                    uiDocument.visualTreeAsset = fullScreenUIAsset; 
                    uiDocument.panelSettings = panelSettingsAsset;
                }
            }
            
            if (_comboContainerUIParent == null)
            {
                _comboContainerUIParent = string.IsNullOrEmpty(uiComboContainerParentName) ? 
                    null :  
                    uiDocument.rootVisualElement.Q<VisualElement>(uiComboContainerParentName);
                
                if (_comboContainerUIParent == null)
                {
                    Debug.LogError($"{name} could not find uiRotationRoot");
                    _comboContainerUIParent = uiDocument.rootVisualElement;
                }
            }
            
            if (_comboContainerRoot == null)
            {
                _comboContainerRoot = uiComboContainerRootAsset.CloneTree(); 
                _comboContainerUIParent.Add(_comboContainerRoot);
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
                VisCamera = Instantiate(cameraPrefab, this.transform).transform.GetChild(0).GetComponent<Camera>();
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

            /* !!!ZyKa RotParams_Conversion: There should be a conversion of rotParams here */
            _activeRotCot = newActiveRotCot;
            
            if (newActiveRotCot == null)
            {
                Debug.LogWarning($"{nameof(SwitchActiveRotCot)} just switched ActiveRotCot to null");
                return; 
            }
            
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
                    rotCot.Reset();
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