using RotParams;
using UnityEngine;
using UnityEngine.UIElements;

public class ZyKaAngleWithType : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument; 
    [SerializeField] private AngleWithType angleWithType;

    private void Awake()
    {
        if (uiDocument != null)
        {
            uiDocument.rootVisualElement.dataSource = this; 
        }
    }
}
