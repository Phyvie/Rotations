using BaseClasses;
using UnityEngine;
using UnityEngine.UIElements;

public class TypedRotationContainer<TRotParams, TRotVis> : MonoBehaviour 
    where TRotParams : RotParams.RotParams, new()
    where TRotVis : RotVis<TRotParams>
{
    public TRotParams rotParams;

    public GameObject rotVisRootObject; 
    public TRotVis rotVis;
    public GameObject rotVisPrefab; 
    
    public VisualElement rotUI; 
    public VisualTreeAsset rotUIPrefab;
    
    public virtual void Init(VisualElement UIParent)
    {
        rotParams = new TRotParams();
        InitVis();
        InitUI(UIParent); 
    }

    public void InitVis()
    {
        if (rotVis is null)
        {
            if (rotVisRootObject != null)
            {
                Destroy(rotVisRootObject);
            }
            rotVisRootObject = Instantiate(rotVisPrefab, this.transform, true);
            rotVis = rotVisRootObject.GetComponent<TRotVis>();
        }
        rotVis.RotParams = rotParams;
    }

    public void InitUI(VisualElement parent)
    {
        VisualElement rotUI = rotUIPrefab.CloneTree();
        parent.Add(rotUI); 
    }
}
