using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class TestClass
{
    public TestClass(float testFloat)
    {
        this.testFloat = testFloat; 
        child = new ChildClass(this); 
    }

    [SerializeField] public float testFloat = -1; 
    [SerializeField] public ChildClass child;
}

[Serializable]
public class ChildClass
{
    public ChildClass(TestClass parent)
    {
        this.parent = parent;
        childTestFloat = parent.testFloat; 
    }
    [NonSerialized] public TestClass parent;    
    [SerializeField] public float childTestFloat; 
}