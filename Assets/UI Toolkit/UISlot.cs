using UnityEngine.UIElements;

namespace UI_Toolkit
{
    [UxmlElement]
    public partial class UISlot : VisualElement
    {
        private VisualElement currentContent;

        public UISlot()
        {
            AddToClassList("ui-slot");
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
}