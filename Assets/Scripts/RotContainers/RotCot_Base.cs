using System;
using System.ComponentModel;
using BaseClasses;
using Packages.UnityExtensionMethods;
using RotObj;
using RotParams;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotContainers
{
    /*
     * generic (non-template) baseclass for a single rotation, contains the generic getters and setters for RotParams, RotVis, RotUI and RotObjCot; the first two are only available as function, they are further specified in the templated child-class
     */
    public abstract class RotCot_GenericBase : MonoBehaviour
    {
        #region VariablesAndGetSetProperties
        #region RotParams
        public abstract RotParams_Base GetRotParams_Generic();
        public abstract void SetRotParams_Generic(RotParams_Base newRotParams);
        #endregion RotParams
        
        #region orientedObject
        [SerializeField] protected GameObject rotObjCotPrefab; 
        [SerializeField] private OrientedObject orientedObject;

        public OrientedObject OrientedObject
        {
            get => orientedObject;
            set
            {
                if (orientedObject != null && GetRotParams_Generic() != null)
                {
                    GetRotParams_Generic().PropertyChanged -= UpdateOrientedObject;
                }
                orientedObject = value;
                if (enabled && GetRotParams_Generic() != null)
                {
                    GetRotParams_Generic().PropertyChanged += UpdateOrientedObject;
                }
            }
        }

        #endregion orientedObject
        
        #region RotVis
        public abstract RotVis_GenericBase GetRotVis_Generic();
        public abstract void SetRotVis_Generic(RotVis_GenericBase newRotVis);
        #endregion RotVis
        
        #region RotUI
        
        [SerializeField] private UIDocument uiDocument; 
        public UIDocument UIDocument
        {
            get => uiDocument;
            set => uiDocument = value;
        }

        [SerializeField] private VisualTreeAsset UIAsset;
        [SerializeField] private PanelSettings panelSettingsAsset;

        [SerializeField] protected string uiParentName;
        protected VisualElement _uiParent;
        
        [SerializeField] protected VisualTreeAsset rotUIAsset;
        protected VisualElement rotUIRoot;
        protected StyleEnum<DisplayStyle> _rotUIDisplayStyle; 
        
        public VisualElement GetRotUI_Generic()
        {
            return rotUIRoot; 
        }

        public void SetRotUI_Generic(VisualElement newRotUI)
        {
            if (rotUIRoot != null && rotUIRoot.parent != null)
            {
                rotUIRoot.dataSource = null; 
                rotUIRoot.RemoveFromHierarchy(); 
            }
            rotUIRoot = newRotUI;
            rotUIRoot.dataSource = GetRotParams_Generic();
        }
        #endregion RotUI
        #endregion VariablesAndGetSetProperties
        
        #region Initialize
        [SerializeField] private MonoBehaviourFunctions initMode; 
        protected void Awake()
        {
            if (initMode == MonoBehaviourFunctions.Awake)
            {
                SelfInitialize(); 
            }

            ResetToIdentity(); 
        }
        
        protected void Start()
        {
            if (initMode == MonoBehaviourFunctions.Start)
            {
                SelfInitialize(); 
            }
        }

        private bool isInitialized = false; 
        private void SelfInitialize()
        {
            if (isInitialized)
            {
                Debug.LogWarning($"{name} is already initialized");
                return; 
            }
            Initialize(null);
            isInitialized = true; 
        }
        public abstract void Initialize(Transform parent, VisualElement UIParent = null, OrientedObject newOrientedObject = null);
        #endregion Initialize

        public abstract void ResetToIdentity(); 
        
        protected void UpdateOrientedObject(object sender, PropertyChangedEventArgs e)
        {
            if (OrientedObject != null && GetRotParams_Generic() != null)
            {
                OrientedObject.SetRotation(GetRotParams_Generic().ToUnityQuaternion());
            }
        }
    }
    
    /*
     * ChildClass to RotCot_Generic, which contains the actual Variables and implements the generic getters/setters for the variables
     * Reason for the double-abstraction:
     * A) The Template_Base can do type-safety checks in the setters of RotParams and RotVis
     * B) The Generic_Base can be stored in a list
     */
    public abstract class RotCot_TemplateBase<TRotParams, TRotVis> : RotCot_GenericBase where TRotParams : RotParams_Base where TRotVis : RotVis_GenericBase
    {
        #region Variables
        [SerializeField] private TRotParams rotParams;

        [SerializeField] private GameObject rotVisPrefab; 
        [SerializeField] private TRotVis rotVisScript;
        #endregion
        
        #region Properties
        public TRotParams RotParams
        {
            get => rotParams;
            set
            {
                if (rotParams != null)
                {
                    rotParams.PropertyChanged -= UpdateOrientedObject;
                }
                if (rotUIRoot != null)
                {
                    rotUIRoot.dataSource = null;
                }

                rotParams = value;

                RotVisScript?.SetRotParamsByRef(rotParams);
                if (rotUIRoot != null)
                {
                    rotUIRoot.dataSource = rotParams;
                }
                if (rotParams != null && enabled)
                {
                    rotParams.PropertyChanged += UpdateOrientedObject;
                }
            }
        }

        public override RotParams_Base GetRotParams_Generic()
        {
            return RotParams;
        }
        
        public override void SetRotParams_Generic(RotParams_Base newRotParams)
        {
            if (newRotParams.GetType().IsAssignableFrom(typeof(TRotParams)))
            {
                RotParams = newRotParams as TRotParams; 
            }
            else
            {
                throw new ArgumentException(); 
            }
        }

        
        public TRotVis RotVisScript
        {
            get => rotVisScript;
            set
            {
                rotVisScript = value;
                rotVisScript.SetRotParamsByRef(rotParams);
            }
        }

        public override RotVis_GenericBase GetRotVis_Generic()
        {
            return RotVisScript; 
        }

        public override void SetRotVis_Generic(RotVis_GenericBase newRotVis)
        {
            RotVisScript = newRotVis as TRotVis;
        }
        #endregion Properties

        #region EnableDisable
        private void OnEnable()
        {
            RotVisScript?.gameObject.SetActive(true);
            
            if (rotParams is not null)
            {
                rotParams.PropertyChanged -= UpdateOrientedObject; // Ensure no double subscription
                rotParams.PropertyChanged += UpdateOrientedObject;
            }

            if (rotUIRoot is not null)
            {
                rotUIRoot.style.display = _rotUIDisplayStyle;
            }
            
            if (OrientedObject is not null)
            {
                OrientedObject.SetRotation(RotParams.ToUnityQuaternion());
            }
        }

        private void OnDisable()
        {
            RotVisScript?.gameObject.SetActive(false);
            
            if (rotParams is not null)
            {
                rotParams.PropertyChanged -= UpdateOrientedObject; 
            }

            if (rotUIRoot is not null)
            {
                rotUIRoot.style.display = DisplayStyle.None;
            }
        }
        #endregion EnableDisable
        
        #region InitializeAndSpawnFunctions
        public override void Initialize(Transform parent, VisualElement uiParent, OrientedObject newOrientedObject = null)
        {
            SpawnAndSetRotVis(parent);
            SpawnOrSetOrientedObject(newOrientedObject);
            InitUI(uiParent); 
        }
        
        private void SpawnAndSetRotVis(Transform parent, bool overwrite = false)
        {
            if (RotVisScript != null && overwrite)
            {
                #if UNITY_EDITOR //required for build compilation
                if (EditorApplication.isPlaying)
                {
                    Destroy(RotVisScript.gameObject); 
                }
                else
                {
                    DestroyImmediate(RotVisScript.gameObject);
                }
                #else
                Destroy(RotVisScript.gameObject); 
                #endif
            }

            if (RotVisScript == null)
            {
                GameObject newGO = Instantiate(rotVisPrefab, this.transform); 
                RotVisScript = newGO.GetComponent<TRotVis>(); 
            }
            RotVisScript.SetRotParamsByRef(rotParams); 
        }
        
        private void SpawnOrSetOrientedObject(OrientedObject toSetOrientedObject = null)
        {
            if (toSetOrientedObject == null)
            {
                if (OrientedObject == null)
                {
                    OrientedObject = Instantiate(rotObjCotPrefab, transform).GetComponent<OrientedObject>();
                }
            }
            else
            {
                if (OrientedObject != toSetOrientedObject && OrientedObject != null)
                {
                    Debug.LogWarning($"Replacing rotObjCot on {name} with the one given from the initialize function");
                    OrientedObject.gameObject.SetActive(false);
                    OrientedObject.gameObject.name += "(deactivated due to external initialization of RotCot)"; 
                    OrientedObject = toSetOrientedObject;
                }
            }
        }

        public void InitUI(VisualElement externalParent = null)
        {
            //external initialization takes precedence over self-initialization; However, warnings are printed if the external initialization uses values different from the self-initialisation
            if (externalParent != null)
            {
                if (_uiParent != null && _uiParent.parent != externalParent)
                {
                    Debug.LogWarning($"{name} external {nameof(InitUI)} uses different uiParent than self-initialisation. External initialization takes precedence.");
                    if (_uiParent.panel.visualTree.GetFirstAncestorOfType<UIDocument>() != UIDocument)
                    {
                        Debug.LogWarning($"{name} external {nameof(InitUI)} uses different uiDocument than self-initialization. ");
                    }
                }
                _uiParent = externalParent; 
            }
            else
            {
                if (UIDocument == null)
                {
                    Debug.LogError($"{name} has no UIDocument set during UI-Initialization");
                    return; 
                }
                _uiParent = UIDocument.rootVisualElement.Q<VisualElement>(uiParentName);
                if (_uiParent == null)
                {
                    Debug.LogError($"{name} could not find UI-Parent with name {uiParentName} during UI-Initialization");
                    return; 
                }
            }

            SpawnUI(); 
        }
        
        private void SpawnUI()
        {
            if (rotUIRoot != null)
            {
                Debug.LogWarning($"{name} has already been initialized with a rotUIRoot. {nameof(SpawnUI)} removes old rotUIRoot and replaces with newly spawned.");
                rotUIRoot.RemoveFromHierarchy();
                rotUIRoot.dataSource = null; 
            }
            rotUIRoot = rotUIAsset.CloneTree();
            _uiParent.Add(rotUIRoot);
            rotUIRoot.dataSource = rotParams; 
            _rotUIDisplayStyle = rotUIRoot.style.display;
        }
        #endregion InitializeAndSpawnFunctions

        [ContextMenu(nameof(ResetToIdentity))]
        public override void ResetToIdentity()
        {
            rotParams?.ResetToIdentity();
        }
    }
}