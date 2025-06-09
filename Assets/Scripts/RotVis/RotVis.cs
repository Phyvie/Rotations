using UnityEngine;
using UnityEngine.Serialization;

namespace BaseClasses
{
    public abstract class RotVis : MonoBehaviour
    {
        public abstract RotParams.RotParams GetRotParams(); 
        
        public abstract void SetRotParams(ref RotParams.RotParams rotParams);
        
        public RotVis(RotParams.RotParams rotParams)
        {
            SetRotParams(ref rotParams);
            VisUpdate();
        }
        
        public abstract void VisUpdate(); 
    }
}