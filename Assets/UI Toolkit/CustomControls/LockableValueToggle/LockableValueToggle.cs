using UnityEditor;
using UnityEngine.UIElements;

namespace UI_Toolkit.CustomControls
{
    [UxmlElement]
    public partial class LockableValueToggle : BaseBoolField
    {
        private static readonly string ussClassName = "lockable-value-toggle";
        private new static readonly string inputUssClassName = ussClassName + "__input";
        private static readonly string iconUssClassName = ussClassName + "__icon";

        private VisualElement visualInput; 
        private readonly VisualElement _icon; 
        
        public LockableValueToggle() : base((string) null)
        {
            AddToClassList(ussClassName);

            if (!QueryVisualInput())
            {
                throw new System.Exception("LockableValueToggle could not find visualInput");
            }
            
            visualInput.AddToClassList(inputUssClassName);
            visualInput.Clear(); 
            
            _icon = new VisualElement();
            _icon.pickingMode = PickingMode.Ignore; //let cliks pass through to the field
            visualInput.Add(_icon);
        }
        
        public LockableValueToggle(string label, bool initialValue = false) : this()
        {
            this.label = label; 
            SetValueWithoutNotify(initialValue);
        }
        
        private bool QueryVisualInput()
        {
            visualInput = this.Q<VisualElement>(className: Toggle.inputUssClassName);
            return visualInput != null;
        }
        
        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);
            style.rotate = new Rotate(new Angle(newValue ? -90 : 0, AngleUnit.Degree));
        }

        // Claude claims I don't need this section, which kind of makes sense, given that it only references itself and nothing from the outside references it (at least not directly in code)
        // public LockableValueToggle(string label, IBinding binding, string bindingPath, bool initialValue = false) : base(label)
        // {
        //     this.binding = binding;
        //     this.bindingPath = bindingPath;
        // }
        //
        // public IBinding binding { get; set; }
        // public string bindingPath { get; set; }
    }
}