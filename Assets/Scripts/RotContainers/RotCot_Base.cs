using System;
using System.ComponentModel;
using BaseClasses;
using RotObj;
using RotParams;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotContainers
{
    /* absolute baseclass for a single rotation, contains the generic getters and setters for RotParams, RotVis, RotUI and RotObjCot; the first two are only available as function, they are further specified in the templated child-class
     */
    
    public abstract class RotCot_GenericBase : MonoBehaviour
    {
        #region VariablesAndGetSetProperties
        #region RotParams
        public abstract RotParams_Base GetRotParams_Generic();
        public abstract void SetRotParams_Generic(RotParams_Base newRotParams);
        #endregion RotParams
        
        #region RotObj
        [SerializeField] protected GameObject rotObjCotPrefab; 
        [SerializeField] protected RotObjCot rotObjCot;

        public RotObjCot GetRotObjCot()
        {
            return rotObjCot; 
        }
        public void SetRotObjCot(RotObjCot newRotObjCot)
        {
            this.rotObjCot = newRotObjCot;
        }
        #endregion RotObj
        
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
            if (rotObjCot == null)
            {
                rotObjCot = Instantiate(rotObjCotPrefab, transform).GetComponent<RotObjCot>();
            }
        }

        public abstract void Initialize(Transform parent, VisualElement UIParent, RotObjCot toSetRotObjCot = null);
        #endregion Initialize
        
        
        protected void OnPropertyChangedVisUpdate(object sender, PropertyChangedEventArgs e)
        {
            rotObjCot.SetRotation(GetRotParams_Generic().ToUnityQuaternion());
        }
    }
    
    /*
     * ChildClass to RotCot_Generic, which contains the actual Variables and implements the generic getters/setters for the variables
     */
    public abstract class RotCot_Base<TRotParams, TRotVis> : RotCot_GenericBase where TRotParams : RotParams_Base where TRotVis : RotVis_GenericBase
    {
        #region Variables
        [SerializeField] private TRotParams rotParams;

        [SerializeField] private GameObject rotVisPrefab; 
        [SerializeField] private TRotVis rotVisCS;
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
                rotVisCS.SetRotParamsByRef(rotParams);
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

        public override RotVis_GenericBase GetRotVis_Generic()
        {
            return rotVisCS; 
        }

        public override void SetRotVis_Generic(RotVis_GenericBase newRotVis)
        {
            rotVisCS = newRotVis as TRotVis;
        }
        #endregion Properties

        #region EnableDisable
        private void OnEnable()
        {
            rotVisCS?.gameObject.SetActive(true);
            if (rotUIroot is not null) { rotUIroot.style.display = DisplayStyle.Flex; }
        }

        private void OnDisable()
        {
            rotVisCS?.gameObject.SetActive(false);
            if (rotUIroot is not null) { rotUIroot.style.display = DisplayStyle.None; }
        }
        #endregion EnableDisable

        #region InitializeAndSpawnFunctions
        public override void Initialize(Transform parent, VisualElement UIParent, RotObjCot toSetRotObjCot = null)
        {
            rotParams.PropertyChanged += OnPropertyChangedVisUpdate; 
            SpawnVis(parent);
            SpawnRotObjCot(toSetRotObjCot); 
            SpawnUI(UIParent);
        }
        
        public void SpawnVis(Transform parent)
        {
            if (rotVisCS != null)
            {
                #if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                {
                    Debug.LogWarning($"Respawning RotVis of TypedRotationContainer {name}");
                    Destroy(rotVisCS.gameObject); 
                }
                else
                {
                    Debug.LogWarning($"Respawning RotVis of TypedRotationContainer {name}");
                    DestroyImmediate(rotVisCS.gameObject);
                }
                #else
                Destroy(rotVisCS.gameObject); 
                #endif
            }
            GameObject newGO = Instantiate(rotVisPrefab, this.transform); 
            rotVisCS = newGO.GetComponent<TRotVis>();
            rotVisCS.SetRotParamsByRef(rotParams);
        }
        
        public void SpawnRotObjCot(RotObjCot toSetRotObjCot)
        {
            if (toSetRotObjCot == null)
            {
                if (rotObjCot == null)
                {
                    rotObjCot = Instantiate(rotObjCotPrefab, transform).GetComponent<RotObjCot>();
                }
            }
            else
            {
                if (rotObjCot != toSetRotObjCot && rotObjCot != null)
                {
                    Debug.LogWarning($"Replacing rotObjCot on {name} with the one given from the initialize function");
                    rotObjCot.gameObject.SetActive(false);
                    rotObjCot.gameObject.name += "(deactivated by external initialisation)"; 
                    rotObjCot = toSetRotObjCot;
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