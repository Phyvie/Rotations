using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI_Toolkit
{
    [UxmlElement]
    public partial class UISlot : VisualElement
    {
        private VisualElement currentContent;
        
        public UISlot()
        {
               
        }

        public void Initialize()
        {
            if (childCount > 1)
            {
                Debug.LogError($"UISlot({name}) should not have more than 1 child");
                return; 
            }

            if (childCount == 1)
            {
                currentContent = Children().First();    
            }
        }
        
        public void PlaceInSlot(VisualElement element)
        {
            if (currentContent != null && Contains(currentContent))
            {
                Remove(currentContent);
            }

            currentContent = element;
            if (currentContent != null)
            {
                Add(currentContent);
            }
        }

        public void ClearSlot()
        {
            if (currentContent != null && Contains(currentContent))
            {
                Remove(currentContent);
                currentContent = null;
            }
        }

        public VisualElement Current => currentContent;
    }

    [System.Serializable]
    public class UISlotReference
    {
        public UISlot UISlot { get; set; }
        public string name;

        public void Initialize(VisualElement root)
        {
            UISlot = root.Q<UISlot>();
            if (UISlot == null)
            {
                Debug.LogError($"Could not find UISlot with name {name}");
                return; 
            }
            UISlot.Initialize();
        }

        public void PlaceInSlot(VisualElement element)
        {
            UISlot.PlaceInSlot(element);
        }

        public void ClearSlot()
        {
            UISlot.ClearSlot();
        }
    }
}