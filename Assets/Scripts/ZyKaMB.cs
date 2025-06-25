using System.Collections.Generic;
using Extensions.MathExtensions;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class ZyKaMB : MonoBehaviour
{
    [SerializeField]
    private UIDocument _document;
    
    [SerializeField] private LockableVector zyKaLockableVector = 
        new LockableVector(new List<LockableFloat>()
        {
            new LockableFloat(0, false), 
            new LockableFloat(1, true),
        }); 
    
    [CreateProperty]
    public float ZyKaFloatX
    {
        get => zyKaLockableVector[0];
        set => zyKaLockableVector.SetFloatValue(0, value); 
    }

    [CreateProperty]
    public float ZyKaFloatY
    {
        get => zyKaLockableVector[1];
        set => zyKaLockableVector.SetFloatValue(1, value); 
    }

    private void Start()
    {
        _document.rootVisualElement.dataSource = this; 
    }
}
