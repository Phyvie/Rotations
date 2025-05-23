using System;
using RotParams;
using UnityEngine;
using Visualisation;

namespace RotationVisualisation
{
    public class RotVis_Matrix : MonoBehaviour
    {
        [SerializeField] private RotParams_Matrix rotParams;
        [SerializeField] private Vis_Vector visVectorRight; 
        [SerializeField] private Vis_Vector visVectorUp; 
        [SerializeField] private Vis_Vector visVectorForward;

        private void VisUpdate()
        {
            if (visVectorRight is null)
            {
                Debug.Log("IsApplicationPlaying: " + Application.isPlaying);
            }
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