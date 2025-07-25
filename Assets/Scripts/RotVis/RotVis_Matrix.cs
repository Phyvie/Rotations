using System;
using BaseClasses;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_Matrix : RotVis_TemplateBase<RotParams_Matrix>
    {
        [SerializeField] private Vis_Vector visVectorRight; 
        [SerializeField] private Vis_Vector visVectorUp; 
        [SerializeField] private Vis_Vector visVectorForward;

        public RotVis_Matrix(RotParams_Matrix rotParams_Matrix) : base(rotParams_Matrix)
        {
            
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
        }

        #if UNITY_EDITOR
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
        #endif
    }
}