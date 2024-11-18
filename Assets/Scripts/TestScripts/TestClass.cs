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
        [SerializeField] private float halfFloatField; 
        public float GSFloatProp
        {
            get
            {
                Debug.Log($" testFloatField: {testFloatField}" + (testFloatField/2 != halfFloatField ? $" !=halfFloatField {halfFloatField}" : "")); 
                return testFloatField; 
            }
            set
            {
                Debug.Log($"setFloatField from: {testFloatField}, to({value})"); 
                testFloatField = value;
                halfFloatField = testFloatField / 2; 
            }
        }

        public void SetTestFloat()
        {
            Debug.Log("SetTestFloat Called");
            testFloatField = -123;
            halfFloatField = -321; 
        }
    }
}
