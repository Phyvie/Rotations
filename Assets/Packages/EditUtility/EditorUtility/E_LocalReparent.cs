using UnityEngine;

namespace Editor
{
    [ExecuteInEditMode]
    public class E_ReparentComponent : MonoBehaviour
    {
        [SerializeField] private bool bKeepLocalLocation; 
        [SerializeField] private bool bKeepLocalRotation;
        [SerializeField] private bool bKeepLocalScale;
    
        private Vector3 localLocation;
        private Quaternion localRotation;
        private Vector3 localScale; 
    
        void StoreLocalValues()
        {
            localLocation = transform.localPosition;
            localRotation = transform.localRotation;
            localScale = transform.localScale;
        }

        void RestoreLocalValues()
        {
            if (bKeepLocalLocation)
            {
                transform.localPosition = localLocation;
            }
            if (bKeepLocalRotation)
            {
                transform.localRotation = localRotation;
            }
            if (bKeepLocalRotation)
            {
                transform.localScale = localScale;
            }
        }

        private void OnBeforeTransformParentChanged()
        {
            Debug.Log($"{nameof(OnBeforeTransformParentChanged)} called"); 
            StoreLocalValues();
        }

        private void OnTransformParentChanged()
        {
            Debug.Log($"{nameof(OnTransformParentChanged)} called");
            RestoreLocalValues();
        }
    }
}
