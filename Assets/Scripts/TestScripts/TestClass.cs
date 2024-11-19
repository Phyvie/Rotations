using System;
using UnityEngine;

namespace TestScripts
{
    [Serializable]
    public class TestClass
    {
        public TestClass(float value)
        {
            GSUnitMultiplierField = value; 
        }

        [SerializeField] private float unitMultiplierField;
        [SerializeField] private float currentUnitsField; 
        public float GSUnitMultiplierField
        {
            get
            {
                return unitMultiplierField; 
            }
            set
            {
                // Debug.Log($"setFloatField from: {unitMultiplierField}, to({value})");
                currentUnitsField *= value / unitMultiplierField; 
                unitMultiplierField = value;
            }
        }

        public void SetTestFloat()
        {
            Debug.Log("SetTestFloat Called");
            unitMultiplierField = -123;
            currentUnitsField = -321; 
        }
    }
}
