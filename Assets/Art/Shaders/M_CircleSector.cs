using Art.Shaders;
using UnityEngine;

public class M_CircleSector : M_ShaderValueController
{
    [SerializeField] private Color _positiveAngleColor;
    [SerializeField] private Color _negativeAngleColor;
    [SerializeField] private Color _fullRotationColor; 
    [SerializeField] private Color _outsideAngleColor; 
    [SerializeField] private float _beginAngle;
    [SerializeField] private float _endAngle;
    private int _fullRotations;
    private int fullRotations => (int) (Mathf.Abs(_endAngle - _beginAngle) / (2*Mathf.PI));

    public float BeginAngle
    {
        get => _beginAngle;
        set
        {
            _beginAngle = value;
            UpdateShader(); 
        }
    }
    
    public float EndAngle
    {
        get => _endAngle;
        set
        {
            _endAngle = value;
            UpdateShader();
        }
    }

    public Color PositiveAngleColor
    {
        get => _positiveAngleColor;
        set
        {
            _positiveAngleColor = value;
            material.SetColor("_PositiveAngleColour", PositiveAngleColor);
        }
    }

    public Color NegativeAngleColor
    {
        get => _negativeAngleColor;
        set
        {
            _negativeAngleColor = value;
            material.SetColor("_NegativeAngleColour", NegativeAngleColor);
        }
    }

    public Color OutsideAngleColor
    {
        get => _outsideAngleColor;
        set
        {
            _outsideAngleColor = value;
            material.SetColor("_OutsideAngleColour", OutsideAngleColor);
        }
    }

    public Color FullRotationColor
    {
        get => _fullRotationColor;
        set
        {
            _fullRotationColor = value;
            material.SetColor("_FullRotationColour", FullRotationColor);
        }
    }

    void UpdateShader()
    {
        if (material == null) return;
        
        material.SetColor("_PositiveAngleColour", PositiveAngleColor);
        material.SetColor("_NegativeAngleColour", NegativeAngleColor);
        material.SetColor("_OutsideAngleColour", OutsideAngleColor);
        material.SetColor("_FullRotationsColor", FullRotationColor);
        material.SetFloat("_BeginAngle", _beginAngle);
        material.SetFloat("_EndAngle", _endAngle);
        material.SetColor("_InsideAngleColour", 
            BeginAngle < EndAngle ? 
                material.GetColor("_PositiveAngleColour") :
                material.GetColor("_NegativeAngleColour"));
        material.SetFloat("_FullRotations", fullRotations);
    }

    private void OnValidate()
    {
        BeginAngle = _beginAngle;
        EndAngle = _endAngle;
    }
}
