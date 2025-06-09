using BaseClasses;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotContainers
{
    public class TypedRotationContainer : MonoBehaviour
    {
        [SerializeField] private RotParams.RotParams rotParams; 
        [SerializeField] private GameObject rotVisGO;
        [SerializeField] private RotVis rotVis; 
        private VisualElement rotUI; 
        
        public void SpawnTypedRotation(ref RotParams.RotParams newRotParams, GameObject rotVisPrefab, Transform rotVisParent, VisualTreeAsset visualTreeAsset, VisualElement visualParent)
        {
            rotParams = newRotParams;
            SpawnVis(rotVisPrefab, this.transform);
            SpawnUI(visualTreeAsset, visualParent);
        }
        
        public void SpawnVis(GameObject prefab, Transform parent)
        {
            if (rotVisGO != null)
            {
                Destroy(rotVisGO);
            }
            rotVisGO = Instantiate(prefab, parent); 
            rotVis = prefab.GetComponent<RotVis>();
            rotVis.SetRotParams(rotParams); 
        }

        public void SpawnUI(VisualTreeAsset visualTreeAsset, VisualElement parent)
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
