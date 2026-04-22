using RotationVisualisation;
using RotParams;
using UnityEngine;

namespace RotContainers
{
    public class RotCotAxisAngle : RotCot_TemplateBase<RotParams_AxisAngle, RotVis_AxisAngle>
    {
        [ContextMenu("Reset to Identity")]
        public override void ResetToIdentity()
        {
            base.ResetToIdentity();
        }
    }
}