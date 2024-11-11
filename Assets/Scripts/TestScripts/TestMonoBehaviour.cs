using UnityEngine;

namespace TestScripts
{
    [ExecuteInEditMode]
    public class TestMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private TestClass testObject; 
        [SerializeField] private TestClass[] testObjectArray; 
        // [SerializeField] private int ZyKa = -3; 

        [ContextMenu("RecreateTestObjectArray")]
        private void RecreateTestObjectArray()
        {
            testObjectArray =  new TestClass[]{new TestClass(-1), new TestClass(-2), new TestClass(-3)};
        }
    }
}
