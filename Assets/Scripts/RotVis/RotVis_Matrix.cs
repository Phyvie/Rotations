using System;
using BaseClasses;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_Matrix : RotVis
    {
        [SerializeField] private Vis_Vector visVectorRight; 
        [SerializeField] private Vis_Vector visVectorUp; 
        [SerializeField] private Vis_Vector visVectorForward;

        public RotVis_Matrix(RotParams_Matrix rotParams_Matrix) : base(rotParams_Matrix)
        {
            
        }
        
        [SerializeField] private RotParams_Matrix rotParams;
        public override RotParams_Base GetRotParams()
        {
            return rotParams; 
        }

        public override void SetRotParamsByRef(ref RotParams_Base newRotParams)
        {
            base.SetRotParamsByRef(ref newRotParams);
            if (newRotParams is RotParams_Matrix rotParamsMatrix)
            {
                rotParams = rotParamsMatrix;
                VisUpdate();
            }
        }
        
        public override void VisUpdate()
        {
            if (visVectorRight is not null)
            {
                visVectorRight.Value = rotParams.XVector;
            }
            if (visVectorUp is not null)
            {
                visVectorUp.Value = rotParams.YVector;
            }
            if (visVectorForward is not null)
            {
                visVectorForward.Value = rotParams.ZVector;
            }
            VisUpdateRotationObject();
        }

        private void OnValidate()
        {
            try
            {
                VisUpdate();
            }
            catch (Exception e)
            {
                Debug.Log($"{name} OnValidateError {e.Message}");
            }
        }
    }
}