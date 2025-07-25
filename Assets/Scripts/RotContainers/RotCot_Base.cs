using System;
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
        [SerializeField] protected GameObject rotObjCotPrefab; 
        [SerializeField] protected RotObjCot rotObjCot;

        public RotObjCot GetRotObjCot()
        {
            return rotObjCot; 
        }
        public void SetRotObjCot(RotObjCot rotObjCot)
        {
            this.rotObjCot = rotObjCot;
        }
        
        public abstract RotParams_Base GetRotParams_Generic();
        public abstract void SetRotParams_Generic(RotParams_Base rotParams);

        public abstract RotVis_GenericBase GetRotVis_Generic();
        public abstract void SetRotVis_Generic(RotVis_GenericBase newRotVis);

        public abstract VisualElement GetRotUI_Generic();
        public abstract void SetRotUI_Generic(VisualElement newRotUI);
        
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
        
        [SerializeField] private VisualTreeAsset rotUIAsset;
        [SerializeField] private VisualElement rotUIroot; 
        
        public TRotParams RotParams
        {
            get => rotParams;
            set
            {
                rotParams = value; 
                if (value != null)
                {
                    enabled = true;
                    rotVisCS.SetRotParamsByRef(rotParams);
                    rotUIroot.dataSource = rotParams; 
                }
                else
                {
                    enabled = false; 
                }
            }
        }

        public override RotParams_Base GetRotParams_Generic()
        {
            return RotParams;
        }

        public override void SetRotParams_Generic(RotParams_Base rotParams)
        {
            if (rotParams.GetType() == typeof(TRotParams))
            {
                RotParams = rotParams as TRotParams; 
            }
            
            RotParams = typeof(TRotParams) switch
            {
                Type t when t == typeof(RotParams_EulerAngles)
                    => (TRotParams)(object)rotParams.ToEulerParams(),

                Type t when t == typeof(RotParams_Quaternion)
                    => (TRotParams)(object)rotParams.ToQuaternionParams(),

                Type t when t == typeof(RotParams_AxisAngle)
                    => (TRotParams)(object)rotParams.ToAxisAngleParams(),

                Type t when t == typeof(RotParams_Matrix)
                    => (TRotParams)(object)rotParams.ToMatrixParams(),

                _ => throw new InvalidOperationException($"Unsupported TRotParams type: {typeof(TRotParams).Name}")
            };
        }

        public override VisualElement GetRotUI_Generic()
        {
            return rotUIroot; 
        }

        public override void SetRotUI_Generic(VisualElement newRotUI)
        {
            rotUIroot.parent.Remove(rotUIroot);
            rotUIroot = newRotUI; 
        }

        public override RotVis_GenericBase GetRotVis_Generic()
        {
            return rotVisCS; 
        }

        public override void SetRotVis_Generic(RotVis_GenericBase newRotVis)
        {
            rotVisCS = newRotVis as TRotVis;
        }

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

        public override void Initialize(Transform parent, VisualElement UIParent, RotObjCot toSetRotObjCot = null)
        { 
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
    }
}