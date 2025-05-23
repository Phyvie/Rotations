using Art.Shaders;
using UnityEngine;

public class M_CircleSector : M_ShaderValueController
{
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

    void UpdateShader()
    {
        if (material == null) return;
        
        material.SetFloat("_BeginAngle", _beginAngle);
        material.SetFloat("_EndAngle", _endAngle);
        material.SetColor("_InsideAngleColour", 
            BeginAngle < EndAngle ? 
                material.GetColor("_PositiveAngleColour") :
                material.GetColor("_NegativeAngleColour"));
        material.SetFloat("FullRotations", fullRotations);
    }

    private void OnValidate()
    {
        BeginAngle = _beginAngle;
        EndAngle = _endAngle;
    }
}
