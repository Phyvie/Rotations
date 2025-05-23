using UnityEngine;

namespace MathExtensions
{
    public static class ExtensionMethods
    {
        public static void GetTransformRelativeTo(this Transform t, Transform transform, 
            ref Vector3 relativePosition, ref Quaternion relativeRotation, ref Vector3 relativeScale)
        {
            if (transform is null)
            {
                relativePosition = t.position;
                relativeRotation = t.rotation;
                relativeScale = t.localScale;
            }
            else
            {
                relativePosition = transform.InverseTransformPoint(t.position);
                relativeRotation = Quaternion.Inverse(transform.rotation) * t.rotation;
                relativeScale = Vector3.Scale(t.lossyScale, transform.lossyScale);  
            }
        }
    }
}
