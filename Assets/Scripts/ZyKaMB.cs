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
            new LockableFloat(1, false),
            new LockableFloat(0, false)
        });

    [CreateProperty]
    public Vector3 ZyKaVector3
    {
        get => new Vector3(zyKaLockableVector[0], zyKaLockableVector[1], zyKaLockableVector[2]);
        set
        {
            zyKaLockableVector.SetVector3(value);
        }
    }

    private void Start()
    {
        _document.rootVisualElement.dataSource = this; 
    }
}
