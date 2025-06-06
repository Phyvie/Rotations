using UnityEditor;
using UnityEngine.UIElements;

namespace UI_Toolkit.RotUI
{
    [UxmlElement]
    public partial class UICC_Quaternion : VisualElement
    {
        public UICC_Quaternion()
        {
            #region Load Stylesheets
#if UNITY_EDITOR
            StyleSheet ussQuaternion =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/RotUI/Quaternion/USS_Quaternion.uss");
            if (ussQuaternion != null) styleSheets.Add(ussQuaternion);

            StyleSheet ussLabeledFloat =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/BaseElements/labeledFloat.uss");
            if (ussLabeledFloat != null) styleSheets.Add(ussLabeledFloat);

            StyleSheet ussBoxLayouts =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/BaseUSS/BoxLayouts.uss");
            if (ussBoxLayouts != null) styleSheets.Add(ussBoxLayouts);
#endif

            #endregion // Load Stylesheets

            #region SinCosContainer

            var sinCosContainer = new VisualElement
            {
                name = "SinCosContainer"
            };
            sinCosContainer.AddToClassList("container--horizontalspacearound");

            #endregion // SinCosContainer

            #region SinContainer

            var sinContainer = new VisualElement
            {
                name = "SinContainer"
            };
            sinContainer.AddToClassList("container--horizontalspacearound");
            sinContainer.Add(UIExtensions.CreateLabel("Sin(", "SinLabel"));
            sinContainer.Add(UIExtensions.CreateFloatField("Alpha", 42.2f, "AlphaField"));
            sinContainer.Add(UIExtensions.CreateLabel(")", "SinLabel"));

            #endregion // SinContainer

            #region Plus Label

            var plusLabel = UIExtensions.CreateLabel("+", "Plus");

            #endregion // Plus Label

            #region CosContainer

            var cosContainer = new VisualElement
            {
                name = "CosContainer"
            };
            cosContainer.AddToClassList("container--horizontalspacearound");

            cosContainer.Add(UIExtensions.CreateLabel("Cos(", "CosOpen"));
            cosContainer.Add(UIExtensions.CreateFloatField("Alpha", 42.2f, "Alpha"));
            cosContainer.Add(UIExtensions.CreateLabel(")", "CosClose"));
            cosContainer.Add(UIExtensions.CreateLabel("(", "UnitVectorOpen"));

            var cosValueContainer = new VisualElement
            {
                name = "CosValueContainer"
            };
            cosValueContainer.AddToClassList("container--horizontalspacearound"); // replaced styling with class

            cosValueContainer.Add(UIExtensions.CreateFloatField("X", 42.2f, "XField"));
            cosValueContainer.Add(UIExtensions.CreateFloatField("Y", 42.2f, "YField"));
            cosValueContainer.Add(UIExtensions.CreateFloatField("Z", 42.2f, "ZField"));

            cosContainer.Add(cosValueContainer);
            cosContainer.Add(UIExtensions.CreateLabel(")\n", "UnitVectorClose"));

            #endregion // CosContainer

            #region QuatContainer

            var quatContainer = new VisualElement
            {
                name = "QuatContainer"
            };
            quatContainer.AddToClassList("container--horizontalspacearound");

            quatContainer.Add(UIExtensions.CreateLabel("=", null));
            quatContainer.Add(UIExtensions.CreateFloatField("W", 42.2f, "WField"));
            quatContainer.Add(UIExtensions.CreateFloatField("X", 42.2f, "XField"));
            quatContainer.Add(UIExtensions.CreateFloatField("Y", 42.2f, "YField"));
            quatContainer.Add(UIExtensions.CreateFloatField("Z", 42.2f, "ZField"));

            #endregion // QuatContainer

            #region Compose

            Add(sinCosContainer);
            {
                sinCosContainer.Add(sinContainer);
                sinCosContainer.Add(plusLabel);
                sinCosContainer.Add(cosContainer);
            }

            Add(quatContainer);

            #endregion // Compose
        }
    }
}