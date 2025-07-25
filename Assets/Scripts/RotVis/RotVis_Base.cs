using System.ComponentModel;
using RotObj;
using RotParams;
using UnityEngine;

namespace BaseClasses
{
    public abstract class RotVis_Base : MonoBehaviour
    {
        public abstract RotParams_Base GetRotParams();
        
        public virtual void SetRotParamsByRef(RotParams_Base newRotParams)
        {
            GetRotParams().PropertyChanged -= VisUpdateOnRotParamsChanged; 
            newRotParams.PropertyChanged += VisUpdateOnRotParamsChanged; 
            VisUpdate();
        }

        private void VisUpdateOnRotParamsChanged(object sender, PropertyChangedEventArgs e)
        {
            VisUpdate();
        }

        public RotVis_Base(RotParams_Base rotParams)
        {
            SetRotParamsByRef(rotParams);
            VisUpdate();
        }
        
        public abstract void VisUpdate();
    }
}