using UnityEngine.UIElements;

namespace UI_Toolkit
{
    public static class UIExtensions
    {
        public static Label CreateLabel(string text, string name, string className = "SinCosText")
        {
            Label label = new Label(text);
            if (!string.IsNullOrEmpty(name)) label.name = name;
            if (!string.IsNullOrEmpty(className)) label.AddToClassList(className);
            return label;
        }

        public static FloatField CreateFloatField(string label, float value, string name, string className = null)
        {
            FloatField field = new FloatField(label)
            {
                value = value,
                name = name
            };
            if (!string.IsNullOrEmpty(className))
                field.AddToClassList(className);
            return field;
        }
    }
}