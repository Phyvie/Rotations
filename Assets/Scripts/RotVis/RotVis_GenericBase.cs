using System.ComponentModel;
using RotParams;
using UnityEngine;

namespace BaseClasses
{
    /* GenericBaseClass with a further TemplateBaseClass (below) to create and adjust the Visualisation (e.g. the Rotation axis GameObject) of Rotations depending on the Parameters (e.g. RotParams_AxisAngle) of the Rotation
     */
    public abstract class RotVis_GenericBase : MonoBehaviour
    {
        public abstract RotParams_Base GetRotParams();

        private void OnEnable()
        {
            VisUpdate(); 
        }

        public abstract void SetRotParamsByRef(RotParams_Base newRotParams); 
        
        public abstract void VisUpdate();
    }

    public abstract class RotVis_TemplateBase<TRotParams> : RotVis_GenericBase where TRotParams : RotParams_Base
    {
        [SerializeField] protected TRotParams rotParams;
        
        public sealed override RotParams_Base GetRotParams()
        {
            return rotParams; 
        }

        public sealed override void SetRotParamsByRef(RotParams_Base newRotParams)
        {
            /* LaterZyKa RotVis_RotParams: null should be possible, it should just deactivate all the remaining visualisation */
            
            if (newRotParams == null)
            {
                throw new System.ArgumentNullException($"{nameof(newRotParams)}");
            }

            if (newRotParams is not TRotParams)
            {
                throw new System.ArgumentException($"{nameof(newRotParams)} is not of type {nameof(TRotParams)}");
            }
            
            rotParams.PropertyChanged -= VisUpdateOnRotParamsChanged; 

            rotParams = newRotParams as TRotParams;
            rotParams.PropertyChanged += VisUpdateOnRotParamsChanged;

            VisUpdate();
        }
        
        protected void VisUpdateOnRotParamsChanged(object sender, PropertyChangedEventArgs e)
        {
            VisUpdate();
        }
    }
}