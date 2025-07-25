using System.ComponentModel;
using RotParams;
using UnityEngine;

namespace BaseClasses
{
    //-ZyKa check which MonoBehaviours can be reduced to Behaviour or Component
    //0ZyKa check whether you can remove the GenericBase class here
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
            GetRotParams().PropertyChanged -= VisUpdateOnRotParamsChanged; 
            newRotParams.PropertyChanged += VisUpdateOnRotParamsChanged;

            if (newRotParams is TRotParams newRP)
            {
                rotParams = newRP;
            }
            else
            {
                rotParams = rotParams.ToSelfType(newRotParams) as TRotParams; 
            }
            
            VisUpdate();
        }
        
        protected void VisUpdateOnRotParamsChanged(object sender, PropertyChangedEventArgs e)
        {
            VisUpdate();
        }
    }
}