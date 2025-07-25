using UnityEngine;
using UnityEngine.Serialization;

namespace RotObj
{
    /* manages the objects which get visually rotated (e.g. the airplane)
     * 
     */
    public class RotObjCot : MonoBehaviour
    {
        //+ZyKa Create this class
        [SerializeField] private GameObject appliedRotationObject; 
        [SerializeField] private GameObject rotationObjectsParents;
        [SerializeField] private GameObject currentlyActiveRotationObject;

        public Quaternion GetRotation(bool includeApplied = false)
        {
            Quaternion rotation = rotationObjectsParents.transform.localRotation;
            if (includeApplied)
            {
                rotation = appliedRotationObject.transform.localRotation * rotation; 
            }
            return rotation; 
        }

        public void SetRotation(Quaternion rotation, bool ignoreApplied = false)
        {
            if (ignoreApplied)
            {
                rotation = Quaternion.Inverse(appliedRotationObject.transform.localRotation) * rotation; 
            }
            appliedRotationObject.transform.localRotation = rotation;
        }

        public Quaternion GetAppliedRotation()
        {
            return appliedRotationObject.transform.localRotation;
        }

        public void SetAppliedRotation(Quaternion rotation, bool addToAppliedRotation = false)
        {
            if (addToAppliedRotation)
            {
                rotation = GetAppliedRotation() * rotation; 
            }
            appliedRotationObject.transform.localRotation = rotation;
        }
        
        [ContextMenu("ApplyObjectRotation")]
        public void ApplyObjectRotation()
        {
            SetAppliedRotation(rotationObjectsParents.transform.localRotation, true); 
            rotationObjectsParents.transform.localRotation = Quaternion.identity;
        }
        
        [ContextMenu("ResetAppliedObjectRotation")]
        public void ResetAppliedObjectRotation()
        {
            SetAppliedRotation(Quaternion.identity); 
        }
    }
}
