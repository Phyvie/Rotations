using UnityEngine;
using UnityEngine.Serialization;

namespace BaseClasses
{
    public abstract class RotVis : MonoBehaviour
    {
        private System.Type type; 
        
        public abstract RotParams.RotParams GetRotParams(); 
        public abstract void SetRotParams(RotParams.RotParams rotParams);
        
        public RotVis(RotParams.RotParams rotParams)
        {
            SetRotParams(rotParams);
            VisUpdate();
        }
        
        public abstract void VisUpdate(); 
    }
}