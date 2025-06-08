using UnityEditor;
using UnityEngine.UIElements;

namespace UI_Toolkit.RotUI
{
    /*
    [UxmlElement]
    public partial class UICC_Matrix : VisualElement
    {
        public UICC_Matrix()
        {
            #region Load Stylesheets
#if UNITY_EDITOR
            StyleSheet ussRot = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/RotUI/USS_Rot.uss"); 
            StyleSheet ussMatrix = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/RotUI/Matrix/USS_Matrix.uss");
            StyleSheet ussBoxLayout = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/BaseUSS/BoxLayouts.uss");
            StyleSheet ussSingleLabelVector = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/BaseUSS/singleLabelVector.uss");

            if (ussRot != null) styleSheets.Add(ussRot);
            if (ussMatrix != null) styleSheets.Add(ussMatrix);
            if (ussBoxLayout != null) styleSheets.Add(ussBoxLayout);
            if (ussSingleLabelVector != null) styleSheets.Add(ussSingleLabelVector);
#endif
            #endregion // Load Stylesheets

            #region MatrixRoot Setup
            name = "MatrixRoot";
            AddToClassList("container--horizontalspacearound");
            style.width = 500;   // [MARKED] width: 500px;
            style.height = 250;  // [MARKED] height: 250px;
            #endregion // MatrixRoot Setup

            #region Add Vector3Fields
            Vector3Field columnLabelContainer = new Vector3Field("_") { name = "ColumnLabelContainer" };
            Vector3Field xVector = new Vector3Field("Right (X-Vector)") { name = "XVector" };
            Vector3Field yVector = new Vector3Field("Up (Y-Vector)") { name = "YVector" };
            Vector3Field zVector = new Vector3Field("Forward (Z-Vector)") { name = "ZVector" };
            #endregion // Add Vector3Fields

            #region Compose
            Add(columnLabelContainer);
            Add(xVector);
            Add(yVector);
            Add(zVector);
            #endregion Compose
        }
    }
    */
}
