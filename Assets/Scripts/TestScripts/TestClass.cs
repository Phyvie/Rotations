using System;
using UnityEngine;

namespace TestScripts
{
    [Serializable]
    public class TestClass
    {
        public TestClass(float value)
        {
            GSFloatProp = value; 
        }
        
        [SerializeField] private float testFloatField;
        public float GSFloatProp
        {
            get
            {
                Debug.Log($"testFloat.Get = {testFloatField}"); 
                return testFloatField; 
            }
            set
            {
                Debug.Log($"testFloat.Set({value})"); 
                testFloatField = (float) Math.Truncate(value); 
            }
        }
    }
}
