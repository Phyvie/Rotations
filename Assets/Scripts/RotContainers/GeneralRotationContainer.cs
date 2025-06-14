using System;
using System.Collections.Generic;
using System.Reflection;
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
        #region RuntimeVariables
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
        #endregion
        
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

        #region Initialization
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
                VisCamera = Instantiate(cameraPrefab, this.transform).GetComponent<Camera>();
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
    }
}
