using RotationVisualisation;
using RotParams;
using UnityEngine;

namespace RotContainers
{
    public class RotCotQuaternion : RotCot_TemplateBase<RotParams_Quaternion, RotVis_Quaternion>
    {
        [ContextMenu("Reset to Identity")]
        public override void ResetToIdentity()
        {
            base.ResetToIdentity();
        }
    }
}