using System.ComponentModel;
using UnityEngine;

namespace BaseClasses
{
    public abstract class RotVis : MonoBehaviour
    {
        [SerializeField] private GameObject rotationObjectParent; //used in order to apply rotation (i.e. setting the parameters to 0 while keeping the objects orientation)
        [SerializeField] protected GameObject rotationObject; 
        
        public abstract RotParams.RotParams GetRotParams();

        public virtual void SetRotParamsByRef(ref RotParams.RotParams newRotParams)
        {
            GetRotParams().PropertyChanged -= VisUpdateOnPropertyChanged; 
            newRotParams.PropertyChanged += VisUpdateOnPropertyChanged; 
        }

        private void VisUpdateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            VisUpdate();
        }

        public RotVis(RotParams.RotParams rotParams)
        {
            SetRotParamsByRef(ref rotParams);
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
            rotationObjectParent.transform.localRotation *= GetRotParams().ToUnityQuaternion();
            GetRotParams().ResetToIdentity(); 
        }

        [ContextMenu("ResetAppliedObjectRotation")]
        public void ResetAppliedObjectRotation()
        {
            rotationObjectParent.transform.rotation = Quaternion.identity; 
        }
    }
}