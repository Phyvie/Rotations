using UnityEngine.UIElements;

namespace UI_Toolkit.CustomControls
{
    [UxmlElement]
    public partial class LockableValueToggle : BaseBoolField
    {
        public LockableValueToggle() : base((string) null)
        {
            
        }
        
        public LockableValueToggle(string label, bool value, IBinding binding, string bindingPath) : base(label)
        {
            this.binding = binding;
            this.bindingPath = bindingPath;
        }

        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);
            style.rotate = new Rotate(new Angle(newValue ? -90 : 0, AngleUnit.Degree));
        }

        public IBinding binding { get; set; }
        public string bindingPath { get; set; }
    }
}