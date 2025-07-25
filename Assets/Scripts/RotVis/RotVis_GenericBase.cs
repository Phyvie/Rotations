using System.ComponentModel;
using RotParams;
using UnityEngine;

namespace BaseClasses
{
    /* GenericBaseClass with a further TemplateBaseClass (below) to create and adjust the Visualisation (e.g. the Rotation axis GameObject) of Rotations depending on the Parameters (e.g. RotParams_AxisAngle) of the Rotation
     */
    public abstract class RotVis_GenericBase : MonoBehaviour
    {
        protected RotVis_GenericBase()
        {
            throw new System.NotImplementedException();
        }
        
        public RotVis_GenericBase(RotParams_Base rotParams)
        {
            SetRotParamsByRef(rotParams);
            VisUpdate();
        }
        
        public abstract RotParams_Base GetRotParams();
        
        public abstract void SetRotParamsByRef(RotParams_Base newRotParams); 
        
        public abstract void VisUpdate();
    }

    public abstract class RotVis_TemplateBase<TRotParams> : RotVis_GenericBase where TRotParams : RotParams_Base
    {
        [SerializeField] protected TRotParams rotParams;

        public RotVis_TemplateBase(TRotParams rotParams) : base(rotParams)
        {
        }
        
        public sealed override RotParams_Base GetRotParams()
        {
            return rotParams; 
        }

        public sealed override void SetRotParamsByRef(RotParams_Base newRotParams)
        {
            if (newRotParams == null)
            {
                throw new System.ArgumentNullException($"{nameof(newRotParams)}");
            }
            
            rotParams.PropertyChanged -= VisUpdateOnRotParamsChanged; 

            rotParams = rotParams.ToSelfType(newRotParams) as TRotParams;
            rotParams.PropertyChanged += VisUpdateOnRotParamsChanged;

            VisUpdate();
        }
        
        protected void VisUpdateOnRotParamsChanged(object sender, PropertyChangedEventArgs e)
        {
            VisUpdate();
        }
    }
}