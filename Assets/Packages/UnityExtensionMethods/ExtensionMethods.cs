using UnityEngine;

namespace Packages.UnityExtensionMethods
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
                
                Vector3 targetScale = t.lossyScale;
                Vector3 refScale = transform.lossyScale;
                relativeScale = new Vector3(
                    targetScale.x / refScale.x,
                    targetScale.y / refScale.y,
                    targetScale.z / refScale.z
                );
            }
        }
    }
}
