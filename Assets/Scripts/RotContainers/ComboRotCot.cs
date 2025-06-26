using System;
using System.Collections.Generic;
using UI_Toolkit;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotContainers
{
    public class ComboRotCot : MonoBehaviour
    {
        #region Variables
        [SerializeField] private bool InitializeOnAwake = false;
        [SerializeField] private bool InitializeOnStart = true; 
        
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
        [SerializeField] private string uiParentName; 
        private VisualElement _uiParent; //the slot into which the _uiRoot will be spawned

        [Tooltip("This is the VisualElement which actually contains uiMenuLine & RotParamsSlot")]
        [SerializeField] private VisualTreeAsset uiFullContainerAsset;
        private VisualElement _uiRoot; //the actual root containing the menuLine + UIRotSlot
        
        [SerializeField] private VisualTreeAsset uiMenuAsset;
        [SerializeField] private string uiMenuName = "uiMenuLine"; 
        private VisualElement _uiMenu;

        [SerializeField] private string uiRotSlotName = "RotParamsSlot"; 
        private VisualElement _uiRotSlot;
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
        
        #region TypedRotationContainers
        private RotCot_GenericBase activeRotCot;
        private System.Type activeRotCotType; 
        
        [SerializeField] private RotCot_AxisAngle rotCot_AxisAngle; 
        [SerializeField] private RotCot_Quaternion rotCot_Quaternion; 
        [SerializeField] private RotCot_Euler rotCot_Euler;
        [SerializeField] private RotCot_Matrix rotCot_Matrix;
        
        private List<RotCot_GenericBase> rotCotsList => new List<RotCot_GenericBase>(){rotCot_AxisAngle, rotCot_Quaternion, rotCot_Euler, rotCot_Matrix};
        #endregion TypedRotationContainers
        #endregion Variables
        
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
                if (!TryGetComponent<UIDocument>(out uiDocument))
                {
                    uiDocument = gameObject.AddComponent<UIDocument>();
                    uiDocument.visualTreeAsset = fullScreenUIAsset; 
                    uiDocument.panelSettings = panelSettingsAsset;
                }
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
                Debug.LogError($"{name} could not find uiMenu");
                return; 
            }
            _uiMenu.dataSource = this;

            if (_uiRotSlot == null)
            {
                _uiRotSlot = _uiParent.Q<VisualElement>(uiRotSlotName);
                if (_uiRotSlot == null)
                {
                    Debug.LogError($"{name} could not find _uiRotSlot");
                    return; 
                }
            }
            _uiRotSlot.Add(_uiRoot);
            
            if (VisCamera ==null)
            {
                VisCamera = Instantiate(cameraPrefab, this.transform).transform.GetChild(0).GetComponent<Camera>();
                VisCamera.rect = cameraScreenRect; 
            }

            if (cameraRotationPivot == null)
            {
                cameraRotationPivot = VisCamera.transform.parent.gameObject; 
            }

            // InitializeRotCots();
            // _selectedTypeIndex = 0; 
            // activeRotCotType = typeof(RotCot_Euler); 
            // rotCot_Euler.enabled = true; 
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

        private void InitializeRotCots()
        {
            foreach (RotCot_GenericBase rotCot in rotCotsList)
            {
                rotCot.Initialize(this.transform, _uiRotSlot);
                rotCot.enabled = false; 
            }
        }
        #endregion Initialization
        
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
                        //+ZyKa set other RotCot enabled
                        break;
                    case 1:
                        //+ZyKa set other RotCot enabled
                        break;
                    case 2: 
                        //+ZyKa set other RotCot enabled
                        break;
                    case 3:
                        //+ZyKa set other RotCot enabled
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _selectedTypeIndex = value;
            }
        }
        #endregion UITypeSelectionControls

        private void SwitchRotParamType(System.Type newType)
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
                activeRotCot.enabled = false;
                activeRotCot = foundRotCot;
                foundRotCot.enabled = true;
                activeRotCotType = newType; 
            }
            else
            {
                Debug.LogError($"{name} cannot switch to RotCot of Type {newType}; no object of such type exists in this {nameof(ComboRotCot)}"); 
            }
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

        [ContextMenu("ApplyObjectRotation")]
        private void ApplyRotation()
        {
            activeRotCot.GetRotVis_Generic().ApplyObjectRotation();
        }

        [ContextMenu("ResetAppliedObjectRotation")]
        private void ResetAppliedObjectRotation()
        {
            activeRotCot.GetRotVis_Generic().ResetAppliedObjectRotation();
        }
        #endregion userInteraction
    }
}