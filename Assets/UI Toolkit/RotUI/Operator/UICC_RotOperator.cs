using UnityEditor;
using UnityEngine.UIElements;

namespace UI_Toolkit.RotUI
{
    [UxmlElement]
    public partial class UICC_Operator : VisualElement
    {
        public UICC_Operator()
        {
            #region Load Stylesheets
            #if UNITY_EDITOR
            StyleSheet rotStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/RotUI/USS_Rot.uss");
            StyleSheet boxLayoutStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/BaseUSS/BoxLayouts.uss");

            if (rotStyle != null) styleSheets.Add(rotStyle);
            if (boxLayoutStyle != null) styleSheets.Add(boxLayoutStyle);
            #endif
            #endregion // Load Stylesheets

            VisualElement veRoot = new VisualElement();
            Add(veRoot); 
        }
    }
}
