using System.Linq;
using BaseClasses;
using RotParams;
using UI_Toolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotContainers
{
    public class TypedRotationContainer : MonoBehaviour
    {
        [SerializeReference] private RotParams_Base rotParams; //-ZyKa make a public property to change the RotParams, currently they can only be set via SpawnTypedRotation
        [Tooltip("Do not set this reference manually, it will be spawned from the outside by GeneralRotationContainer")] //-ZyKa RotationContainers; enable setting this manually & ensure that the rotParams are updated accordingly
        [SerializeField] private GameObject rotVisGO;
        [Tooltip("Do not set this reference manually, it will be spawned from the outside by GeneralRotationContainer")]
        [SerializeField] private RotVis_Base rotVis;
        private VisualElement rotUI;
        
        public RotVis_Base RotVis => rotVis;
        public VisualElement RotUI => rotUI;
        
        public void SpawnTypedRotation(ref RotParams_Base newRotParams, GameObject rotVisPrefab, Transform rotVisParent, VisualTreeAsset visualTreeAsset, UISlotReference visualParent)
        {
            rotParams = newRotParams;
            SpawnVis(rotVisPrefab, this.transform);
            SpawnUI(visualTreeAsset, visualParent);
        }
        
        public void SpawnVis(GameObject prefab, Transform parent)
        {
            if (rotVisGO != null)
            {
                #if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                {
                    Destroy(rotVisGO);   
                }
                else
                {
                    DestroyImmediate(rotVisGO);
                }
                #else
                Destroy(rotVisGO); 
                #endif
            }
            rotVisGO = Instantiate(prefab, parent); 
            rotVis = rotVisGO.GetComponent<RotVis_Base>();
            rotVis.SetRotParamsByRef(rotParams); 
        }

        public void SpawnUI(VisualTreeAsset visualTreeAsset, UISlotReference parent)
        {
            if (rotUI != null)
            {
                rotUI.RemoveFromHierarchy();
            }
            rotUI = visualTreeAsset.CloneTree();
            parent.PlaceInSlot(rotUI);
            rotUI.dataSource = rotParams; 
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            /* !ZyKa OnValidate
            if (EditorApplication.isPlayingOrWillChangePlaymode && !Application.isPlaying)
            {
                return; 
            }
            rotVis.VisUpdate();
            */   
        }
        #endif
    }
}
