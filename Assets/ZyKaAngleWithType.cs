using RotParams;
using UnityEngine;
using UnityEngine.UIElements;

public class ZyKaAngleWithType : MonoBehaviour
{
    [SerializeField] private UIDocument uiDoc; 
    [SerializeField] private AngleWithType angleWithType;

    private void Awake()
    {
        if (uiDoc != null)
        {
            uiDoc.rootVisualElement.dataSource = angleWithType;
        }
    }
}
