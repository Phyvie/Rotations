using System;
using System.ComponentModel;
using BaseClasses;
using RotObj;
using RotParams;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace RotContainers
{
    /*
     * generic (non-templated) baseclass for a single rotation, contains the generic getters and setters for RotParams, RotVis, RotUI and RotObjCot; the first two are only available as function, they are further specified in the templated child-class
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
        [SerializeField] protected OrientedObject orientedObject;

        public OrientedObject GetOrientedObject()
        {
            return orientedObject; 
        }
        public void SetOrientedObject(OrientedObject newOrientedObject)
        {
            this.orientedObject = newOrientedObject;
        }
        #endregion orientedObject
        
        #region RotVis
        public abstract RotVis_GenericBase GetRotVis_Generic();
        public abstract void SetRotVis_Generic(RotVis_GenericBase newRotVis);
        #endregion RotVis
        
        #region RotUI
        [SerializeField] protected VisualTreeAsset rotUIAsset;
        [SerializeField] protected VisualElement rotUIroot; 
        
        public VisualElement GetRotUI_Generic()
        {
            return rotUIroot; 
        }

        public void SetRotUI_Generic(VisualElement newRotUI)
        {
            rotUIroot.parent.Remove(rotUIroot);
            rotUIroot = newRotUI; 
        }
        #endregion RotUI
        #endregion VariablesAndGetSetProperties
        
        #region Initialize
        [SerializeField] private bool selfInitOnAwake = false;
        protected void Awake()
        {
            if (selfInitOnAwake)
            {
                InitRotObj(); 
            }
        }
        
        [SerializeField] private bool selfInitOnStart = true; 
        protected void Start()
        {
            if (selfInitOnStart)
            {
                InitRotObj(); 
            }
        }

        private void InitRotObj()
        {
            if (orientedObject == null)
            {
                orientedObject = Instantiate(rotObjCotPrefab, transform).GetComponent<OrientedObject>();
            }
        }

        public abstract void Initialize(Transform parent, VisualElement UIParent, OrientedObject orientedObject = null);
        #endregion Initialize
        
        
        protected void OnPropertyChangedVisUpdate(object sender, PropertyChangedEventArgs e)
        {
            orientedObject.SetRotation(GetRotParams_Generic().ToUnityQuaternion());
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
                rotParams.PropertyChanged -= OnPropertyChangedVisUpdate;
                // rotVisCS.SetRotParamsByRef(null); //cannot and should not be set to null
                rotUIroot.dataSource = null;

                if (value is TRotParams)
                {
                    rotParams = value; 
                }
                else
                {
                    Debug.LogWarning($"Settings {name}.{nameof(RotParams)} to a new Value which is of type {value.GetType().Name} which is not {typeof(TRotParams).Name}, must therefore convert which creates a new instance of RotParams of the correct Type)");
                    rotParams = rotParams.ToSelfType(value) as TRotParams; 
                }

                if (rotParams == null)
                {
                    enabled = false;
                    return; 
                }
                enabled = true;
                
                rotParams.PropertyChanged += OnPropertyChangedVisUpdate;
                RotVisScript.SetRotParamsByRef(rotParams);
                rotUIroot.dataSource = rotParams;
            }
        }

        public override RotParams_Base GetRotParams_Generic()
        {
            return RotParams;
        }
        
        public override void SetRotParams_Generic(RotParams_Base newRotParams)
        {
            if (newRotParams.GetType() == typeof(TRotParams))
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
            if (rotUIroot is not null) { rotUIroot.style.display = DisplayStyle.Flex; }
        }

        private void OnDisable()
        {
            RotVisScript?.gameObject.SetActive(false);
            if (rotUIroot is not null) { rotUIroot.style.display = DisplayStyle.None; }
        }
        #endregion EnableDisable

        #region InitializeAndSpawnFunctions
        public override void Initialize(Transform parent, VisualElement UIParent, OrientedObject orientedObject = null)
        {
            rotParams.PropertyChanged += OnPropertyChangedVisUpdate; 
            SpawnAndSetVis(parent);
            SpawnOrSetOrientedObject(orientedObject); 
            SpawnUI(UIParent);
        }
        
        private void SpawnAndSetVis(Transform parent, bool overwrite = false)
        {
            if (RotVisScript != null && overwrite)
            {
                /* TodoZyKa RotParams_Conversion: Do I need to destroy an existing visualisation to replace it with a new one? */
                #if UNITY_EDITOR
                Debug.LogWarning($"Respawning RotVis of TypedRotationContainer {name}");
                if (EditorApplication.isPlaying)
                {
                    Destroy(RotVisScript.gameObject); 
                }
                else
                {
                    DestroyImmediate(RotVisScript.gameObject);
                }
                #else
                Destroy(rotVisCS.gameObject); 
                #endif
            }
            GameObject newGO = Instantiate(rotVisPrefab, this.transform); 
            RotVisScript = newGO.GetComponent<TRotVis>(); 
            //RotVisScript.SetRotParamsByRef(rotParams) happens via the RotVisScript setter
        }
        
        private void SpawnOrSetOrientedObject(OrientedObject toSetOrientedObject)
        {
            if (toSetOrientedObject == null)
            {
                if (orientedObject == null)
                {
                    orientedObject = Instantiate(rotObjCotPrefab, transform).GetComponent<OrientedObject>();
                }
            }
            else
            {
                if (orientedObject != toSetOrientedObject && orientedObject != null)
                {
                    Debug.LogWarning($"Replacing rotObjCot on {name} with the one given from the initialize function");
                    orientedObject.gameObject.SetActive(false);
                    orientedObject.gameObject.name += "(deactivated by external initialisation)"; 
                    orientedObject = toSetOrientedObject;
                }
            }
        }
        
        public void SpawnUI(VisualElement parent)
        {
            if (rotUIroot != null)
            {
                rotUIroot.RemoveFromHierarchy();
            }
            rotUIroot = rotUIAsset.CloneTree();
            parent.Add(rotUIroot);
            rotUIroot.dataSource = rotParams; 
        }
        #endregion InitializeAndSpawnFunctions
    }
}