using System;
using BaseClasses;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_Matrix : RotVis<RotParams_Matrix>
    {
        [SerializeField] private Vis_Vector visVectorRight; 
        [SerializeField] private Vis_Vector visVectorUp; 
        [SerializeField] private Vis_Vector visVectorForward;

        public RotVis_Matrix(RotParams_Matrix rotParams_Matrix) : base(rotParams_Matrix)
        {
            
        }
        
        public override void VisUpdate()
        {
            if (visVectorRight is null)
            {
                Debug.Log("IsApplicationPlaying: " + Application.isPlaying);
            }
            if (visVectorRight is not null)
            {
                visVectorRight.Value = _rotParams.GetColumn(0);
            }
            if (visVectorUp is not null)
            {
                visVectorUp.Value = _rotParams.GetColumn(1);
            }
            if (visVectorForward is not null)
            {
                visVectorForward.Value = _rotParams.GetColumn(2);
            }
        }

        private void OnValidate()
        {
            VisUpdate();
        }
    }
}