using System;
using System.Collections.Generic;
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
        #region Variables
        [SerializeField] private bool InitializeOnAwake = false;
        [SerializeField] private bool InitializeOnStart = true; 
        #endregion Variables
        
        #region TypedRotCots
        [SerializeField] private rotCotTemplateAxisAngle rotCotTemplateAxisAngle; 
        [SerializeField] private rotCotTemplateQuaternion rotCotTemplateQuaternion; 
        [SerializeField] private rotCotEuler rotCotEuler;
        [SerializeField] private rotCotTemplateMatrix rotCotTemplateMatrix;
        private List<RotCot_GenericBase> rotCotsList => new List<RotCot_GenericBase>(){rotCotTemplateAxisAngle, rotCotTemplateQuaternion, rotCotEuler, rotCotTemplateMatrix};
        
        private RotCot_GenericBase activeRotCot;
        private System.Type activeRotCotType; 
        public RotParams_Base ActiveRotParams_Generic
        {
            get => activeRotCot.GetRotParams_Generic(); 
            set => activeRotCot.SetRotParams_Generic(value);
        }
        
        #endregion TypedRotCots
        
        #region RotObjCot
        [SerializeField] private GameObject orientedObjectPrefab; 
        [SerializeField] private OrientedObject orientedObject; 
        #endregion RotObjCot
        
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
        
        #region CoordinateGrid
        [SerializeField] private Vis_CoordinateGrid coordinateGrid;
        #endregion CoordinateGrid
        
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
            InitUI();
            InitVisCam(); 
            InitOrientedObject();
            
            InitializeRotCots(); 
            _selectedTypeIndex = 1; 
            ActivateRotCot(rotCotTemplateQuaternion);
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
            
            _uiMenuLine = _comboContainerUIParent.Q<VisualElement>(uiMenuLineName);
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
            InitializeRotCots(); 
            _selectedTypeIndex = 1; 
            ActivateRotCot(rotCotTemplateQuaternion);
            
            if (coordinateGrid != null)
            {
                coordinateGrid.ViewCamera = VisCamera; 
            }
            else
            {
                Debug.LogWarning("No CoordinateGrid found; cannot initialize CoordinateGrid"); 
            }
        }
        
        private void InitializeRotCots()
        {
            foreach (RotCot_GenericBase rotCot in rotCotsList)
            {
                rotCot.Initialize(this.transform, _uiRotParamsSlot, orientedObject);
                rotCot.enabled = false; 
            }
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
                switch (value)
                {
                    case 0: 
                        SwitchActiveRotCot(typeof(rotCotTemplateAxisAngle));
                        break;
                    case 1:
                        SwitchActiveRotCot(typeof(rotCotTemplateQuaternion));
                        break;
                    case 2: 
                        SwitchActiveRotCot(typeof(rotCotEuler));
                        break;
                    case 3:
                        SwitchActiveRotCot(typeof(rotCotTemplateMatrix));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
        private void SwitchActiveRotCot(Type newType)
        {
            if (activeRotCotType == newType)
            {
                return; 
            }

            RotCot_GenericBase foundRotCot = null; 
            foreach (RotCot_GenericBase rotCot in rotCotsList)
            {
                if (rotCot.GetType() == newType)
                {
                    foundRotCot = rotCot;
                    break; 
                }
            }

            if (foundRotCot != null)
            { 
                DeactiveRotCot();
                ActivateRotCot(foundRotCot);
            }
            else
            {
                Debug.LogError($"{name} cannot switch to RotCot of Type {newType}; no object of such type exists in this {nameof(ComboRotCot)}"); 
            }
        }

        private void DeactiveRotCot()
        {
            if (activeRotCot == null)
            {
                return; 
            }

            activeRotCot.enabled = false; 
            activeRotCotType = null; 
        }

        private void ActivateRotCot(RotCot_GenericBase rotCot)
        {
            if (rotCot == null)
            {
                return; 
            }
            
            activeRotCot = rotCot;
            rotCot.enabled = true;
            activeRotCotType = rotCot.GetType(); 
        }
        #endregion SwitchRotParamType
        
        #region CameraInteraction
        private void RotateCamera(float deltaX, float deltaY)
        {
            cameraRotationPivot.transform.localEulerAngles += new Vector3(deltaY, deltaX, 0); 
        }

        private void ZoomCamera(float deltaZoom)
        {
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
        
        [CreateProperty]
        public bool ParamsResetFunction
        {
            get => false;
            set
            {
                activeRotCot.GetRotParams_Generic().ResetToIdentity();
                activeRotCot.GetRotVis_Generic().VisUpdate();
            }
        }
        #endregion userInteraction
    }
}