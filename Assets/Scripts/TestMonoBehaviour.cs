using System;
using System.Collections.Generic;
using RotationTypes;
using UnityEngine;

public class TestMonoBehaviour : MonoBehaviour
{
    [SerializeField] private TestClass testObject = new TestClass("initialisedTestName");

    [ContextMenu("CopyTestNameToChild")]
    public void CopyTestNameToChild()
    {
        if (testObject.child.parent != testObject)
        {
            Debug.LogError("testObject.child.parent != testObject");
            testObject.child.parent = testObject; 
        }
        testObject.child.childName = testObject.child.parent.testName; 
    }
    
    [ContextMenu("RegenTestObj")]
    public void RegenTestObj()
    {
        testObject = new TestClass("initialisedTestName"); 
    }
}
