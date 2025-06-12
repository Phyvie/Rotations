using UnityEngine;

namespace RotParams
{
    [System.Serializable] 
    public class LockableValue<Type> 
    { 
        [SerializeField] private Type typeValue; 
        [SerializeField] public bool isLocked; 
        
        public LockableValue(Type newTypeValue, bool isLocked)
        {
            this.typeValue = newTypeValue; 
            this.isLocked = isLocked;
        } 
        
        public Type TypeValue
        {
            get => typeValue;
            set
            {
                if (!isLocked)
                {
                    this.typeValue = value;
                }
                else
                {
                    Debug.LogWarning("Value is locked and cannot be changed.");
                }
            }
        } 
        
        public static implicit operator Type(LockableValue<Type> lockableValue)
        {
            return lockableValue.TypeValue;
        }
        
        public static implicit operator LockableValue<Type>(Type value) 
        { 
            LockableValue<Type> lockableValue = new LockableValue<Type>(value, false); 
            return lockableValue; 
        }
        
        public void SetValue(Type value, bool forceSet = false)
        {
            if (!isLocked || forceSet)
            {
                typeValue = value;
            }
            else
            {
                Debug.Log("Value is locked and cannot be changed.");
            }
        }
        
        #region UXMLSupport
        
        #endregion UXMLSupport
    }

    [System.Serializable]
    public class LockableFloat : LockableValue<float>
    {
        public LockableFloat(float newTypeValue, bool isLocked) : base(newTypeValue, isLocked)
        {
        }
    }
}