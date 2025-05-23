using UnityEngine;

namespace RotParams
{
    [System.Serializable] public struct LockableFloat 
    { 
        [SerializeField] public float value; 
        [SerializeField] public bool isLocked; 
        
        LockableFloat(float value, bool isLocked)
        {
            this.value = value; 
            this.isLocked = isLocked;
        } 
        
        public float Value
        {
            get => value;
            set
            {
                if (!isLocked)
                {
                    value = value;
                }
                else
                {
                    Debug.LogWarning("Value is locked and cannot be changed.");
                }
            }
        } 
        
        public static implicit operator float(LockableFloat lockableFloat)
        {
            return lockableFloat.Value;
        }
        
        //TODO: this definitely doesn't work
        public static implicit operator LockableFloat(float value) 
        { 
            LockableFloat lockableFloat = new LockableFloat(value, false); 
            return lockableFloat; 
        }
        
        void SetValue(float value)
        {
            if (!isLocked)
            {
                Value = value;
            }
            else
            {
                Debug.LogWarning("Value is locked and cannot be changed.");
            }
        }
    }
}