using System;
using RotationTypes;
using UnityEngine;

namespace TestScripts
{
    [ExecuteInEditMode]
    public class TestMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private AngleType angleTypeA = null; 
        [SerializeField] private AngleType angleTypeB = AngleType.Degree; 
        
        [SerializeField] private TestClass testObject; 
        [SerializeField] private TestClass[] testObjectArray; 

        [ContextMenu("RecreateTestObjectArray")]
        private void RecreateTestObjectArray()
        {
            testObjectArray =  new TestClass[]{new TestClass(), new TestClass(), new TestClass()};
        }

        public TestClass TestObject
        {
            get
            {
                Debug.Log("Getting TestObject as Property");
                return testObject;
            }
            set
            {
                Debug.Log("Setting TestObject as Property");
                testObject = value;
            }
        }

        private void Update()
        {
            Debug.Log(angleTypeA);
            Debug.Log(angleTypeB);
        }
    }
}
