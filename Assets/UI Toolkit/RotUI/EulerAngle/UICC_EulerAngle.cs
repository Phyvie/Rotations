using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace UI_Toolkit.RotUI
{
    [UxmlElement]
    public partial class UICC_EulerAngle : VisualElement
    {
        public UICC_EulerAngle()
        {
            #region Load Stylesheets
#if UNITY_EDITOR
            var rotStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/RotUI/USS_Rot.uss");
            var boxLayoutStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/BaseUSS/BoxLayouts.uss");
            var dropdownStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/BaseUSS/DropdownWithFloat.uss");

            if (rotStyle != null) styleSheets.Add(rotStyle);
            if (boxLayoutStyle != null) styleSheets.Add(boxLayoutStyle);
            if (dropdownStyle != null) styleSheets.Add(dropdownStyle);
#endif
            #endregion // Load Stylesheets

            #region Root Setup
            name = "EulerAngleRoot";
            style.flexGrow = 1; // [MARKED] flex-grow: 1;
            #endregion // Root Setup

            #region Add Axes
            for (int i = 0; i < 3; i++)
            {
                var axisContainer = new VisualElement
                {
                    name = "FirstAxis" // repeated name; you may want to make these unique in practice
                };
                axisContainer.AddToClassList("DropdownWithFloat");

                axisContainer.style.flexGrow = 1; // [MARKED]
                axisContainer.style.flexDirection = FlexDirection.Row; // [MARKED]
                axisContainer.style.justifyContent = Justify.SpaceAround; // [MARKED]

                DropdownField axisDropdownField = new DropdownField
                {
                    label = "Axis",
                    choices = new List<string> { "Yaw", "Pitch", "Roll" },
                    index = 0
                };
                FloatField valueField = UIExtensions.CreateFloatField("Float Field", 42.2f, null);

                axisContainer.Add(axisDropdownField);
                axisContainer.Add(valueField);

                Add(axisContainer);
            }
            #endregion // Add Axes
        }
    }
}
