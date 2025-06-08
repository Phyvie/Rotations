using BaseClasses;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotContainers
{
    public class TypedRotationContainer : MonoBehaviour
    {
        [SerializeField] private GameObject rotVisGO;
        [SerializeField] private RotVis rotVis; 
        private VisualElement rotUI; 
        
        public void SpawnTypedRotation(ref RotParams.RotParams rotParams, GameObject rotVisPrefab, Transform rotVisParent, VisualTreeAsset visualTreeAsset, VisualElement visualParent)
        {
            SpawnVis(rotVisPrefab, ref rotParams, this.transform);
            SpawnUI(visualTreeAsset, ref rotParams, visualParent);
        }
        
        public void SpawnVis(GameObject prefab, ref RotParams.RotParams rotParams, Transform parent)
        {
            if (rotVisGO != null)
            {
                Destroy(rotVisGO);
            }
            rotVisGO = Instantiate(prefab, parent); 
            rotVis = prefab.GetComponent<RotVis>();
            rotVis.SetRotParams(rotParams); 
        }

        public void SpawnUI(VisualTreeAsset visualTreeAsset, ref RotParams.RotParams rotParams, VisualElement parent)
        {
            if (rotUI != null)
            {
                rotUI.RemoveFromHierarchy();
            }
            rotUI = visualTreeAsset.CloneTree();
            parent.Add(rotUI);
            rotUI.dataSource = rotParams; 
        }
    }
}
