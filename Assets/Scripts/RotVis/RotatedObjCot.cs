using UnityEngine;
using UnityEngine.Serialization;

namespace RotObj
{
    /*
     * manages the objects which get visually rotated (e.g. airplane / ship / ...)
     */
    public class OrientedObject : MonoBehaviour
    {
        //TodoZyKa RotSceneHierarchy: Rework and Document Hierarchy 
        [SerializeField] private Transform userRotation;
        [SerializeField] private Transform appliedRotation;
        [SerializeField] private GameObject currentlyActiveRotationObject;

        public Quaternion GetRotation(bool includeApplied = false)
        {
            Quaternion rotation = userRotation.transform.localRotation;
            if (includeApplied)
            {
                rotation = appliedRotation.transform.localRotation * rotation; 
            }
            return rotation; 
        }

        public void SetRotation(Quaternion rotation, bool ignoreApplied = false)
        {
            if (ignoreApplied)
            {
                rotation = Quaternion.Inverse(appliedRotation.transform.localRotation) * rotation; 
            }
            appliedRotation.transform.localRotation = rotation;
        }

        public Quaternion GetAppliedRotation()
        {
            return appliedRotation.transform.localRotation;
        }

        public void SetAppliedRotation(Quaternion rotation, bool addToAppliedRotation = false)
        {
            if (addToAppliedRotation)
            {
                rotation = GetAppliedRotation() * rotation; 
            }
            appliedRotation.transform.localRotation = rotation;
        }
        
        [ContextMenu("ApplyObjectRotation")]
        public void ApplyObjectRotation()
        {
            SetAppliedRotation(userRotation.transform.localRotation, true); 
            userRotation.transform.localRotation = Quaternion.identity;
        }
        
        [ContextMenu("ResetAppliedObjectRotation")]
        public void ResetAppliedObjectRotation()
        {
            SetAppliedRotation(Quaternion.identity); 
        }
    }
}
