using BaseClasses;
using UnityEngine;

public class RotParent<TRotParams, TRotVis, TRotUI> : MonoBehaviour 
    where TRotParams : RotParams.RotParams, new() 
    where TRotVis : RotVis<TRotParams>, new() 
    where TRotUI : RotUI.RotUI<TRotParams>, new()
{
    public TRotParams rotParams;
    public TRotVis rotVis;
    public TRotUI rotUI;

    //!ZyKa this is absolute non-sense it won't work, because you are spawning components which don't have any RotParams connected to them; 
    // My Suggestion: you first finish the UI and connect the UI to its own RotParams & then you figure out how to connect the things to one another
    public void Init()
    {
        rotParams = new TRotParams();
        rotVis = gameObject.AddComponent<TRotVis>();
        rotUI = gameObject.AddComponent<TRotUI>(); 

        rotVis.RotParams = rotParams;
        rotUI.RotParams = rotParams;
    }

    private void Awake()
    {
        Init();
    }
}
