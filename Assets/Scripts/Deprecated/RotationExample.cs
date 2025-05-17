using System;
using UnityEngine;
using UnityEngine.Serialization;

public class RotationCircle : MonoBehaviour
{
    [SerializeField] private GameObject rotationPlane; 
    [SerializeField] private GameObject rotationAxis;
    [SerializeField] private GameObject startingPointPivot;
    [SerializeField] private GameObject endPointPivot;
    [SerializeField] private Color circleColor; 
    
    private readonly String SVN_startAngle = "_StartingAngle"; 
    private readonly String SVN_endAngle = "_EndingAngle"; 
    private readonly String SVN_circleColor = "_CircleColour"; 
    
    private Material shaderMaterial;
    [SerializeField] private float AxisLengthMultiplier = 1.0f;

    void Start()
    {
        shaderMaterial = rotationPlane.GetComponent<MeshRenderer>().materials[0]; 
        Debug.Log(shaderMaterial);
        if (shaderMaterial == null)
        {
            Debug.LogError("Can't update RotationExample Shader, because material is null");
            enabled = false; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCircleShader(); 
        UpdateAxisScale(); 
    }

    [ContextMenu("UpdateCircleShader")]
    void UpdateCircleShader()
    {
        float startingAngle = Mathf.Deg2Rad * startingPointPivot.transform.localEulerAngles.z; 
        shaderMaterial.SetFloat(SVN_startAngle, startingAngle);
        Debug.Log(startingAngle);
        float endingAngle = Mathf.Deg2Rad * endPointPivot.transform.localEulerAngles.z; 
        shaderMaterial.SetFloat(SVN_endAngle, endingAngle);
        Debug.Log(endingAngle);
        shaderMaterial.SetColor(SVN_circleColor, circleColor);
    }

    [ContextMenu("UpdateAxisScale")]
    void UpdateAxisScale()
    {
        rotationAxis.transform.localScale = new Vector3(
            rotationAxis.transform.localScale.x,
            rotationAxis.transform.localScale.y,
            (endPointPivot.transform.localEulerAngles.z - startingPointPivot.transform.localEulerAngles.z) * AxisLengthMultiplier); 
    }
}
