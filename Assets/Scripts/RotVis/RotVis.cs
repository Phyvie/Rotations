using UnityEngine;
using UnityEngine.Serialization;

namespace BaseClasses
{
    public abstract class RotVis<TRotParams> : MonoBehaviour
    {
        [FormerlySerializedAs("rotParams")] [SerializeField] protected TRotParams _rotParams;

        public RotVis(TRotParams rotParams)
        {
            _rotParams = rotParams;
            VisUpdate();
        }
        
        public TRotParams RotParams
        {
            get => _rotParams;
            set => _rotParams = value;
        }
        
        public abstract void VisUpdate(); 
    }
}