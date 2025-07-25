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
    public class ComboRotCot : MonoBehaviour
    {
        #region Variables
        [SerializeField] private bool InitializeOnAwake = false;
        [SerializeField] private bool InitializeOnStart = true; 
        #endregion Variables
        
        #region RotObjCot
        [SerializeField] private GameObject rotObjCotPrefab; 
        [SerializeField] private RotObjCot rotObjCot; 
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

        [FormerlySerializedAs("uiParentName")]
        [Tooltip("This is not the object which contains uiMenu & UIRotSlot; it is the visualElement into which the uiRoot will be spawned")]
        [SerializeField] private string uiComboContainerParentName; 
        private VisualElement _comboContainerUIParent; //the slot into which the _uiRoot will be spawned

        [FormerlySerializedAs("uiFullContainerAsset")]
        [Tooltip("This is the VisualElement which actually contains uiMenuLine & RotParamsSlot")]
        [SerializeField] private VisualTreeAsset uiComboContainerRootAsset;
        private VisualElement _comboContainerRoot; //the actual root containing the menuLine + UIRotSlot
        
        [FormerlySerializedAs("uiMenuAsset")] [SerializeField] private VisualTreeAsset uiMenuLineAsset;
        [FormerlySerializedAs("uiMenuName")] [SerializeField] private string uiMenuLineName = "uiMenuLine"; 
        private VisualElement _uiMenuLine;

        [FormerlySerializedAs("uiRotSlotName")] [SerializeField] private string uiRotParamsSlot = "RotParamsSlot"; 
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
        
        #region TypedRotCots
        private RotCot_GenericBase activeRotCot;
        private System.Type activeRotCotType; 
        
        [SerializeField] private RotCot_AxisAngle rotCot_AxisAngle; 
        [SerializeField] private RotCot_Quaternion rotCot_Quaternion; 
        [SerializeField] private RotCot_Euler rotCot_Euler;
        [SerializeField] private RotCot_Matrix rotCot_Matrix;
        
        private List<RotCot_GenericBase> rotCotsList => new List<RotCot_GenericBase>(){rotCot_AxisAngle, rotCot_Quaternion, rotCot_Euler, rotCot_Matrix};
        #endregion TypedRotCots

        public RotParams_Base ActiveRotParams_Generic
        {
            get => activeRotCot.GetRotParams_Generic(); 
            set => activeRotCot.SetRotParams_Generic(value);
        }
        
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
            InitRotObjCot();
            
            InitializeRotCots(); 
            _selectedTypeIndex = 1; 
            ActivateRotCot(rotCot_Quaternion);
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
                    uiDocument.rootVisualElement :  
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

        private void InitRotObjCot()
        {
            if (rotObjCot == null)
            {
                rotObjCot = Instantiate(rotObjCotPrefab, this.transform).GetComponent<RotObjCot>(); //!ZyKa this where the bug happens
            }
        }
        
        private void InitializeRotCots()
        {
            foreach (RotCot_GenericBase rotCot in rotCotsList)
            {
                rotCot.Initialize(this.transform, _uiRotParamsSlot, rotObjCot);
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
                        SwitchRotParamType(typeof(RotCot_AxisAngle));
                        break;
                    case 1:
                        SwitchRotParamType(typeof(RotCot_Quaternion));
                        break;
                    case 2: 
                        SwitchRotParamType(typeof(RotCot_Euler));
                        break;
                    case 3:
                        SwitchRotParamType(typeof(RotCot_Matrix));
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
                RotParams_Base rotParams = activeRotCot.GetRotParams_Generic(); 
                DeactiveRotCot();
                ActivateRotCot(foundRotCot);
                activeRotCot.SetRotParams_Generic(rotParams);
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
            rotObjCot.ApplyObjectRotation();
        }

        public RotParams_Base GetAppliedRotation()
        {
            //!ZyKa fix this to use the actual class of the current RotParams
            return new RotParams_Quaternion(rotObjCot.GetAppliedRotation()); //!ZyKa proper conversion functions 
        }
        
        [ContextMenu("ResetAppliedObjectRotation")]
        public void ResetAppliedObjectRotation()
        {
            rotObjCot.ResetAppliedObjectRotation(); 
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
