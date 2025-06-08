using UnityEngine;
using UnityEngine.UIElements;

public class GeneralRotationContainer : MonoBehaviour
{
    [SerializeField] private UIDocument uiParent;
    
    public void InitTypedRotation<T>() where T : TypedRotationContainer<>
    {
        uiParent.rootVisualElement.Add(); 
    }
}
