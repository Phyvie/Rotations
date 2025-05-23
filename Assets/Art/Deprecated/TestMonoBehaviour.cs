using System;
using RotParams;
using UnityEngine;
using UnityEngine.Serialization;

namespace TestScripts
{
    [ExecuteInEditMode]
    public class TestMonoBehaviour : MonoBehaviour
    {
        [FormerlySerializedAs("quaternionRotation")] [SerializeField] private RotParams_Quaternion rotParamsQuaternion; 
        
        [SerializeField] private TestClass testObject; 
        [SerializeField] private TestClass[] testObjectArray; 

        [ContextMenu("RecreateTestObjectProperties")]
        private void RecreateTestObjectProperties()
        {
            testObject = new TestClass(); 
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
            
        }
    }
}
