using UnityEditor;
using UnityEngine.UIElements;

namespace UI_Toolkit.RotUI
{
    [UxmlElement]
    public partial class UICC_AxisAngle : VisualElement
    {
        public UICC_AxisAngle()
        {
            #region Load Stylesheets
            #if UNITY_EDITOR
            StyleSheet rotStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/RotUI/USS_Rot.uss");
            StyleSheet labeledFloatStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/BaseUSS/labeledFloat.uss");
            StyleSheet boxLayoutStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/BaseUSS/BoxLayouts.uss");

            if (rotStyle != null) styleSheets.Add(rotStyle);
            if (labeledFloatStyle != null) styleSheets.Add(labeledFloatStyle);
            if (boxLayoutStyle != null) styleSheets.Add(boxLayoutStyle);
            #endif
            #endregion // Load Stylesheets

            #region AxisAngleRoot
            name = "AxisAngleRoot";

            #region AxisAngleContainer
            VisualElement axisAngle = new VisualElement { name = "AxisAngle" };
            axisAngle.AddToClassList("container--horizontalspacearound");
            #endregion
            
            #region AxisSubContainer
            VisualElement axis = new VisualElement { name = "Axis" };
            axis.AddToClassList("container--horizontalspacearound");
            axis.Add(UIExtensions.CreateFloatField("X", 42.2f, "XValue"));
            axis.Add(UIExtensions.CreateFloatField("Y", 42.2f, "YValue"));
            axis.Add(UIExtensions.CreateFloatField("Z", 42.2f, "ZValue"));
            axisAngle.Add(axis);
            #endregion //AxisSubContainer
            
            #region AngleSubContainer
            VisualElement angle = new VisualElement { name = "Angle" };
            angle.AddToClassList("container--horizontalspacearound");
            angle.Add(UIExtensions.CreateFloatField("Angle", 42.2f, null));
            axisAngle.Add(angle);
            #endregion
            
            #endregion // AxisAngleRoot

            #region RotationVector
            VisualElement rotationVector = new VisualElement { name = "RotationVector" };
            rotationVector.AddToClassList("container--horizontalspacearound");

            Label equalLabel = UIExtensions.CreateLabel("=", "EqualSign", "CenteredText");
            rotationVector.Add(equalLabel);

            FloatField xField = UIExtensions.CreateFloatField("X", 42.2f, "XField");
            FloatField yField = UIExtensions.CreateFloatField("Y", 42.2f, "YField");
            FloatField zField = UIExtensions.CreateFloatField("Z", 42.2f, "ZField");

            rotationVector.Add(xField);
            rotationVector.Add(yField);
            rotationVector.Add(zField);
            #endregion // RotationVector
            
            Add(axisAngle);
            Add(rotationVector);
        }
    }
}
