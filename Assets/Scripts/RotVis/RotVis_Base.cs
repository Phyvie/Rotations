using System.ComponentModel;
using RotParams;
using UnityEngine;

namespace BaseClasses
{
    public abstract class RotVis_Base : MonoBehaviour
    {
        [SerializeField] private GameObject appliedRotationObject; //child of the actual rotationObject, whose child are the actual objects
        [SerializeField] protected GameObject rotationObject; 
        
        public abstract RotParams_Base GetRotParams();

        public virtual void SetRotParamsByRef(RotParams_Base newRotParams)
        {
            GetRotParams().PropertyChanged -= VisUpdateOnPropertyChanged; 
            newRotParams.PropertyChanged += VisUpdateOnPropertyChanged; 
            VisUpdate();
        }

        private void VisUpdateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            VisUpdate();
        }

        public RotVis_Base(RotParams_Base rotParams)
        {
            SetRotParamsByRef(rotParams);
            VisUpdate();
        }
        
        public abstract void VisUpdate();

        public void VisUpdateRotationObject()
        {
            rotationObject.transform.localRotation = GetRotParams().ToUnityQuaternion(); 
        }
        
        public void ReplaceRotationObject(GameObject newRotationObject, bool isPrefab)
        {
            Transform parent = rotationObject.transform.parent;
            Destroy(rotationObject);
            if (isPrefab)
            {
                Instantiate(newRotationObject, parent);
            }
            else
            {
                rotationObject = newRotationObject;
                newRotationObject.transform.SetParent(parent);
            }
        }

        [ContextMenu("ApplyObjectRotation")]
        public void ApplyObjectRotation()
        {
            appliedRotationObject.transform.localRotation =
                GetRotParams().ToUnityQuaternion() * appliedRotationObject.transform.localRotation; 
            GetRotParams().ResetToIdentity(); 
        }

        [ContextMenu("ResetAppliedObjectRotation")]
        public void ResetAppliedObjectRotation()
        {
            appliedRotationObject.transform.localRotation = Quaternion.identity; 
        }
    }
}