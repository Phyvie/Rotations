using System;
using System.Collections.Generic;
using RotParams;
using UI_Toolkit;
using Unity.Properties;
using UnityEngine;
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
        
        [SerializeField] private RotParams_Base _rotParams;

        public RotParams_Base RotParams
        {
            get => _rotParams;
            set
            {
                _rotParams = value; 
                GenerateRotationContainer(_rotParams);
            }
        }

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
        #endregion Variables
        
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
        private int SelectedTypeIndex //+ZyKa update this so that it doesn't generate a new Parametrization but a conversion of the Parametrization
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

        public TypedRotationContainerPrefab GetTypedRotationContainerPrefab(System.Type type)
        {
            var method = typeof(GeneralRotationContainer).GetMethod(nameof(GetTypedRotationContainerPrefab), Type.EmptyTypes);
            var genericMethod = method.MakeGenericMethod(type);
            return (TypedRotationContainerPrefab)genericMethod.Invoke(this, null);
        }
        
        public TypedRotationContainerPrefab GetTypedRotationContainerPrefab<RotParamsType>()
        {
            return typeof(RotParamsType) switch
                {
                    Type t when t == typeof(RotParams_EulerAngles) => eulerPrefab,
                    Type t when t == typeof(RotParams_Quaternion) => quaternionPrefab,
                    Type t when t == typeof(RotParams_AxisAngle) => axisAnglePrefab,
                    { } t when t == typeof(RotParams_Matrix) => matrixPrefab,
                    _ => null
                };
        }
        
        public void GenerateContainerForRotParams(RotParams_Base newRotParams)
        {
            _rotParams = newRotParams;
            if (typedRotationContainer == null)
            {
                typedRotationContainer = gameObject.AddComponent<TypedRotationContainer>();
            }
            GenerateRotationContainer(_rotParams); 
        }
        
        public void GenerateNewRotation<RotParamsType>() where RotParamsType : RotParams_Base, new()
        {
            _rotParams = new RotParamsType();
            GenerateRotationContainer(_rotParams);
        }

        private void GenerateRotationContainer(RotParams_Base rotParams)
        {
            if (typedRotationContainer == null)
            {
                typedRotationContainer = gameObject.AddComponent<TypedRotationContainer>();
            }
            TypedRotationContainerPrefab prefab = GetTypedRotationContainerPrefab(rotParams.GetType());
            if (prefab == null)
            {
                throw new NullReferenceException();
            }
            typedRotationContainer.SpawnTypedRotation(ref _rotParams, prefab.visPrefab, this.transform, prefab.uiPrefab, uiRotSlot); 
        }
        
        //accessor function for the templated version
        public void GenerateNewRotationGeneric(System.Type type)
        {
            var method = typeof(GeneralRotationContainer).GetMethod(nameof(GenerateNewRotation), Type.EmptyTypes);
            var genericMethod = method.MakeGenericMethod(type);
            genericMethod.Invoke(this, null);
        }
        
        private void GenerateNewMatrix()
        {
            GenerateNewRotation<RotParams_Matrix>(); 
        }
        
        private void GenerateNewAxisAngle()
        {
            GenerateNewRotation<RotParams_AxisAngle>(); 
        }
        
        private void GenerateNewQuaternion()
        {
            GenerateNewRotation<RotParams_Quaternion>(); 
        }
        
        private void GenerateNewEulerAngles()
        {
            GenerateNewRotation<RotParams_EulerAngles>();
        }
        #endregion ChangeRotationType

        #if UNITY_EDITOR
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
        #endif

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
            typedRotationContainer.RotVis.ApplyObjectRotation();
        }

        [ContextMenu("ResetAppliedObjectRotation")]
        private void ResetAppliedObjectRotation()
        {
            typedRotationContainer.RotVis.ResetAppliedObjectRotation();
        }
        #endregion userInteraction
    }
}
