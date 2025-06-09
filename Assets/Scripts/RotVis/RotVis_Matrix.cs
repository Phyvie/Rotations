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
        public override RotParams.RotParams GetRotParams()
        {
            return rotParams; 
        }

        public override void SetRotParams(RotParams.RotParams newRotParams)
        {
            if (newRotParams.GetType().IsSubclassOf(typeof(RotParams_Matrix)))
            {
                rotParams = (RotParams_Matrix)newRotParams;
            }
        }
        
        public override void VisUpdate()
        {
            if (visVectorRight is not null)
            {
                visVectorRight.Value = rotParams.GetColumn(0);
            }
            if (visVectorUp is not null)
            {
                visVectorUp.Value = rotParams.GetColumn(1);
            }
            if (visVectorForward is not null)
            {
                visVectorForward.Value = rotParams.GetColumn(2);
            }
        }

        private void OnValidate()
        {
            VisUpdate();
        }
    }
}