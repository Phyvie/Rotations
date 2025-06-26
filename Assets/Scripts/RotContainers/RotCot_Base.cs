using System;
using BaseClasses;
using RotParams;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotContainers
{
    public abstract class RotCot_GenericBase : MonoBehaviour
    {
        public abstract RotParams_Base GetRotParams_Generic();
        public abstract void SetRotParams_Generic(RotParams_Base rotParams);

        public abstract RotVis_Base GetRotVis_Generic();
        
        public abstract void SetRotVis_Generic(RotVis_Base newRotVis);
        public abstract void Initialize(Transform parent, VisualElement UIParent);
    }
    
    public abstract class RotCot_Base<TRotParams, TRotVis> : RotCot_GenericBase where TRotParams : RotParams_Base where TRotVis : RotVis_Base
    {
        [SerializeField] private TRotParams rotParams;

        [SerializeField] private GameObject rotVisPrefab; 
        [SerializeField] private TRotVis rotVisCS;

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
            RotParams = typeof(TRotParams) switch
            {
                Type t when t == typeof(RotParams_EulerAngles)
                    => (TRotParams)(object)rotParams.ToEulerAngleRotation(),

                Type t when t == typeof(RotParams_Quaternion)
                    => (TRotParams)(object)rotParams.ToQuaternionRotation(),

                Type t when t == typeof(RotParams_AxisAngle)
                    => (TRotParams)(object)rotParams.ToAxisAngleRotation(),

                Type t when t == typeof(RotParams_Matrix)
                    => (TRotParams)(object)rotParams.ToMatrixRotation(),

                _ => throw new InvalidOperationException($"Unsupported TRotParams type: {typeof(TRotParams).Name}")
            };
        }

        public override RotVis_Base GetRotVis_Generic()
        {
            return rotVisCS; 
        }

        public override void SetRotVis_Generic(RotVis_Base newRotVis)
        {
            rotVisCS = newRotVis as TRotVis;
        }

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

        public override void Initialize(Transform parent, VisualElement UIParent)
        {
            SpawnVis(parent);
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
                Destroy(rotVisGO); 
                #endif
            }
            GameObject newGO = Instantiate(rotVisPrefab, this.transform); 
            rotVisCS = newGO.GetComponent<TRotVis>();
            rotVisCS.SetRotParamsByRef(rotParams);
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